using System;
using System.Threading.Tasks;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.StoreAccess.Abstractions;

/// <summary>
/// Describes the behaviors provided by the Store Access component.
/// </summary>
public interface IStoreApiAccess : IResourceAccessService
{

    /// <summary>
    /// Creates a checkout session at LemonSqueezy.
    /// If successful, a URL to the Checkout Page for the identified
    /// customer and product is returned which can be used to open a new
    /// browser tab in the main application.
    /// </summary>
    /// <param name="request">CheckoutRequest is a child of OperationRequest<> with CheckoutData as its payload.</param>
    /// <returns>Task<CheckoutResponse>.  CheckoutResponse is a child of OperationResponse with a nullable string containing the end-user checkout URL if the call is successful.</returns>
    Task<CheckoutResponse> StartCheckoutAsync(CheckoutRequest request);
}
