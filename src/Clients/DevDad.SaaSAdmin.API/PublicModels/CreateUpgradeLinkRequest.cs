using System;

namespace DevDad.SaaSAdmin.API.PublicModels;

/// <summary>
/// Submitted tot he AdminAPI to create a Checkout session at
/// the LemonSqueezy API.  Upon creating the Checkout session,
/// the API will return the URL to the particular Checkout page.
/// </summary>
public class CreateUpgradeLinkRequest
{
    /// <summary>
    /// The customer's Id within the local system.
    /// (Corresponds with their Entra ID)
    /// </summary>
    public string LocalCustomerId { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the prodiuct variant selected, as listed in the LS Store.
    /// </summary>
    public string TargetSubscriptionSku { get; set; } = string.Empty;

    /// <summary>
    /// If we have this data, we'll include it in the Checkout request.
    /// </summary>
    public string EmailAddress { get; set; } = string.Empty;

}
