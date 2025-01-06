using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

[DomainEntity(
    entityName: Entities.UserQuotas.EntityName,
    declaringArchetype: ComponentArchetype.Manager)]
public class UserQuotas:IdiomaticType
{
    public UserQuotas()
    {
        UserId = string.Empty;
        StoredNpcs = new UserResourceQuota()
        {
            MeteredResource = MeteredResourceKinds.NpcStorage
        };
        
        AiGeneratedNpcs = new UserResourceQuota()
        {
            MeteredResource = MeteredResourceKinds.NpcAiDetail
        };
    }

    [EntityAttribute(Entities.UserQuotas.Attributes.UserId)]
    public string UserId { get; set; }

    [EntityAttribute(Entities.UserQuotas.Attributes.Storage,
        isCollection:false,
        valueEntityName:Entities.UserResourceQuota.EntityName)]
    public UserResourceQuota StoredNpcs {get;set;}

    [EntityAttribute(Entities.UserQuotas.Attributes.AiGenerations,
        isCollection:false,
        valueEntityName:Entities.UserResourceQuota.EntityName)]
    public UserResourceQuota AiGeneratedNpcs{get;set;}

}
