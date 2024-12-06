using System;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using DevDad.SaaSAdmin.UserAccountAccess.AzureTableProvider.StorageModels;
using ThatDeveloperDad.iFX.Serialization;

namespace DevDad.SaaSAdmin.UserAccountAccess.AzureTableProvider.ObjectMapping;

public static class TableToResourceMappers
{
    public static UserAccountResource ToResource(this UserEntity entity)
    {
        UserAccountResource resource =  new UserAccountResource
        {
            UserId = entity.UserId,
            DisplayName = entity.DisplayName,
            SubscriptionStatus = entity.SubscriptionStatus,
            ExternalIds = entity.IdsJson.ToInstance<List<UserIdResource>>()
                ?? new List<UserIdResource>(),
            Subscription = entity.CurrentSubscriptionJson.ToInstance<AccountSubscriptionResource>(),
            // We'll Add Subscription Activity later once it's defined.
            //SubscriptionActivityJson = entity.SubscriptionActivityJson
        };

        return resource;
    }
}
