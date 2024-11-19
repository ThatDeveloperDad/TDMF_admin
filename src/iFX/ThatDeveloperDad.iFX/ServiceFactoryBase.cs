using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.ServiceModel;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace ThatDeveloperDad.iFX;

/// <summary>
/// Provides common functionality used to obtain concrete instances
/// of declared Service Types.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="U"></typeparam>
public abstract class ServiceFactoryBase<T, U> :IServiceFactory<T>
    where T: ISystemComponent 
    where U: IServiceOptions, new()
{
    protected IEnumerable<Type> _dependencyManifest;
    protected ServiceFactoryBase()
    {
        _dependencyManifest = GetComponentDependencyList();
    }

    public abstract T CreateService(IConfiguration config,
        IServiceProvider? standardDependencies = null);

    protected V AddStandardDependencies<V>(
        V service, 
        IServiceProvider? standardDependencies = null)
    where V: ServiceBase, T
    {
        if(standardDependencies == null)
        {
            return service;
        }

        var logFactory = standardDependencies.GetService<ILoggerFactory>();
        service.AddLogger(logFactory);

        return service;
    }

        protected V AddStandardDependenciesToProxy<V>(
        V service, 
        IServiceProvider? standardDependencies = null)
    where V: DynamicServiceProxy<T, T>, T
    {
        if(standardDependencies == null)
        {
            return service;
        }

        var logFactory = standardDependencies.GetService<ILoggerFactory>();
        

        return service;
    }

    protected V CreateServiceInstance<V>(
        IConfiguration config, 
        bool useConcreteConfigName = false,
        IServiceProvider? standardDependencies = null)
    where V: ServiceBase, T, new()
    {
        V service = new();
        
        string configSectionName = useConcreteConfigName == true
            ? typeof(V).Name
            : typeof(T).Name;
        IConfigurationSection serviceConfig
            = config.GetSection(configSectionName);

        service.SetConfiguration(BuildOptions(config));

        service = BuildComponentDependencies(service, serviceConfig, standardDependencies);
        return service;
    }

    protected U BuildOptions(IConfiguration config)
    {
        U options = new();
        config.Bind(options);
        return options;
    }

    protected abstract IEnumerable<Type> GetComponentDependencyList();

    protected V BuildComponentDependencies<V>(
        V service, 
        IConfigurationSection componentConfig,
        IServiceProvider? standardDependencies = null)
    where V: ServiceBase, T
    {
        // If there are no ComponentDependencies, bail now.
        if(_dependencyManifest == null || _dependencyManifest.Any() == false)
        {
            return service;
        }

        string serviceTypeName = service.GetType().Name;

        foreach(Type dependencyType in _dependencyManifest)
        {
            var serviceDependency = CreateDependencyInstance
                (
                    dependencyType, 
                    componentConfig, 
                    serviceTypeName,
                    standardDependencies
                );

            service = (V)ApplyDependency(dependencyType, service, serviceDependency);
        }

        return service;
    }

    private static object ApplyDependency(Type dependencyType, object service, object dependency)
    {
        string serviceTypeName = service.GetType().Name;
        // Need to get the generic SetDependency method on the service, typed to depType.
        var setDependencyMethod = service.GetType()
            .GetMethod("SetDependency", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.MakeGenericMethod(dependencyType);

        if (setDependencyMethod == null)
        {
            throw new Exception($"The {serviceTypeName} does not expose the required SetDependency method.");
        }

        setDependencyMethod.Invoke(service, [dependency]);
        return service;
    }

    private static object CreateDependencyInstance(
        Type dependencyType, 
        IConfigurationSection componentConfig,
        string serviceTypeName,
        IServiceProvider? standardDependencies = null)
    {
        string dependencyContractName = dependencyType.Name;
        var dependencyConfiguration
            = GetDependencyConfiguration(componentConfig, dependencyType);

        if (dependencyConfiguration == null)
        {
            throw new Exception($"{serviceTypeName} requires an instance of {dependencyContractName} that has no configuration specification.");
        }

        string concreteAssemblyName = dependencyConfiguration["Assembly"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(concreteAssemblyName) == true)
        {
            throw new Exception($"No assembly has been identified to resolve the {dependencyContractName} dependency held by {serviceTypeName}");
        }

        var dependencyFactory = GetDependencyFactory(dependencyType, concreteAssemblyName);

        MethodInfo? concreteFactoryFunction = GetFactoryFunction(dependencyFactory);
        
        if(concreteFactoryFunction == null)
        {
            throw new Exception($"The {dependencyFactory.GetType().Name} does not provide the required CreateService method.");
        }

        var serviceDependency = concreteFactoryFunction
            .Invoke(dependencyFactory, [dependencyConfiguration, standardDependencies]);

        if(serviceDependency == null)
        {
            throw new Exception($"Could not create an instance of {dependencyContractName}.");
        }

        return serviceDependency;
    }

    private static IConfigurationSection GetDependencyConfiguration(
        IConfigurationSection serviceConfiguration, 
        Type dependencyType)
    {
        string dependencyContractName = dependencyType.Name;
        IConfigurationSection dependencyConfiguration = serviceConfiguration.GetSection(dependencyContractName);
        return dependencyConfiguration;
    }

    private static object GetDependencyFactory
        (
            Type dependencyType,
            string concreteAssemblyName
        )
    {
        string dependencyContractName = dependencyType.Name;

        Assembly concreteAssembly = Assembly.Load(concreteAssemblyName);
        Type abstractFactoryType = typeof(IServiceFactory<>).MakeGenericType(dependencyType);
        Type concreteFactoryType = concreteAssembly.ExportedTypes
            .Where(t => t.IsAssignableTo(abstractFactoryType))
            .First();
        if (concreteFactoryType == null)
        {
            throw new Exception($"The configured Assembly for {dependencyContractName} does not contain a component factory for that type.");
        }
        var dependencyFactory = Activator.CreateInstance(concreteFactoryType);

        if(dependencyFactory == null)
        {
            throw new Exception($"Could not create a factory to build an {dependencyContractName} implementation.");
        }
        return dependencyFactory;
    }

    private static MethodInfo GetFactoryFunction(object dependencyFactory)
    {
        Type concreteFactoryType = dependencyFactory.GetType();
        MethodInfo? concreteFactoryFunction = concreteFactoryType.GetMethod(nameof(CreateService));
        if (concreteFactoryFunction == null)
        {
            throw new Exception($"The {concreteFactoryType.Name} does not provide the required CreateService method.");
        }
        return concreteFactoryFunction;
    }
}
