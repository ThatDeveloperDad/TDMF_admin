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
        };
        
        if(resource.Subscription != null)
        {
            if(string.IsNullOrWhiteSpace(entity.SubscriptionActivityJson) == false)
            {
                var historyList = entity.SubscriptionActivityJson!
                        .ToInstance<List<SubscriptionActivityResource>>();
                resource.Subscription.History 
                    = historyList??new List<SubscriptionActivityResource>(); 
            }
        }
        
        return resource;
    }

    public static UserEntity? ToEntity(this UserAccountResource? resource)
    {
        if(resource == null)
        {
            return null;
        }

        UserEntity entity = new UserEntity
        {
            UserId = resource.UserId,
            DisplayName = resource.DisplayName,
            SubscriptionStatus = resource.SubscriptionStatus,
            IdsJson = resource.ExternalIds
                .SerializeForStorage(),
            CurrentSubscriptionJson = resource.Subscription?
                .SerializeForStorage()??string.Empty,
            // We'll Add Subscription Activity later once it's defined.
            SubscriptionActivityJson = resource.Subscription?.History?
                .SerializeForStorage()??string.Empty
        };

        return entity;
    }
}
