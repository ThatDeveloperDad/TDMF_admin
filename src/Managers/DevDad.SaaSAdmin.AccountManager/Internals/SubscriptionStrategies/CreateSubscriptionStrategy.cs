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
    // I normally wouldn't put the objects that I'm working on here at the class-global scope,
    // But we're dealing with one instance of a CreateSubscriptionStrategy for each time we
    // perform this operation, and the contextual validation methods will need access to these objects.
    private CustomerSubscription? _subscriptionToUpdate;
    private SubscriptionTemplateResource? _subscriptionTemplate; 
    private SubscriptionActionDetail? _changeDetail;

    public CreateSubscriptionStrategy()
    {
    }

    public override string ActivityKind => SubscriptionChangeKinds.ActivityKind_Create;

    public override ChangeStrategyResponse ApplyChange(ChangeStrategyRequest request)
    {
        CustomerSubscription? newSubscription = null;
        ChangeStrategyResponse response = new ChangeStrategyResponse(request, newSubscription);
        response.ChangeCompleted = false;
        string changeStep = "Validation";
        string executionSite = $"{nameof(CreateSubscriptionStrategy)}.{nameof(ApplyChange)}{changeStep}";

        // Validate the request for the Create action.
        var validationErrors = ValidateChangeRequest(request);
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
        return response;
    }

    /// <summary>
    /// Encapsulates all the Validation passes into a single method.
    /// 
    /// Really just did this for readability's sake.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private IEnumerable<ServiceError> ValidateChangeRequest(ChangeStrategyRequest request)
    {
        List<ServiceError> errors = new();
        
        var requestErrors = new ChangeRequestValidator().Validate(request);
        if(requestErrors.Any(e=> e.Severity == ErrorSeverity.Error))
        {
            return errors;
        }
        
        // At this point, we can assume that the request is valid, and we have the "parts"
        // that we need.  We still need to make sure that those parts are correct for the
        // requested transformation.  The Part validations all possibly require some
        // access to the other parts, so we'll assign them to class-level variables now.
        _subscriptionToUpdate = request.SubscriptionToUpdate;
        _changeDetail = request.ChangeDetails;
        _subscriptionTemplate = request.TargetTemplate;

        // Make sure the requested ChangeDetail is well formed.
        var changeDetailErrors = new SubscriptionActionValidator().Validate
            (request.ChangeDetails);
        errors.AddRange(changeDetailErrors);

        // Make sure the existing subscription data is appropriate for the Create action.
        var subscriptionErrors = new SubscriptionValidator().Validate
            (request.SubscriptionToUpdate,
            ValidateCurrentSubscriptionForCreateAction);
        errors.AddRange(subscriptionErrors);

        return errors;
    }

    /// <summary>
    /// Validates that the current subscirption is in an appropriate state
    /// for the requested action, and provided template.
    /// </summary>
    /// <param name="subscriptionToUpdate">The customer's CURRENT subcription State</param>
    /// <param name="actionDetail">The details for the Action to perform on the Subscription</param>
    /// <param name="subscriptionTemplate">The template for the subscription type described in the actionDetail</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private IEnumerable<ServiceError> ValidateCurrentSubscriptionForCreateAction(CustomerSubscription? subscriptionToUpdate)
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
        // if the currentSub is a FreeTier, let's set the currentStatus to string.Empty;
        // Within the CreateSubscription context, we can treat a Free Sub the same as No Sub.
        if(currentSubIsFreeTier)
        {
            currentStatus = string.Empty;
        }

        // Validate that the requested Activity is appropriate for the currentStatus.
        // The ValidActivities / Status are defined in the SubscriptionStateTransitions
        // helper class, in DevDad.SaaSAdmin.iFX
        if(SubscriptionStateTransitions
            .ValidStatusActions[currentStatus]
            .Contains(activityKind) == false)
        {
            errors.Add(new ServiceError
            {
                Message = $"The requested activity '{activityKind}' is not valid for the current subscription status '{currentStatus}'.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation
            });
        }

        return errors;
    }

}
