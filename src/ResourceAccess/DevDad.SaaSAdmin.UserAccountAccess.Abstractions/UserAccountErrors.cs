using System;

namespace DevDad.SaaSAdmin.UserAccountAccess.Abstractions;

/// <summary>
/// A collection of constants used to identify known errors that may
/// occur in the UserAccountAccess component.
/// </summary>
public class UserAccountErrors
{
    public const string UserAccountResource_Conversion = "ResourceToEntity_MappingError";
    public const string UserAccountResource_StorageError = "ResourceStorageError";
}
