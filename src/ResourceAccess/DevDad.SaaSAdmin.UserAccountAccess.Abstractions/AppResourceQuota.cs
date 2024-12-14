using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.DomainUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;
using Entities = DevDad.SaaSAdmin.iFX.DomainConstants.SaaSDomain;


namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;


[DomainEntity(
    entityName: Entities.UserResourceQuota.EntityName,
    declaringArchetype: ComponentArchetype.ResourceAccess)]
public class AppResourceQuota:IdiomaticType
{

    public int QuotaId { get; set; }
    /// <summary>
    /// Identifies the general kind of resource that has this quota
    /// 
    /// i.e.:  Storage or AiTokens
    /// </summary>
    [EntityAttribute(Entities.UserResourceQuota.Attributes.ResourceKind)]
    public MeteredResourceKinds MeteredResource { get; set; }

    /// <summary>
    /// Provides the specific name of the Resource that has the quota
    /// 
    /// i.e.:  NpcStorage, NpcAiGeneration
    /// </summary>
    public string ResourceName => MeteredResource.ToString();

    /// <summary>
    /// Describes the amount of the Specific Resource that the 
    /// user can consume.
    /// </summary>
    [EntityAttribute(Entities.UserResourceQuota.Attributes.GrantedBudget)]
    public int Budget { get; set; }

    /// <summary>
    /// Describes the amount of the Specific Resource that the user
    /// HAS consumed.
    /// </summary>
    [EntityAttribute(Entities.UserResourceQuota.Attributes.ConsumedBudget)]
    public int Consumption { get; set; }
}
