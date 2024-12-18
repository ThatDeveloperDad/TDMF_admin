using System;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

internal abstract class ChangeStrategyBase
{
    public abstract string ActivityKind { get; }
    
    public abstract CustomerSubscription ApplyChange(
        CustomerSubscription subscriptionToUpdate, 
        SubscriptionActionDetail actionDetail, 
        SubscriptionTemplateResource subscriptionTemplate);

    // Each implementation of this method will:
    // 1. Validate the current subscription state against the requested Action.
    //      - Before we can apply a change, we need to make sure that the requested change 
    //        Makes sense for that susbcription's state.  (Can't renew a "Free" subscription.)
    // 2. EITHER copy the existing subscription and perform the changes that are necessary
    //      for the provided action,
    //    OR create a new subscription based on the provided template.
    // 3. Return the updated subscription.
}
