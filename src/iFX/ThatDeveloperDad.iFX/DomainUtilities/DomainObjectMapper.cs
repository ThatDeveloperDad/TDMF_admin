using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ThatDeveloperDad.iFX.CollectionUtilities;
using ThatDeveloperDad.iFX.DomainUtilities.Attributes;

namespace ThatDeveloperDad.iFX.DomainUtilities;

/// <summary>
/// Provides methods by which we can more easily and consistently map between
/// different idiomatic expressions of Canonical Domain Objects that are declared
/// in our systems.
/// </summary>
public class DomainObjectMapper
{

    /// <summary>
    /// Maps a class instance from its TSource idiom to an instance of the TDestination idiom.
    /// Both classes must implement IDomainIdiom, and both must be decorated with the DomainEntityName attribute, 
    /// identifying the same Canonical Domain Entity.
    /// 
    /// Remember, there may not be exact property-parity between Local Idioms for a Canonical Type.
    /// Only the properties that match between Source and Destination will be copied.
    /// </summary>
    /// <typeparam name="TSource">The Type of the SOURCE model</typeparam>
    /// <typeparam name="TDestination">The Type of the desired DESTINATION model</typeparam>
    /// <param name="sourceInstance">And instance of the Source idiomatic Type</param>
    /// <returns>An instance of the Destination type, with its properties populated from the corresponding EntityAttributes from the Source model.</returns>
    /// <exception cref="ArgumentException">If either Source or Destination types are not marked as
    /// an Idiomatic Type of the DomainEntity, or that DomainEntity name is is different, we'll throw 
    /// and ArgumentException.
    /// </exception>
    public static TDestination Map<TSource, TDestination>
        (
            TSource sourceInstance,
            TDestination? destinationInstance = default
        )
        where TSource : IDomainIdiom
        where TDestination : IDomainIdiom
    {
        if(destinationInstance == null || destinationInstance.Equals(default(TDestination)))
        {
            destinationInstance = Activator.CreateInstance<TDestination>();
        }

        GuardTypesAreEquivalent<TSource, TDestination>();

        Dictionary<string, PropertyInfo> sourceMap = GetMappableProperties(typeof(TSource));
        Dictionary<string, PropertyInfo> destinationMap = GetMappableProperties(typeof(TDestination));
        var commonKeys = sourceMap.Keys.Intersect(destinationMap.Keys);

        if(commonKeys.Count() == 0)
        {
            throw new ArgumentException($"The source and destination types must have at least one common attribute to map.");
        }

        foreach(string sourceKey in commonKeys)
        {
            PropertyInfo sourceProperty = sourceMap[sourceKey];
            PropertyInfo destinationProperty = destinationMap[sourceKey];
            object? sourceValue = sourceProperty.GetValue(sourceInstance);
            destinationProperty.SetValue(destinationInstance, sourceValue);
        }

        return destinationInstance;
    }

    /// <summary>
    /// Maps a Filter built against an Idiom of a DomainEntity to
    /// a filter for the same DomainEntity expressed by a different Idiom.
    /// </summary>
    /// <typeparam name="TSource">The Type of the original Filter</typeparam>
    /// <typeparam name="TDestination">The Type for the new Filter</typeparam>
    /// <param name="currentFilter">The instance of Filter<typeparamref name="TSource"/> to be converted.</param>
    /// <returns>A Filter<typeparamref name="TDestination"/>.
    /// FilterCriteria that cannot be mapped between Source and Destination will not be included
    /// on the destination filter.
    /// </returns>
    public static Filter<TDestination> MapFilter<TSource, TDestination>(Filter<TSource> currentFilter)
        where TSource : class, IDomainIdiom
        where TDestination : class, IDomainIdiom
    {
        GuardTypesAreEquivalent<TSource, TDestination>();

        Filter<TDestination> newFilter = new();
        foreach(var criterion in currentFilter.Criteria)
        {
            string entityAttributeName = GetEntityAttributeName(criterion.PropertyName, typeof(TSource));
            if(string.IsNullOrWhiteSpace(entityAttributeName) == true)
            {
                continue;
            }
            string destinationPropertyName = GetPropertyForAttribute(entityAttributeName, typeof(TDestination));
            if(string.IsNullOrWhiteSpace(destinationPropertyName) == true)
            {
                continue;
            }
            newFilter.AddCriteria(
                destinationPropertyName, 
                criterion.Operator, 
                criterion.ExpectedValue);
        }

        return newFilter;
    }

    /// <summary>
    /// Use this when we have instances of the types to be mapped.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="sourceInstance"></param>
    /// <param name="destinationInstance"></param>
    /// <exception cref="ArgumentException"></exception>
    private static void GuardTypesAreEquivalent<TSource, TDestination>(
        TSource sourceInstance,
        TDestination destinationInstance)
        where TSource : IDomainIdiom
        where TDestination : IDomainIdiom
    {
        if(sourceInstance.EntityName != destinationInstance.EntityName)
        {
            throw new ArgumentException($"The source and destination types must represent the same Domain Entity.  Source: {sourceInstance.EntityName}, Destination: {destinationInstance.EntityName}");
        }
    }

    /// <summary>
    /// use this when we don't have instances of the two types.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <exception cref="ArgumentException"></exception>
    private static void GuardTypesAreEquivalent<TSource, TDestination>()
        where TSource : IDomainIdiom
        where TDestination : IDomainIdiom
    {
        string sourceEntityName = GetEntityName(typeof(TSource));
        string destinationEntityName = GetEntityName(typeof(TDestination));

        if(sourceEntityName != destinationEntityName)
        {
            throw new ArgumentException($"The source and destination types must represent the same Domain Entity.  Source: {sourceEntityName}, Destination: {destinationEntityName}");
        }
    }

    /// <summary>
    /// Retrieves the EntityName that the idiom represents.
    /// </summary>
    /// <param name="idiom">The type of the class that defines a Local Idiom for a Canonical Domain Entity</param>
    /// <returns>The name of the DomainEntity that the idiom represents.</returns>
    /// <exception cref="ArgumentException">If the provided Type does NOT express a DomainEntity, we can't work with it in this Mapper utility.</exception>
    private static string GetEntityName(Type idiom)
    {
        string entityName;
        var entityAttribute = idiom.GetCustomAttribute<DomainEntityAttribute>();
        if(entityAttribute != null)
        {
            entityName = entityAttribute.EntityName;
            return entityName;
        }

        throw new ArgumentException($"The type declaration for '{idiom.Name}' must be decorated with a DomainEntityAttribute.");
    }

    /// <summary>
    /// Retrieves the EntityAttribute name that the property on the idiom represents.
    /// </summary>
    /// <param name="propertyName">The name of the Property on the idiom Type</param>
    /// <param name="idiom">The Type we're inspecting</param>
    /// <returns>The EntityAttributeName value that as been assigned as metadata on the target property.
    /// IF the target property has not been decorated with an EntityAttribute, we'll return an empty string.
    /// </returns>
    private static string GetEntityAttributeName(string propertyName, Type idiom)
    {
        string entityAttributeName = string.Empty;
        PropertyInfo? property = idiom.GetProperty(propertyName);
        if(property == null)
        {
            throw new ArgumentException($"Property {propertyName} not found on {idiom.Name}");
        }

        var entityAttribute = property.GetCustomAttribute<EntityAttributeAttribute>();
        if(entityAttribute != null)
        {
            entityAttributeName = entityAttribute.EntityAttributeName;
        }

        return entityAttributeName;
    }

    private static string GetPropertyForAttribute(string entityAttributeName, Type idiom)
    {
        string propertyName = string.Empty;
        PropertyInfo[] decoratedProperties = idiom.GetProperties()
            .Where(p => p.GetCustomAttribute<EntityAttributeAttribute>() != null)
            .ToArray();

        foreach(var property in decoratedProperties)
        {
            var entityAttribute = property.GetCustomAttribute<EntityAttributeAttribute>();
            if(entityAttribute != null && entityAttribute.EntityAttributeName == entityAttributeName)
            {
                propertyName = property.Name;
                break;
            }
        }

        return propertyName;
    }

    private static Dictionary<string, PropertyInfo> GetMappableProperties(Type idiom)
    {
        Dictionary<string, PropertyInfo> mappableProperties = new();
        PropertyInfo[] properties = idiom.GetProperties()
            .Where(p => p.GetCustomAttribute<EntityAttributeAttribute>() != null)
            .ToArray();

        foreach(var property in properties)
        {
            var entityAttribute = property.GetCustomAttribute<EntityAttributeAttribute>();
            if(entityAttribute != null)
            {
                mappableProperties.Add(entityAttribute.EntityAttributeName, property);
            }
        }

        return mappableProperties;
    }
}
