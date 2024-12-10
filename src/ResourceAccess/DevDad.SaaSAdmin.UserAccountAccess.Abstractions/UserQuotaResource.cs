using System;
using DevDad.SaaSAdmin.iFX;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

public class UserQuotaResource
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

    public string UserId { get; set; } = string.Empty;
    public AppResourceQuota Storage {get;set;}
    public AppResourceQuota AiGenerations{get;set;}
}
