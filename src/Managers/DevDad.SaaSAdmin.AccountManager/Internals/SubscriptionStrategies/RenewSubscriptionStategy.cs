using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

/// <summary>
/// Handles the transformations necessary to renew a subscription.
/// </summary>
sealed class RenewSubscriptionStategy : ChangeStrategyBase
{
    public override string ActivityKind => SubscriptionChangeKinds.ActivityKind_Renew;

    private CustomerSubscription? _subscriptionToUpdate;
    private SubscriptionTemplateResource? _subscriptionTemplate;
    private SubscriptionActionDetail? _changeDetail;

    public RenewSubscriptionStategy() { }


    public override ChangeStrategyResponse ApplyChange(ChangeStrategyRequest request)
    {
        CustomerSubscription? transformedSubscription = null;
        ChangeStrategyResponse response = new ChangeStrategyResponse(request, transformedSubscription);
        response.ChangeCompleted = false;
        string changeStep = "Validation";
        string executionSite = $"{nameof(RenewSubscriptionStategy)}.{nameof(ApplyChange)}{changeStep}";

        var requestErrors = ValidateRequest(request);
        response.AddErrors(requestErrors, executionSite);
        if(response.ShouldHaltProcess)
        {
            return response;
        }

        // Rather than a reference assignment, let's do a deep copy of the current sub.
        transformedSubscription = DomainObjectMapper
            .MapEntities<CustomerSubscription, CustomerSubscription>(_subscriptionToUpdate!);

        // Now, we set the properties on TransformedSubscription in accordance with the change.
        // 1:  Bump the EndDate.
        int daysInPeriod = _subscriptionTemplate!.RenewalPeriodDays;
        transformedSubscription.EndDateUtc = transformedSubscription.EndDateUtc.AddDays(daysInPeriod);
        // 2:  Set WillRenew to True.
        transformedSubscription.WillRenew = true;
        // 3:  Set Status to Active.
        transformedSubscription.CurrentStatus = SubscriptionChangeKinds.ActivityStatusResult[_changeDetail!.ActionName];
        // 4:  Update the Quotas as specified by the Quoate Rules in the template.
        transformedSubscription = UpdateQuotas(transformedSubscription, _subscriptionTemplate);
        // 5:  Add a new Activity to the History.
        SubscriptionActivity activityLogItem = new()
        {
            ActivityKind = _changeDetail.ActionName,
            ActivityDateUTC = DateTime.UtcNow,
            Comment = $"Action Triggered by {_changeDetail.RequestSource}.  Vendor: {_changeDetail.VendorName}."
        };
        // 6:  Put a little dirt under their pillow.  (For the Dirt man)

        response.Payload = transformedSubscription;
        response.ChangeCompleted = true;
        return response;
    }

    /// <summary>
    /// Uses the Quota information on the template to update the user Quotas on the subscription.
    /// </summary>
    /// <param name="subscription">The subscription to update</param>
    /// <param name="template">The template containing the ResourceGrant rules.</param>
    /// <returns>The updated Subscription instance</returns>
    private CustomerSubscription UpdateQuotas(CustomerSubscription subscription, SubscriptionTemplateResource template)
    {
        var aiQuotaRules = template.ResourceGrants.FirstOrDefault(r=> r.ResourceKind == MeteredResourceKinds.NpcAiDetail);
        var storageQuotaRules = template.ResourceGrants.FirstOrDefault(r=> r.ResourceKind == MeteredResourceKinds.NpcStorage);

        // The ResourceGrant specifies the VALUE that a quota of the same kind should be set to upon renewal.     
        // if the ResourceGrant.RenewalBudget is zero, we don't update the user quota.
        if(aiQuotaRules != null && aiQuotaRules.RenewalBudget > 0)
        {
            subscription.Quotas.AiGeneratedNpcs.Budget = aiQuotaRules.RenewalBudget;
        }

        // Storage renewal budget is currently 0 across the board, but we may
        // have subcirption types in the future that grant additional storage
        // each month, so let's just handle this now, because we'll totally forget
        // to make this change if we ever create a subscription that grants more storage.
        if(storageQuotaRules != null && storageQuotaRules.RenewalBudget > 0)
        {
            // With the current subscirption types, this line will never be hit.
            // Don't sweat it if this code doesn't receive coverage.
            subscription.Quotas.StoredNpcs.Budget = storageQuotaRules.RenewalBudget;
        }

        return subscription;
    }

    private IEnumerable<ServiceError> ValidateRequest(ChangeStrategyRequest request)
    {
        List<ServiceError> errors = new();

        var requestErrors = new ChangeRequestValidator().Validate(request);
        errors.AddRange(requestErrors);
        if(errors.Any(e=> e.Severity == ErrorSeverity.Error))
        {
            return errors;
        }

        _changeDetail = request.ChangeDetails;
        _subscriptionToUpdate = request.Payload;
        _subscriptionTemplate = request.TargetTemplate;

        var changeErrors = new SubscriptionActionValidator().Validate(_changeDetail);
        errors.AddRange(changeErrors);

        var subscriptionErrors = new SubscriptionValidator().Validate
            (_subscriptionToUpdate, ValidateCurrentSubscriptionForRenewal);
        errors.AddRange(subscriptionErrors);

        return errors;
    }

    private IEnumerable<ServiceError> ValidateCurrentSubscriptionForRenewal(CustomerSubscription? subscriptionToUpdate)
    {
        List<ServiceError> errors = new();

        if(subscriptionToUpdate==null)
        {
            return errors;
        }

        // When renewing a subscription:
        string activityKind = _changeDetail!.ActionName;
        string currentStatus = subscriptionToUpdate.CurrentStatus;
        bool currentIsFree = subscriptionToUpdate.SKU == SubscriptionIdentifiers.SKUS_TDMF_FREE;

        // Free tier subscriptions are perpetual, and cannot be renewed.
        // Add an error and bail.
        if(currentIsFree)
        {
            errors.Add(new ServiceError
            {
                Severity = ErrorSeverity.Error,
                Message = "Free tier subscriptions cannot be renewed.",
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation
            });
            return errors;
        }

        // 1:  The current subscription's state must be compatible with renewal.
        bool actionIsValidForStatus = SubscriptionStateTransitions
            .CanPerformActivityForStatus(activityKind, currentStatus);

        if(actionIsValidForStatus == false)
        {
            errors.Add(new ServiceError()
            {
                Severity = ErrorSeverity.Error,
                Message = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForStatus,
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation
            });
        }        

        //2:  The subscription's start date must be in the past.
        DateTime now = DateTime.UtcNow;
        if(subscriptionToUpdate.StartDateUtc > now)
        {
            errors.Add(new ServiceError()
            {
                Severity = ErrorSeverity.Error,
                Message = "The subscription's start date is in the future, and cannot be renewed.",
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation
            });
        }

        //3:  The subscription's WillRenew field must not be false.
        if(subscriptionToUpdate.WillRenew == false)
        {
            errors.Add(new ServiceError()
            {
                Severity = ErrorSeverity.Error,
                Message = "The subscription is not set to renew.",
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation
            });
        }

        return errors;
    }
}
