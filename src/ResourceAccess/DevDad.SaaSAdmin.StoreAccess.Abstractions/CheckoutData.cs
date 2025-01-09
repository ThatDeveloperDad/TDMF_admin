using System;

namespace DevDad.SaaSAdmin.StoreAccess.Abstractions;

public class CheckoutData
{
    public string  CustomerEntraId { get; set; } = string.Empty;

    public int ProductIdToPurchase { get; set; }
}
