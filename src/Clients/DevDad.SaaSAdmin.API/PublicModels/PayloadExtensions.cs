using System;
using DevDad.SaaSAdmin.AccountManager.Contracts;

namespace DevDad.SaaSAdmin.API.PublicModels;

internal static class PayloadExtensions
{
    public static ManageSubscriptionRequest ToManagerModel(this InboundLsEvent inbound)
    {
        ManageSubscriptionRequest req = new("ProcessStoreEvent");
        SubscriptionActionDetail eventDetail = new ();
        
        
        req.Payload = eventDetail;
        return req;
    }
}
