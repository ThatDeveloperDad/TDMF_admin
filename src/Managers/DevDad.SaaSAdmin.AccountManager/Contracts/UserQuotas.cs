using System;
using DevDad.SaaSAdmin.iFX;

namespace DevDad.SaaSAdmin.AccountManager.Contracts;

public class UserQuotas
{

    
    public UserQuotas()
    {
        UserId = string.Empty;
        StoredNpcs = new AppResourceQuota()
        {
            MeteredResource = MeteredResourceKinds.NpcStorage
        };
        
        AiGeneratedNpcs = new AppResourceQuota()
        {
            MeteredResource = MeteredResourceKinds.NpcAiDetail
        };
    }

    public string UserId { get; set; }

    public AppResourceQuota StoredNpcs {get;set;}

    public AppResourceQuota AiGeneratedNpcs{get;set;}

}
