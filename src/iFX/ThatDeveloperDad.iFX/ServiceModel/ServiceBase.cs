using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace ThatDeveloperDad.iFX.ServiceModel;

public abstract class ServiceBase : ISystemComponent
{
    private Type? _serviceArchetype;

    protected ILogger? _logger;

    protected ServiceBase()
    {
        _serviceArchetype = ComputeServiceArchetype();
    }
    
    private Type? ComputeServiceArchetype(Type? type = null)
    {
        // if type is null, assume we're looking for THIS component's archetype.
        if(type == null)
        {
            type = this.GetType();
        }
        
        List<Type> archetypes = ((ISystemComponent)this).ServiceArchetypes.ToList();
        var interfaces = type.GetInterfaces();
        
        if(interfaces.Count(i=> archetypes.Contains(i)) != 1)
        {
            throw new InvalidOperationException($"The Service {type.Name} must implement one and only one ServiceArchetype.");
        }

        return interfaces
            .Where(i => archetypes.Contains(i))
            .FirstOrDefault();
    }

    protected IServiceOptions? _options;
    protected Dictionary<Type, ISystemComponent> _dependencies = new();
    protected T Options<T>() where T: IServiceOptions
    {
        if (_options == null)
        {
            throw new InvalidOperationException("Options not set");
        }

        return (T)_options;
    }

    public void SetConfiguration(IServiceOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Enforce the closed architecture rules for the Depndency Archetypes.
    /// </summary>
    /// <param name="dependencyType"></param>
    protected virtual void EnforceDependencyPolicy(Type dependencyType)
    {
        // First, we need to evaluate the Archetype of the CURRENT service.
        // (Store this in a local field so we don't have to do it over and over again.)
        if(_serviceArchetype == null)
        {
            _serviceArchetype = ComputeServiceArchetype();
        }

        if(_serviceArchetype == null)
        {
            throw new InvalidOperationException($"The Service {this.GetType().Name} is not a valid Service Archetype.");
        }

        Type? dependencyArchetype = ComputeServiceArchetype(dependencyType);

        if(dependencyArchetype == null)
        {
            throw new InvalidOperationException($"The Dependency Type {dependencyType.Name} is not a valid Dependency Archetype.");
        }

        if(dependencyArchetype == _serviceArchetype)
        {
            throw new InvalidOperationException($"The Dependency Type {dependencyType.Name} is the same Archetype as the current Service, which is invalid.");
        }

        switch(_serviceArchetype.Name)  // If THIS service is a...
        {
            case nameof(IClientService):
                // We can only take Managers or Utilities as dependencies.
                if(dependencyArchetype == typeof(IManagerService) || dependencyArchetype == typeof(IUtilityService))
                {
                    return;
                }
                throw new InvalidOperationException($"Client Components may not have dependencies on {dependencyArchetype.Name} Components.");
            case nameof(IManagerService):
                // We can take only Engines, ResourceAccess, or Utilities as dependencies.
                if(dependencyArchetype == typeof(IEngineService) || dependencyArchetype == typeof(IResourceAccessService) || dependencyArchetype == typeof(IUtilityService))
                {
                    return;
                }
                throw new InvalidOperationException($"Manager Components may not have dependencies on {dependencyArchetype.Name} Components.");
            case nameof(IEngineService):
                // Engines may ony take ResourceAccess or Utility dependencies.
                if(dependencyArchetype == typeof(IResourceAccessService) || dependencyArchetype == typeof(IUtilityService))
                {
                    return;
                }
                throw new InvalidOperationException($"Engine Components may not have dependencies on {dependencyArchetype.Name} Components.");
                
            case nameof(IResourceAccessService):
                // ResourceAccess components may only take Utility dependencies.
                if(dependencyArchetype == typeof(IUtilityService))
                {
                    return;
                }
                throw new InvalidOperationException($"ResourceAccess Components may not have dependencies on {dependencyArchetype.Name} Components.");
                
            case nameof(IUtilityService):
                throw new InvalidOperationException($"Utility Components may not have depndencies on other Archetypal Components.");

            default:
                //nothing to do here.
                return;    
        }
    }
    protected void SetDependency<T>(T serviceDependency) where T: ISystemComponent
    {
        EnforceDependencyPolicy(typeof(T));

        Type dependencyType = typeof(T);
        _dependencies[dependencyType] = serviceDependency;
    }
    protected T GetProxy<T>() where T: ISystemComponent
    {
        T service = (T)_dependencies[typeof(T)];
        return service;
    } 

    public void AddLogger(ILoggerFactory? loggerFactory)
    {
        if(loggerFactory == null)
        {
            return;
        }

        _logger = loggerFactory.CreateLogger(this.GetType().Name);
    }

    protected async Task LogInformationAsync(string message)
    {
        try
        {
            if(_logger == null)
            {
                return;
            }

            await Task.Run(() => _logger?.LogInformation(message));
        }
        catch
        {
            // eat the error.
        }
    }

    protected async Task LogExceptionAsync(Exception e)
    {
        try
        {
            if(_logger == null)
            {
                return;
            }

            await Task.Run(() => _logger?.LogError(e, e.Message));
        }
        catch
        {
            // eat the error.
        }
    }
}
