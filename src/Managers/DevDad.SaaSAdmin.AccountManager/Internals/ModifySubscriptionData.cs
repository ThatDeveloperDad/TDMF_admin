using System;
using DevDad.SaaSAdmin.AccountManager.Contracts;

namespace DevDad.SaaSAdmin.AccountManager.Internals;

internal class ModifySubscriptionData
{

    public ModifySubscriptionData(CustomerProfile customer, SubscriptionActionDetail changeDetail)
    {
        CustomerProfile = customer;
        ChangeDetail = changeDetail;
    }
    
    public CustomerProfile CustomerProfile { get; set; }

    public SubscriptionActionDetail ChangeDetail { get; set; }
}
