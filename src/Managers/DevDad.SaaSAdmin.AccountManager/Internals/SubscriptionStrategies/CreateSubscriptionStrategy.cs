using System;
using System.Collections.Generic;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

class CreateSubscriptionStrategy : ChangeStrategyBase
{
    public CreateSubscriptionStrategy()
    {
    }

    public override string ActivityKind => SubscriptionChangeKinds.ActivityKind_Create;

    public override CustomerSubscription ApplyChange(
        CustomerSubscription subscriptionToUpdate, 
        SubscriptionActionDetail actionDetail, 
        SubscriptionTemplateResource subscriptionTemplate)
    {
        throw new NotImplementedException();
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
    private List<ServiceError> ValidateData(
        CustomerSubscription subscriptionToUpdate, 
        SubscriptionActionDetail actionDetail,
        SubscriptionTemplateResource subscriptionTemplate)
    {
        // Brainstrom the Rules we should check when creating a new subscription.
        
        throw new NotImplementedException();
    }
}
