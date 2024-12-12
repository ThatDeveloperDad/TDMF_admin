using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThatDeveloperDad.iFX.DomainUtilities;

/// <summary>
/// Base Class that can be applied to any DTO or POCO to identify it as
/// a local idiomatic form of a defined Domain Entity.
/// 
/// This mechanism is currently used by the DomainObjectMapper to ease the
/// Developer Experience of mapping between instances of different idiomatic classes
/// that represent the smae conceptual Domain Entity.
/// </summary>
public abstract class IdiomaticType : IDomainIdiom
{
    // It'd be really cool to build in a Roslyn Analyzer that would inspect
    // classes that inherit this and ensure that they are decorated with 
    // the DomainEntityAttribute.
    private string? _entityName;
    
    public string EntityName  
    {
        get
        {
            // use the Lazy-load pattern here.
            if(_entityName != null)
            {
                return _entityName;
            }
            _entityName = this.ReadDomainEntityName();
            return _entityName;
        }
    }


    private Dictionary<string, PropertyInfo>? _entityAttributes;
    public Dictionary<string, PropertyInfo> EntityProperties
    {
        get
        {
            // use the Lazy-load pattern here too.
            if(_entityAttributes != null)
            {
                return _entityAttributes;
            }
            _entityAttributes = this.ReadEntityAttributes();
            return _entityAttributes;
        }
    }
}
