using System;
using System.Collections.Generic;
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
        _changeStrategies.Add(SubscriptionChangeKinds.ActivityKind_Create, new CreateSubscriptionStrategy());;
    }

    public ChangeStrategyBase? GetChangeStrategy(string activityKind)
    {
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
