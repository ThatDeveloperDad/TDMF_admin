using System;

namespace DevDad.SaaSAdmin.StoreManager.Contracts;

/// <summary>
/// Represents the data needed to start a new checkout session at LemonSqueezy.
/// </summary>
public class NewCheckoutData
{
    public string LocalUserId { get; set; } = string.Empty;

    public string RequestedSku  {get;set;} = string.Empty;
}
