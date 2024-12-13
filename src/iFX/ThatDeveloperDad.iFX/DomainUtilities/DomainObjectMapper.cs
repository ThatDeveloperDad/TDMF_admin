using System;
using System.Collections.Generic;
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
    //TODO:  When we review the work we've done here, we should move over to ConcurrentDictionary
    // for both of these in-mem caches. Explain why.

    // Builds up a persistent runtime cache of IdiomaticTypes and the DomainEntity they represent.
    // Hopfully, this will cust down on the number of reflection calls we need to make.
    private static Dictionary<Type, string> _idiomEntityCache = new();
    
    // Builds up a persistent runtime cache of IdiomaticType pairs and the MethodInfo
    // for the Generic Map<> method used to execute the mapping.
    private static Dictionary<(Type, Type), MethodInfo> _mapMethodCache = new();

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
    public static TDestination MapEntities<TSource, TDestination>
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

        GuardTypesAreEquivalent(sourceInstance, destinationInstance);

        Dictionary<string, PropertyInfo> sourceMap = sourceInstance.EntityProperties;
        Dictionary<string, PropertyInfo> destinationMap = destinationInstance.EntityProperties;
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
            
            destinationInstance = ConvertAttribute
                (
                    destinationInstance, 
                    sourceValue,
                    sourceProperty,
                    destinationProperty
                );
        }

        return destinationInstance;
    }

    private static TDestination ConvertAttribute<TDestination>
        (
            TDestination destinationInstance, 
            object? sourceValue, 
            PropertyInfo sourceProperty, 
            PropertyInfo destinationProperty
        )
    {
        if(sourceValue == null)
        {
            return destinationInstance;
        }

        EntityAttributeAttribute attributeInfo = sourceProperty
            .GetCustomAttribute<EntityAttributeAttribute>()!;

        
        object? destinationValue = null;

        bool destinationAssigned = false;
        // If the source and destination property types are assignable, we can just assign the value.
        if(sourceProperty.PropertyType.IsAssignableTo(destinationProperty.PropertyType))
        {
            destinationValue = sourceValue;
            destinationAssigned = true;
        }

        if(destinationAssigned == false 
           && attributeInfo.IsCollection == false 
           && attributeInfo.IsEntityValue == true)
        {
            destinationValue = ConvertEntityValue(
                sourceValue, 
                sourceProperty.PropertyType, 
                destinationProperty.PropertyType);
            destinationAssigned = true;
        }

        if(destinationAssigned == false 
           && attributeInfo.IsCollection)
        {            
            destinationValue =  ConvertCollectionValue(sourceValue, sourceProperty, destinationProperty);
            destinationAssigned = true;
        }

        if(destinationAssigned == false)
        {
            throw new ArgumentException("DomainObjectMapper.TransferAttributeValue has an unaccounted edge case.");
        }

        destinationProperty.SetValue(destinationInstance, destinationValue);
        return destinationInstance;
    }

    // Create asimple mapping method for complex ENtity TYpes when we only have objects.
    private static object? ConvertEntityValue
        (
            object? sourceValue, 
            Type sourceType, 
            Type destType
        )
    {
        object? destinationValue = null;
        if(sourceValue == null)
        {
            return destinationValue;
        }

        // Try the MapMethod cache first... ;)
        if(_mapMethodCache.ContainsKey((sourceType, destType)))
        {
            MethodInfo mapMethod = _mapMethodCache[(sourceType, destType)];
            destinationValue = mapMethod.Invoke(null, new object?[] { sourceValue, destinationValue });
            return destinationValue;
        }

        // We need to make sure the source & destination are compatible.
        string sourceEntity = GetEntityName(sourceType);
        string destEntity = GetEntityName(destType);
        if(sourceEntity != destEntity)
        {
            throw new ArgumentException($"Cannot map between types that do not express the same Entity.  Source: {sourceEntity}, Destination: {destEntity}");
        }

        MethodInfo baseMap = typeof(DomainObjectMapper)
            .GetMethod(nameof(MapEntities), BindingFlags.Public | BindingFlags.Static)!;

        MethodInfo typedMap = baseMap.MakeGenericMethod(
            sourceType, 
            destType);

        destinationValue = typedMap.Invoke(null, new object?[] { sourceValue, null });

        // if we got this far without an exception, let's cache the mapping method with the Type Pair.
        _mapMethodCache.Add((sourceType, destType), typedMap);

        return destinationValue;
    }

    /// <summary>
    /// Determines the correct Type of collection expected by the destination property,
    /// then populates it from the sourceValue.
    /// 
    /// This will work whether we're dealing with collections of simp[le values,
    /// or collections of IdiomaticTypes.
    /// 
    /// This will not work for collections of complex Non-Domain types.
    /// </summary>
    /// <param name="sourceValue">The VALUE of the source Property on the DomainEntity being mapped</param>
    /// <param name="sourceProp">The PropertyInfo for the Source being mapped FROM.</param>
    /// <param name="destProp">The PropertyInfo for the destination instance property that we're mapping TO</param>
    /// <returns>An instance of the Type expected by the Destination Property</returns>
    private static object? ConvertCollectionValue(object? sourceValue, PropertyInfo sourceProp, PropertyInfo destProp)
    {
        if(sourceValue == null)
        {
            return null;
        }

        Type sourceElementType = sourceProp.PropertyType.GetGenericArguments().FirstOrDefault()!;
        IEnumerable<object?> sourceCollection = (IEnumerable<object?>)sourceValue;

        // Need to create a destination list of the same element type of the destination Property.
        // Otherwise, iwe lose type fidelity and the runtime throws exceptions.
        Type destinationElementType = destProp.PropertyType.GetGenericArguments().FirstOrDefault()!;
        Type destListType = typeof(List<>).MakeGenericType(destinationElementType);
        
        // Because Activator.CreateInstance returns an object?, And we can't cast to a 
        // generic type that we only know at runtime, we have to use the non-generic IList.
        // Don't worry, this works just fine.
        var destList = (System.Collections.IList?)Activator.CreateInstance(destListType);
        if(destList == null)
        {
            throw new ArgumentException($"Could not create the required destination List Type.");
        }
        // If we can map directly, our loop iterations will go WAY faster.
        // Especially if we do this reflection call OUTSIDE of the loop.
        bool canDirectMap = sourceElementType.IsAssignableTo(destinationElementType);

        foreach(var element in sourceCollection)
        {
            if(canDirectMap == true)
            {
                destList.Add(element);
            }
            else
            {
                var destElement = ConvertEntityValue(element, sourceElementType, destinationElementType);
                destList.Add(destElement);
            }
        }

        return destList;
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
    /// Compares the Types of the provided instances to ensure they represent the same DomainEntity.
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
    /// Determines whether two types are both Idiomatic of the same DomainEntity.
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
        if(_idiomEntityCache.ContainsKey(idiom))
        {
            return _idiomEntityCache[idiom];
        }

        var entityAttribute = idiom.GetCustomAttribute<DomainEntityAttribute>();
        if(entityAttribute != null)
        {
            entityName = entityAttribute.EntityName;
            _idiomEntityCache.Add(idiom, entityName);
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

    /// <summary>
    /// Looks for a property on the provided Type that is assigned to the provided EntityAttributeName.
    /// 
    /// Used when converting Filter Criteria from one Filter Target type to another, when the Filter
    /// Targets are Idiomatic for the same DomainEntity.
    /// </summary>
    /// <param name="entityAttributeName"></param>
    /// <param name="idiom"></param>
    /// <returns></returns>
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

}
