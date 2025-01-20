using System;
using System.Text.Json;

namespace DevDad.SaaSAdmin.StoreAccess.LsApi.LemonSqueezyModels;

internal class LsReqBody<T> where T : ILsReqData
{
    public string type { get; set; } = string.Empty;

    public T attributes { get; set; }

    public LsRelationships relationships { get; set; }

    public LsReqBody(string requestType, T requestData, LsRelationships relationshipNodes)
    {
        type = requestType;
        attributes = requestData;
        relationships = relationshipNodes;
    }

    public string ToJson()
    {
        //TODO:  We're going to need to grind on this bit to get it right.
        string bodyJson = JsonSerializer.Serialize(this);
        string outerJson = $"{{ \"data\":  {bodyJson} }}";
        return outerJson;
    }
}
