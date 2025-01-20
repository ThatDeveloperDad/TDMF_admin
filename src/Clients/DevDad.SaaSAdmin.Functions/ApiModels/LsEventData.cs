using System;

namespace DevDad.SaaSAdmin.Functions.ApiModels;

internal class LsEventData
{
    public LsEventData()
    {
        EventJson = string.Empty;
        RequestId = string.Empty;
    }

    public string EventJson { get; set; }

    public string RequestId { get; set; }
}
