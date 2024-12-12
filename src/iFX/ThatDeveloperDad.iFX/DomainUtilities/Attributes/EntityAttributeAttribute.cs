using System;

namespace ThatDeveloperDad.iFX.DomainUtilities.Attributes;

/// <summary>
/// Identifies a Property of a Local Idiom as an Attribute of the Entity identified by the containing class.
/// Used by the DomainObjectMapper to map between different idiomatic expressions of the same Domain Entity.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class EntityAttributeAttribute : Attribute
{
    /// <summary>
    /// Identifies the name of the Property in the Domain Entity that the decorated Property represents.
    /// </summary>
    public string EntityAttributeName { get; }

    /// <summary>
    /// Identifies the name of the Property in the Domain Entity that the decorated Property represents.
    /// </summary>
    /// <param name="entityAttributeName">The name of the Property in the Domain Entity that the decorated Property represents.</param>
    public EntityAttributeAttribute(string entityAttributeName)
    {
        EntityAttributeName = entityAttributeName;
    }
}
