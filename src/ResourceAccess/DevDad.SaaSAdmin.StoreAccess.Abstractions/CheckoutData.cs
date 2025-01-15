using System;

namespace DevDad.SaaSAdmin.StoreAccess.Abstractions;

public class CheckoutData
{
    public string  CustomerEntraId { get; set; } = string.Empty;

    public string ProductIdToPurchase { get; set; } = string.Empty;

    public string LocalSystemProductSku { get; set; } = string.Empty;
}
