
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.Behaviors;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace ThatDeveloperDad.iFX.Utilities;
/// <summary>
/// Specifies behavior that will execute before and/or after a method is invoked.
/// </summary>
public class LoggingBehavior<T> 
    : IOperationBehavior
    where T: ISystemComponent
{
    private ILogger _logger;
    private LoggingBehavior(ILogger? logger )
    {
        _logger = logger;
    }

    public static LoggingBehavior<T>? Create(IServiceProvider? serviceProvider = null)
    {
        if(serviceProvider == null)
        {
            return null;
        }

        var logFactory = serviceProvider.GetService<ILoggerFactory>();
        return LoggingBehavior<T>.Create(logFactory);
    }

    public static LoggingBehavior<T>? Create(ILoggerFactory? logFactory = null)
    {
        if(logFactory == null)
        {
            return null;
        }

        return new LoggingBehavior<T>(logFactory.CreateLogger<T>());
    }

    private long? _methodStartedAt;

    public Task OnMethodEntryAsync(MethodContext context)
    {
        if(_logger == null)
        {
            return Task.CompletedTask;
        }
        _methodStartedAt = DateTime.Now.Ticks;
        _logger.LogInformation($"Entering {context.MethodName} with parameters {context.Parameters}");
        return Task.CompletedTask;
    }

    public Task OnMethodExitAsync(MethodContext context)
    {
        if(_logger == null)
        {
            return Task.CompletedTask;
        }
        
        var methodDuration = DateTime.Now.Ticks - _methodStartedAt;
        TimeSpan execTime = new TimeSpan(methodDuration??0);
        _logger.LogInformation($"Exiting {context.MethodName} with parameters {context.Parameters} in {execTime.Milliseconds} ms");
        return Task.CompletedTask;
    }
}
