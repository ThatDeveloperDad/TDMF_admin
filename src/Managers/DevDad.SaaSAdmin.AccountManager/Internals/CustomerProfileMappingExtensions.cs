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

internal static class CustomerProfileMappingExtensions
{
    
    public static CustomerProfile ApplyAccountFrom(
        this CustomerProfile? profile,
        UserAccountResource accountResource)
    {
        if(profile == null)
        {
            profile = new();
        }

        var subResource = accountResource.Subscription;

        profile.Subscription?.ImportHistory(subResource?.History);
        profile.SubscriptionStatus = subResource?.CurrentStatus??"None";

        return profile;
    }

    public static UserAccountResource ToResourceModel(this CustomerProfile profile)
    {
        UserAccountResource resource = new();

        resource.UserId = profile.UserId;
        resource.DisplayName = profile.DisplayName;
        resource.Subscription = profile.Subscription?.ToResourceModel()??null;
        resource.SubscriptionStatus = profile.SubscriptionStatus;
        resource.ExternalIds = profile.ExternalIds
            .Select(x=> x.ToResourceModel())
            .ToList();

        return resource;
    }

    private static UserIdResource ToResourceModel(this ExternalId externalId)
    {
        UserIdResource resource = new();

        resource.VendorName = externalId.Vendor;
        resource.UserIdAtVendor = externalId.IdAtVendor;

        return resource;
    }

    private static AccountSubscriptionResource ToResourceModel(this CustomerSubscription subscription)
    {
        AccountSubscriptionResource resource = new();

        resource.UserId = subscription.UserId;
        resource.SKU = subscription.SKU;
        resource.StartDateUtc = subscription.StartDateUtc;
        resource.EndDateUtc = subscription.EndDateUtc;
        resource.WillRenew = subscription.WillRenew;
        resource.CurrentStatus = subscription.CurrentStatus;
        resource.Quotas = subscription.Quotas.ToResourceModel();
        resource.History = subscription.History.Select(x=>x.ToResourceModel()).ToList();

        return resource;
    }

    private static UserQuotaResource ToResourceModel(this UserQuotas quotas)
    {
        UserQuotaResource resource = new();

        resource.UserId = quotas.UserId;
        resource.Storage.Budget = quotas.StoredNpcs.Budget;
        resource.Storage.Consumption = quotas.StoredNpcs.Consumption;
        resource.AiGenerations.Budget = quotas.AiGeneratedNpcs.Budget;
        resource.AiGenerations.Consumption = quotas.AiGeneratedNpcs.Consumption;

        return resource;
    }

    private static SubscriptionActivityResource ToResourceModel(this SubscriptionActivity activity)
    {
        SubscriptionActivityResource resource = new();

        resource.ActivityKind = activity.ActivityKind;
        resource.ActivityDateUTC = activity.ActivityDateUTC;

        return resource;
    }

    public static CustomerSubscription ToNewSubscription(this SubscriptionTemplateResource template)
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
        model.WillRenew = false;
        model.CurrentStatus = template.DefaultStatus;
        model.Quotas.AiGeneratedNpcs.Budget = aiGrant?.InitialBudget??0;
        model.Quotas.StoredNpcs.Budget = storageGrant?.InitialBudget??0;

        return model;
    }

    private static void ImportHistory(
        this CustomerSubscription model, 
        List<SubscriptionActivityResource>? resourceHistory)
    {
        if(resourceHistory == null)
        {
            return;
        }

        foreach(var activityResource in resourceHistory)
        {
            model.History.Add(new SubscriptionActivity()
            {
                ActivityKind = activityResource.ActivityKind,
                ActivityDateUTC = activityResource.ActivityDateUTC
            });
        }
    }
}
