using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace DevDad.SaaSAdmin.API.PublicModels;

public class InboundLsEvent
{
    public InboundLsEvent()
    {
        EventJson = string.Empty;

    }

    public string EventJson { get; set; }

    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// A unique identifier for a webhook event instance.
    /// Use this to prevent processing the same event multiple times.
    /// </summary>
    public string EventId { get; set; } = string.Empty;

    public int LS_CustomerId { get; set; }

    public int LS_ProductId { get; set; }

    public bool TestMode { get; set; }

    

}
