using System;
using DevDad.SaaSAdmin.iFX;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

public class AppResourceQuota
{

    

    /// <summary>
    /// Identifies the general kind of resource that has this quota
    /// 
    /// i.e.:  Storage or AiTokens
    /// </summary>
    public MeteredResourceKinds MeteredResource { get; internal set; }

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
    public int Budget { get; internal set; }

    /// <summary>
    /// Describes the amount of the Specific Resource that the user
    /// HAS consumed.
    /// </summary>
    public int Consumption { get; internal set; }
}
