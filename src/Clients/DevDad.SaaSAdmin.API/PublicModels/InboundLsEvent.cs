using System;
using System.Text.Json.Serialization;

namespace DevDad.SaaSAdmin.API.PublicModels;

public class InboundLsEvent
{
    public InboundLsEvent()
    {
        EventJson = string.Empty;

    }

    public string EventJson { get; set; }


}
