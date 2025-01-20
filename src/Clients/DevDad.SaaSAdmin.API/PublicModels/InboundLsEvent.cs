using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace DevDad.SaaSAdmin.API.PublicModels;

public class InboundLsEvent
{
    public InboundLsEvent()
    {
        EventJson = string.Empty;
        RequestId = string.Empty;
    }

    public string EventJson { get; set; }

    public string RequestId { get; set; }

}
