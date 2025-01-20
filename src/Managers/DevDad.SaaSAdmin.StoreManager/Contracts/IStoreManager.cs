using System;
using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace DevDad.SaaSAdmin.StoreManager.Contracts;

/// <summary>
/// Provides the orchestration of ResourceAccess and Logic/Compute services
/// to perform activities against the online Store (Ecommerce) service.
/// 
/// If we switch to a different EcommerceProvider in the future, we shouldn't need
/// to do more than implement the IStoreAccess contract against its API
/// and do the ol' switcheroo on the StoreProvider. :)
/// </summary>
public interface IStoreManager : IManagerService
{
    
    Task<StartCheckoutResponse> StartCheckoutAsync(StartCheckoutRequest request);
}
