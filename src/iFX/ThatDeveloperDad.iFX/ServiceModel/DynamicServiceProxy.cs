using System;
using System.Collections.Concurrent;
using System.Reflection;
using ThatDeveloperDad.iFX.Behaviors;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace ThatDeveloperDad.iFX.ServiceModel;


public class DynamicServiceProxy<T, TService>
    : DispatchProxy
    where T: ISystemComponent
    where TService: T
{
    private TService? _serviceInstance;
    private ConcurrentDictionary<string, List<IOperationBehavior>> _methodBehaviors
        = new();
    private List<IOperationBehavior> _globalBehaviors = new();

    internal void AddBehavior(IOperationBehavior behavior, string? methodName = null)
    {
        if(methodName == null)
        {
            _globalBehaviors.Add(behavior);
            return;
        }

        if(!_methodBehaviors.ContainsKey(methodName))
        {
            _methodBehaviors[methodName] = new List<IOperationBehavior>();
        }

        _methodBehaviors[methodName].Add(behavior);
    }

    public T CreateProxy(TService service)
    {
        object proxy = Create<T, DynamicServiceProxy<T, T>>();
        ((DynamicServiceProxy<T, T>)proxy).SetServiceInstance(service);

        foreach(var behavior in _globalBehaviors)
        {
            ((DynamicServiceProxy<T, T>)proxy).AddBehavior(behavior);
        }

        foreach(var key in _methodBehaviors.Keys)
        {
            foreach(var behavior in _methodBehaviors[key])
            {
                ((DynamicServiceProxy<T, T>)proxy).AddBehavior(behavior, key);
            }
        }

        return (T)proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if(targetMethod == null)
        {
            throw new InvalidOperationException("You can't invoke a method without telling me which method to invoke.");
        }

        MethodContext context = new()
        {
            MethodName = targetMethod!.Name,
            Parameters = args
        };

        // Execute the Service-Global Pre-Method behaviors.
        foreach(IOperationBehavior behavior in _globalBehaviors)
        {
            behavior.OnMethodEntry(context);
        }

        // Execute the Method-Specific Pre-Method behaviors.
        if(_methodBehaviors.ContainsKey(targetMethod.Name))
        {
            foreach(IOperationBehavior behavior in _methodBehaviors[targetMethod.Name])
            {
                behavior.OnMethodEntry(context);
            }
        }

        var result = targetMethod.Invoke(_serviceInstance, args);

        if(result is Task taskResult)
        {
            taskResult.GetAwaiter().GetResult();
            context.ReturnValue = taskResult.GetType().GetProperty("Result")?.GetValue(taskResult);
        }
        else
        {
            context.ReturnValue = result;
        }

        // Execute the Method-Specific Post-Method behaviors.
        if(_methodBehaviors.ContainsKey(targetMethod.Name))
        {
            foreach(IOperationBehavior behavior in _methodBehaviors[targetMethod.Name])
            {
                behavior.OnMethodExit(context);
            }
        }

        // Execute the Service-Global Post-Method behaviors.
        foreach(IOperationBehavior behavior in _globalBehaviors)
        {
            behavior.OnMethodExit(context);
        }

        return result;
    }

    protected void SetServiceInstance(TService service)
    {
        _serviceInstance = service;
    }
}

