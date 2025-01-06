using System;
using System.Collections.Generic;
using System.Linq;
using DevDad.SaaSAdmin.iFX;
using Microsoft.Extensions.Logging;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

internal class ChangeStrategyFactory
{
    private Dictionary<string, ChangeStrategyBase> _changeStrategies = new Dictionary<string, ChangeStrategyBase>();
    private ILogger? _logger;
    public ChangeStrategyFactory(ILogger? logger)
    {
        _logger = logger;
        _changeStrategies.Add(SubscriptionChangeKinds.ActivityKind_Create, new CreateSubscriptionStrategy());
        _changeStrategies.Add(SubscriptionChangeKinds.ActivityKind_Renew, new RenewSubscriptionStategy());
        _changeStrategies.Add(SubscriptionChangeKinds.ActivityKind_Expire, new ExpireSubscriptionStrategy());
        _changeStrategies.Add(SubscriptionChangeKinds.ActivityKind_Cancel, new CancelSubscriptionStrategy());
    }

    /// <summary>
    /// Validates that the requested activity is a known activty, and returns the strategy
    /// for it, if it exists.
    /// </summary>
    /// <param name="activityKind"></param>
    /// <returns></returns>
    public ChangeStrategyBase? GetChangeStrategy(string activityKind)
    {

        if(SubscriptionChangeKinds.AllowedValues.Contains(activityKind) == false)
        {
            string logMessage = $"The requested Subscription Action {activityKind} is not valid.";
            _logger?.LogError(logMessage);
            return null;
        }

        if(_changeStrategies.TryGetValue(activityKind, out ChangeStrategyBase? strategy) == true)
        {
            return strategy;
        }
        else
        {
            string logMessage = $"The Change Type {activityKind} was requested, but is not yet supported.";
            _logger?.LogError(logMessage);
            return null;
        }
    }
}
