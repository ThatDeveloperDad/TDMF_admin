using System;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.Behaviors;

namespace TestConsole.ProgramBehaviors;

/// <summary>
/// Specifies behavior that will execute before and/or after a method is invoked.
/// </summary>
public class LoggingBehavior : IOperationBehavior
{
    private ILogger _logger;
    public LoggingBehavior(ILogger? logger )
    {
        _logger = logger;
    }

    public Task OnMethodEntryAsync(MethodContext context)
    {
        if(_logger == null)
        {
            return Task.CompletedTask;
        }

        _logger.LogInformation($"Entering {context.MethodName} with parameters {context.Parameters}");
        return Task.CompletedTask;
    }
}
