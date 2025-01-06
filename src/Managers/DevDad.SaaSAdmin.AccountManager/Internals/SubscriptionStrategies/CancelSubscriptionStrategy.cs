using System;
using System.Collections;
using System.Collections.Generic;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

sealed class CancelSubscriptionStrategy : ChangeStrategyBase
{
    public override string ActivityKind => SubscriptionChangeKinds.ActivityKind_Cancel;

    protected override IEnumerable<ServiceError> ValidateSubscriptionForChange(CustomerSubscription? subscription)
    {
        throw new NotImplementedException();
    }

    protected override CustomerSubscription TransformSubscription(
        CustomerSubscription subscription, 
        SubscriptionTemplateResource template, 
        SubscriptionActionDetail changeDetail)
    {
        throw new NotImplementedException();
    }
}
