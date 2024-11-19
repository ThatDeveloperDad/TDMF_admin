using System;
using System.ComponentModel.Design;
using ThatDeveloperDad.iFX.Behaviors;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace ThatDeveloperDad.iFX.ServiceModel;

public static class ProxyBehaviorExtensions
{
    public static DynamicServiceProxy<T, TService> AddBehavior<T, TService>
        (
            this DynamicServiceProxy<T, TService> proxy, 
            IOperationBehavior? behavior, 
            string? methodName = null
        )
        where T: ISystemComponent
        where TService: T
    {
        if(behavior == null)
        {
            return proxy;
        }

        proxy.AddBehavior(behavior, methodName);
        return proxy;
    }

}
