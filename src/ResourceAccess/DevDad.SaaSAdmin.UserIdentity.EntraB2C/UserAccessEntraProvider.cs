using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserIdentity.EntraB2C;

internal class UserAccessEntraProvider 
    : ServiceBase, IUserIdentityAccess
{
    public string IdentityVendor => "MS-Entra";

    public async Task<Abstractions.UserIdentity?> LoadUserIdentityAsync(string userId)
    {
        await Task.Delay(1);
        Console.WriteLine($"Looking up User with Id {userId}");
        return null;
    }

}
