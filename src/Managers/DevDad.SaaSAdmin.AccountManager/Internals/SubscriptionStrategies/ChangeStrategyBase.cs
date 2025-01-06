using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

internal abstract class ChangeStrategyBase
{
    public abstract string ActivityKind { get; }

    protected CustomerSubscription? _subscriptionToUpdate;
    protected SubscriptionTemplateResource? _subscriptionTemplate;
    protected SubscriptionActionDetail? _changeDetail;

    public ChangeStrategyResponse ApplyChange(ChangeStrategyRequest request)
    {
        CustomerSubscription? transformedSubscription = null;
        ChangeStrategyResponse response = new ChangeStrategyResponse(request, transformedSubscription);
        response.ChangeCompleted = false;
        string changeStep = "Validation";
        string executionSite = $"{this.GetType().Name}.{nameof(ApplyChange)}{changeStep}";

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
    }

    protected IEnumerable<ServiceError> ValidateRequest(ChangeStrategyRequest request)
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
            (_subscriptionToUpdate, ValidateSubscriptionForChange);
            errors.AddRange(subscriptionErrors);

        return errors;
    }

    /// <summary>
    /// Performs the checks to ensure that the incoming subscription is in an
    /// appropriate state for the change defined by children of this class.
    /// </summary>
    /// <param name="subscription"></param>
    /// <returns></returns>
    protected abstract IEnumerable<ServiceError> ValidateSubscriptionForChange(CustomerSubscription? subscription);

    protected abstract CustomerSubscription TransformSubscription(
        CustomerSubscription subscription, 
        SubscriptionTemplateResource template, 
        SubscriptionActionDetail changeDetail);
    

}
