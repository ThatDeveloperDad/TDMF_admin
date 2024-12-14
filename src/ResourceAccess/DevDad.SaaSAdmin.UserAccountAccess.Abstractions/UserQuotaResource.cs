using System;
using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

[DomainEntity(
    entityName: Entities.UserQuotas.EntityName,
    declaringArchetype: ComponentArchetype.ResourceAccess)]
public class UserQuotaResource:IdiomaticType
{
    public UserQuotaResource()
    {
        Storage = new AppResourceQuota()
        {
            MeteredResource = MeteredResourceKinds.NpcStorage,
            QuotaId = 1,
        };
        AiGenerations = new AppResourceQuota()
        {
            MeteredResource = MeteredResourceKinds.NpcAiDetail,
            QuotaId = 2,
        };
        
    }
    public AppResourceQuota? GetQuota(int quotaId)
    {
        if(quotaId == 1)
        {
            return Storage;
        }
        if(quotaId == 2)
        {
            return AiGenerations;
        }
        return null;
    }

    [EntityAttribute(Entities.UserQuotas.Attributes.UserId)]
    public string UserId { get; set; } = string.Empty;
    
    [EntityAttribute(Entities.UserQuotas.Attributes.Storage,
        isCollection:false,
        valueEntityName:Entities.UserResourceQuota.EntityName)]
    public AppResourceQuota Storage {get;set;}
    
    [EntityAttribute(Entities.UserQuotas.Attributes.AiGenerations,
        isCollection:false,
        valueEntityName:Entities.UserResourceQuota.EntityName)]
    public AppResourceQuota AiGenerations{get;set;}
}
