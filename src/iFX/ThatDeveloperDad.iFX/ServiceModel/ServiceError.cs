using System;
using System.Text;

namespace ThatDeveloperDad.iFX.ServiceModel;

public class ServiceError
{
    public string Site { get; set; } = string.Empty;

    public string ErrorKind { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;

    public ErrorSeverity Severity { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Severity: {Severity}");
        sb.AppendLine($"ErrorKind: {ErrorKind}");
        sb.AppendLine($"Site: {Site}");
        sb.AppendLine($"Message: {Message}");

        return sb.ToString();
    }
}

public enum ErrorSeverity
{
    /// <summary>
    /// Identifies a condition that does not signal that a process should be halted,
    /// but may result in a problems further on.
    /// </summary>
    Warning,
    /// <summary>
    /// Identifies a condition that signals a process should be halted immediately
    /// due to a critical error.
    /// </summary>
    Error
}
