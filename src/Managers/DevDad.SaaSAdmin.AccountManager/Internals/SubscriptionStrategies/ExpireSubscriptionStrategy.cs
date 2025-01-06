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

sealed class ExpireSubscriptionStrategy : ChangeStrategyBase
{
    public override string ActivityKind => SubscriptionChangeKinds.ActivityKind_Expire;

    /* public override ChangeStrategyResponse ApplyChange(ChangeStrategyRequest request)
    {
        CustomerSubscription? transformedSubscription = null;
        ChangeStrategyResponse response = new ChangeStrategyResponse(request, transformedSubscription);
        response.ChangeCompleted = false;
        string changeStep = "Validation";
        string executionSite = $"{nameof(ExpireSubscriptionStrategy)}.{nameof(ApplyChange)}{changeStep}";

        var requestErrors = ValidateRequest(request);
        response.AddErrors(requestErrors, executionSite);
        if(response.ShouldHaltProcess)
        {
            return response;
        }

        // Rather than a reference assignment, let's do a deep copy of the current sub.
        transformedSubscription = TransformSubscription(_subscriptionToUpdate!, _subscriptionTemplate!, _changeDetail!);

        response.Payload = transformedSubscription;
        response.ChangeCompleted = true;
        return response;
    } */

    protected override IEnumerable<ServiceError> ValidateSubscriptionForChange(CustomerSubscription? subscription)
    {
        List<ServiceError> errors = new();

        if(subscription == null)
        {
            return errors;
        }

        // Cannot Expire Free Subscriptions.
        if(subscription.SKU == SubscriptionIdentifiers.SKUS_TDMF_FREE)
        {
            errors.Add(new ServiceError
            {
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
                Message = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForSubscriptionType,
                Severity = ErrorSeverity.Error
            });
            return errors;
        }

        // Check current Status for validity.
        bool actionIsValidForStatus = SubscriptionStateTransitions
            .CanPerformActivityForStatus
            (
                _changeDetail!.ActionName, 
                _subscriptionToUpdate!.CurrentStatus
            );

        if(actionIsValidForStatus == false)
        {
            errors.Add(new ServiceError()
            {
                Severity = ErrorSeverity.Error,
                Message = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForStatus,
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation
            });
        }        


        return errors;
    }

    protected override CustomerSubscription TransformSubscription(
            CustomerSubscription subscription, 
            SubscriptionTemplateResource template, 
            SubscriptionActionDetail changeDetail)
    {
        CustomerSubscription transformedSubscription = DomainObjectMapper
            .MapEntities<CustomerSubscription, CustomerSubscription>(_subscriptionToUpdate!);

        // Now, we set the properties on TransformedSubscription in accordance with the change.
        // 1:  Set WillRenew to False.
        transformedSubscription.WillRenew = false;
        // 2:  Set Status to Expired.
        transformedSubscription.CurrentStatus = SubscriptionChangeKinds.ActivityStatusResult[_changeDetail!.ActionName];
        // 3:  Add a new Activity to the History.
        SubscriptionActivity activityLogItem = new()
        {
            ActivityKind = _changeDetail.ActionName,
            ActivityDateUTC = DateTime.UtcNow,
            Comment = $"Action Triggered by {_changeDetail.RequestSource}.  Vendor: {_changeDetail.VendorName}."
        };
        transformedSubscription.History.Add(activityLogItem);

        return transformedSubscription;
    }
}
