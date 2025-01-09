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
    public string LocalCustomerId { get; set; }

    public int? LSCustomerId { get; set; }

    /// <summary>
    /// The ID of the subscirption product as listed in the LS Store.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The ID of the prodiuct variant selected, as listed in the LS Store.
    /// </summary>
    public int VariantId { get; set; }

    public string EmailAddress { get; set; }

    /// <summary>
    /// The ID of the LS Store
    /// </summary>
    public int StoreId { get; set; }
}
