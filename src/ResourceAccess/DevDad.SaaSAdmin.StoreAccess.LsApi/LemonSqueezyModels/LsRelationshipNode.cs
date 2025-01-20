using System;

namespace DevDad.SaaSAdmin.StoreAccess.LsApi.LemonSqueezyModels;

internal class LsRelationships
{
    public LsRelationNode? store { get; set; }
    public LsRelationNode? variant { get; set; }
}

internal class LsRelationNode
{
    public LsRelationNode(LsNodeData nodeData)
    {
        data = nodeData;
    }
    public LsNodeData data { get; set; }
}

internal class LsNodeData
{
    public string type { get; set; } = string.Empty;
    public string id { get; set; } =string.Empty;
}