using System;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

internal class ChangeStrategyRequest : OperationRequest<CustomerSubscription>
{
    private const string _operationName = "ApplySubscriptionChange";
    public override string OperationName => _operationName;

// It's OK to suppress this issue here, because this constructor is Private and called only from
// the complete form of the constructor that DOES initialize the required properties.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ChangeStrategyRequest
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        (
            OperationRequest parent, 
            CustomerSubscription? payload = null
        ) : base(parent, payload)
    {
    }

    public ChangeStrategyRequest
        (
            OperationRequest parentRequest,
            CustomerSubscription? subscriptionToUpdate,
            SubscriptionActionDetail actionDetail,
            SubscriptionTemplateResource subscriptionTemplate
        ) :this(parentRequest, subscriptionToUpdate)
    {
        SubscriptionToUpdate = subscriptionToUpdate;
        ChangeDetails = actionDetail;
        TargetTemplate = subscriptionTemplate;
    }
        
    public CustomerSubscription? SubscriptionToUpdate 
    { 
        get 
        {
            return Payload;
        }
        set
        {
            Payload = value;
        } 
    }

    public SubscriptionActionDetail ChangeDetails { get; set; }

    public SubscriptionTemplateResource TargetTemplate { get; set; }
}
