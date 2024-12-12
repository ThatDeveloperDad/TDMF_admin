using System;
using DevDad.SaaSAdmin.iFX;

namespace DevDad.SaaSAdmin.Catalog.Abstractions;

public class QuotaGrantResource
{
    /// <summary>
    /// What kind of Metered Resource are we controlling with this?
    /// </summary>
    public MeteredResourceKinds ResourceKind {get;set;}

    /// <summary>
    /// The number of Resources that are granted when the grant is applied.
    /// </summary>
    public int InitialBudget { get; set; }

    /// <summary>
    /// The number of resources that are granted when a grant is renewed.
    /// </summary>
    public int RenewalBudget { get; set; }
}

