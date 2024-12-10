using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using Microsoft.Graph;
using ThatDeveloperDad.iFX.ServiceModel;
using Azure.Identity;
using Microsoft.Extensions.Logging;

namespace DevDad.SaaSAdmin.UserIdentity.EntraB2C;

public class UserAccessEntraProvider 
    : IUserIdentityAccess, IDisposable
{
    public string IdentityVendor => "MS-Entra";

    private GraphServiceClient? _clientInstance;
    private ClientSecretCredential? _secret;
    private readonly MsGraphOptions _options;
    private readonly ILogger? _logger;
    private bool disposedValue;

    public UserAccessEntraProvider(MsGraphOptions options,
        ILoggerFactory? loggerFactory)
    {
        _logger = loggerFactory?.CreateLogger<UserAccessEntraProvider>();
        _options = options;
    }

    private GraphServiceClient MsGraph
    {
        get
        {
        if(_clientInstance == null)
        {
           

            if(_options == null)
            {
                throw new InvalidOperationException("Cannot connect to MS Graph.  Required configuration is missing.");
            }
            _secret = new ClientSecretCredential(
                tenantId: _options.TenantId,
                clientId: _options.ClientId,
                clientSecret: _options.ClientSecret
            );
            
            _clientInstance = new GraphServiceClient(_secret, ["https://graph.microsoft.com/.default"]);
        }
        return _clientInstance;
        }
        
    }

    public async Task<LoadIdentityResponse> LoadUserIdentityAsync(LoadIdentityRequest request)
    {
        LoadIdentityResponse response = new(request);

        if(request.UserId == null)
        {
            response.AddError(new ServiceError{
                Severity = ErrorSeverity.Error,
                Message = "The UserId is required to load a user identity.",
                Site = "UserAccessEntraProvider.LoadUserIdentityAsync",
                ErrorKind = "MissingRequestData"
            });
            return response;
        }

        try
        {
            var graphResult = await MsGraph.Users[request.UserId].GetAsync();
            if(graphResult == null)
            {
                response.AddError(new ServiceError{
                    Severity = ErrorSeverity.Error,
                    Message = "No user was found with the provided UserId.",
                    Site = "UserAccessEntraProvider.LoadUserIdentityAsync",
                    ErrorKind = "UserIdentity_NotFound"
                });
                return response;
            }

            UserIdentityResource userResource = new()
            {
                UserId = graphResult.Id??request.UserId,
                DisplayName = graphResult!.DisplayName??string.Empty,
            };
            response.Payload = userResource;
            return response;
        }
        catch(Exception e)
        {
            // We're not awaiting this Log statement.  It's designed to be Fire & Forget.
            _logger?.LogError(e, "An exception occurred while connecting to the Identity Store.");
            response.AddError(new ServiceError{
                Severity = ErrorSeverity.Error,
                Message = "An exception occurred while connecting to the Identity Store.",
                Site = "UserAccessEntraProvider.LoadUserIdentityAsync",
                ErrorKind = "RemoteServiceException"
            });
        }
        return response;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                MsGraph.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
