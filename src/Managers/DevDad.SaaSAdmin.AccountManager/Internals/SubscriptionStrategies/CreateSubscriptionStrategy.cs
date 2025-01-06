using System;
using System.Collections.Generic;
using System.Linq;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

sealed class CreateSubscriptionStrategy : ChangeStrategyBase
{

    public CreateSubscriptionStrategy()
    {
    }

    public override string ActivityKind => SubscriptionChangeKinds.ActivityKind_Create;

    /* public override ChangeStrategyResponse ApplyChange(ChangeStrategyRequest request)
    {
        CustomerSubscription? newSubscription = null;
        ChangeStrategyResponse response = new ChangeStrategyResponse(request, newSubscription);
        response.ChangeCompleted = false;
        string changeStep = "Validation";
        string executionSite = $"{nameof(CreateSubscriptionStrategy)}.{nameof(ApplyChange)}{changeStep}";

        // Validate the request for the Create action.
        var validationErrors = ValidateRequest(request);
        response.AddErrors(validationErrors, executionSite);
        if(response.ShouldHaltProcess)
        {
            return response;
        }

        // With Request & Data validation complete, we can proceed with the transformation activity.
        // Create Subscription is fairly simple.  We just build a new instance from the template.
        newSubscription = _subscriptionTemplate!.BuildNewSubscription();
        
        // Now, we'll set the properties that don't (can't) come from the Template.
        newSubscription.CurrentStatus = SubscriptionChangeKinds.ActivityStatusResult[_changeDetail!.ActionName];
        newSubscription.SetUserId(_changeDetail!.CustomerProfileId);
        SubscriptionActivity activityLogItem = new()
        {
            ActivityKind = _changeDetail.ActionName,
            ActivityDateUTC = DateTime.UtcNow,
            Comment = $"Action Triggered by {_changeDetail.RequestSource}.  Vendor: {_changeDetail.VendorName}."
        };
        newSubscription.History.Add(activityLogItem);
        
        response.Payload = newSubscription;
        response.ChangeCompleted = true;
        return response;
    } */


    /// <summary>
    /// Validates that the current subscirption is in an appropriate state
    /// for the requested action, and provided template.
    /// </summary>
    /// <param name="subscriptionToUpdate">The customer's CURRENT subcription State</param>
    /// <param name="actionDetail">The details for the Action to perform on the Subscription</param>
    /// <param name="subscriptionTemplate">The template for the subscription type described in the actionDetail</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override IEnumerable<ServiceError> ValidateSubscriptionForChange(CustomerSubscription? subscriptionToUpdate)
    {
        // What do we care about the "current" subscription, when we're creating a NEW subscription?
        List<ServiceError> errors = new();

        // If the current Subscription is NULL, 
        // there's not much more to do, and we can proceed.
        // Since THIS transformation CREATES a new subscription,
        // a null value here is fine.
        if(subscriptionToUpdate == null)
        {
            return errors;
        }

        string activityKind = _changeDetail?.ActionName!;
        string currentStatus = subscriptionToUpdate.CurrentStatus;
        bool currentSubIsFreeTier = subscriptionToUpdate.SKU == SubscriptionIdentifiers.SKUS_TDMF_FREE;
        string requestedNewSku = _changeDetail!.SubscriptionSku;
        
        // if the currentSub is a FreeTier, let's set the currentStatus to string.Empty;
        // Within the CreateSubscription context, we can treat a Free Sub the same as No Sub.
        if(currentSubIsFreeTier)
        {
            currentStatus = string.Empty;
        }

        //Cannot add a new subscription of the same sku to an existing subscription.
        // If currentStatus is Cancelled, we're effectively replacing it with a new sub.
        if(_subscriptionToUpdate!.SKU == requestedNewSku
          && currentStatus != SubscriptionStatuses.Cancelled)
        {
            errors.Add(new ServiceError
            {
                Message = AccountManagerConstants.ModifySubscriptionErrors.Validation_AddNewSameSku,
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation
            });
        }

        // Cannot Add a Free Subscirption to an active Paid Subscription.
        if(currentSubIsFreeTier == false
          && currentStatus == SubscriptionStatuses.Active
          && requestedNewSku == SubscriptionIdentifiers.SKUS_TDMF_FREE)
        {
            errors.Add(new ServiceError
            {
                Message = AccountManagerConstants.ModifySubscriptionErrors.Validation_AddFreeToActivePaid,
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation
            });
        }

        // Validate that the requested Activity is appropriate for the currentStatus.
        // The ValidActivities / Status are defined in the SubscriptionStateTransitions
        // helper class, in DevDad.SaaSAdmin.iFX
        bool actionIsValidForStatus = 
            SubscriptionStateTransitions.CanPerformActivityForStatus(activityKind, currentStatus);
        
        if(actionIsValidForStatus == false)
        {
            errors.Add(new ServiceError
            {
                Message = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForStatus,
                Severity = ErrorSeverity.Error,
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
        CustomerSubscription newSubscription = _subscriptionTemplate!.BuildNewSubscription();
        
        // Now, we'll set the properties that don't (can't) come from the Template.
        newSubscription.CurrentStatus = SubscriptionChangeKinds.ActivityStatusResult[_changeDetail!.ActionName];
        newSubscription.SetUserId(_changeDetail!.CustomerProfileId);
        SubscriptionActivity activityLogItem = new()
        {
            ActivityKind = _changeDetail.ActionName,
            ActivityDateUTC = DateTime.UtcNow,
            Comment = $"Action Triggered by {_changeDetail.RequestSource}.  Vendor: {_changeDetail.VendorName}."
        };
        newSubscription.History.Add(activityLogItem);

        return newSubscription;
    }

}
