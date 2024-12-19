using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.iFX;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;

namespace DevDad.SaaSAdmin.AccountManager.Internals;

internal static class AccountManagerExtensions
{
    public static CustomerSubscription BuildNewSubscription(this SubscriptionTemplateResource template)
    {
        CustomerSubscription model = new();

        DateTime now = DateTime.UtcNow; 
        var aiGrant = template.ResourceGrants
            .FirstOrDefault(x=>x.ResourceKind == MeteredResourceKinds.NpcAiDetail);
        var storageGrant = template.ResourceGrants
            .FirstOrDefault(x=>x.ResourceKind == MeteredResourceKinds.NpcStorage);            

        model.SKU = template.SKU;
        model.StartDateUtc = now;
        model.EndDateUtc = now.AddDays(template.RenewalPeriod);
        model.WillRenew = template.RenewalPeriod != SubscriptionRenewalFrequencies.Permanent;
        model.CurrentStatus = template.DefaultStatus;
        model.Quotas.AiGeneratedNpcs.Budget = aiGrant?.InitialBudget??0;
        model.Quotas.StoredNpcs.Budget = storageGrant?.InitialBudget??0;

        return model;
    }
}
