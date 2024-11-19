using System;

namespace ThatDeveloperDad.iFX.Behaviors;

public interface IOperationBehavior
{
    Task OnMethodEntryAsync(MethodContext context)
        => Task.CompletedTask;
    Task OnMethodExitAsync(MethodContext context)
        => Task.CompletedTask;

    void OnMethodEntry(MethodContext context)
        => OnMethodEntryAsync(context).Wait();
    void OnMethodExit(MethodContext context)
        => OnMethodExitAsync(context).Wait();
}
