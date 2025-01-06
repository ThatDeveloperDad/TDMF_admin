using System;
using System.Collections.Generic;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

sealed class CancelSubscriptionStrategy : ChangeStrategyBase
{
    public override string ActivityKind => SubscriptionChangeKinds.ActivityKind_Cancel;

    protected override IEnumerable<ServiceError> ValidateSubscriptionForChange(CustomerSubscription? subscription)
    {
        List<ServiceError> errors = new();

        if(subscription == null)
        {
            return errors;
        }

        // What rules must be satisfied to cancel a subscription?

        // 1:  It must be a renewable Subscription type.  ("Free" subs can't be cancelled.)
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

        // 2:  It must be in an appropriate State. (Active, Suspended)
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
        CustomerSubscription cancelledSubscription = DomainObjectMapper
            .MapEntities<CustomerSubscription, CustomerSubscription>(_subscriptionToUpdate!);

        // When we cancel a subscirption we:
        // 1:  Set the Subscription State to "Cancelled"
        cancelledSubscription.CurrentStatus = SubscriptionStatuses.Cancelled;

        // 2:  Set "Will Renew" to false.
        cancelledSubscription.WillRenew = false;

        // 3:  Set the Subscription End Date to the current date.
        cancelledSubscription.EndDateUtc = DateTime.UtcNow;

        // 4:  Set Quota Budgets equal to their consumed amounts.
        cancelledSubscription
            .Quotas
            .AiGeneratedNpcs
            .Budget = cancelledSubscription
                .Quotas
                .AiGeneratedNpcs
                .Consumption;

        cancelledSubscription
            .Quotas
            .StoredNpcs
            .Budget = cancelledSubscription
                .Quotas
                .StoredNpcs
                .Consumption;

        // 5:  Add an Activity log entry to the Subscription History.
        SubscriptionActivity activityLogItem = new()
        {
            ActivityKind = changeDetail.ActionName,
            ActivityDateUTC = DateTime.UtcNow,
            Comment = $"Action Triggered by {changeDetail.RequestSource}.  Vendor: {changeDetail.VendorName}."
        };
        cancelledSubscription.History.Add(activityLogItem);

        return cancelledSubscription;
    }
}
