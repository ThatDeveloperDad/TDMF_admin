using System;

namespace ThatDeveloperDad.iFX.Behaviors;

public class MethodContext
{
    public string MethodName { get; set; }
    public object[] Parameters { get; set; }
    public object ReturnValue { get; set; }
    public Exception Exception { get; set; }
}
