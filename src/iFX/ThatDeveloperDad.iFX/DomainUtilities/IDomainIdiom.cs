using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThatDeveloperDad.iFX.DomainUtilities;

/// <summary>
/// Marker interface that identifies the implementing class as a local idiom of a DomainEntity.
/// 
/// The Domain metadata is to be expressed on the class and its properties using attributes
/// defined in ThatDeveloperDad.iFX.DomainUtilities.Attributes.
/// </summary>
public interface IDomainIdiom 
{ 
    /// <summary>
    /// The name of the DomainEntity that the implementing class is a local idiom of.
    /// </summary>
    string EntityName { get; }

    Dictionary<string, PropertyInfo> EntityProperties { get; }
}
