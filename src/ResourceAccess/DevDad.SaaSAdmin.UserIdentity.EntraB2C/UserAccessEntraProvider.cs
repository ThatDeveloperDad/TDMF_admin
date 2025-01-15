using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using ThatDeveloperDad.iFX.ServiceModel;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using DevDad.SaaSAdmin.UserIdentity.EntraB2C.Internals;
using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.CollectionUtilities;

namespace DevDad.SaaSAdmin.UserIdentity.EntraB2C;

public class UserAccessEntraProvider 
    : IUserIdentityAccess, IDisposable
{
    public string IdentityVendor => ExternalServiceVendors.MsEntra;

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

        var validationProblems = new LoadIdentityRequestValidator()
            .Validate(request);

        if(validationProblems.Any())
        {
            foreach(var error in validationProblems)
            {
                error.Site = $"{nameof(UserAccessEntraProvider)}.{nameof(LoadUserIdentityAsync)}";
                response.AddError(error);
            }
            // If any of the identified problems are fatal, we need to stop now.
            if(response.HasErrors)
            {
                return response;
            }
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
                    ErrorKind = IdentityServiceConstants.ErrorKinds.UnknownIdentity
                });
                return response;
            }

            UserIdentityResource userResource = new()
            {
                UserId = graphResult.Id??request.UserId!,
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
                Site = $"{nameof(UserAccessEntraProvider)}.{nameof(LoadUserIdentityAsync)}",
                ErrorKind = $"{ServiceErrorKinds.StepFailed}{IdentityServiceConstants.ErrorKinds.ExternalApiError}"
            });
        }
        return response;
    }

    /// <summary>
    /// Ensures that the user has membership only in the specified groups.
    /// 
    /// Because there MAY be groups in the user's membership collection that
    /// are not part of the Application we're administering, we'll use
    /// the AppGroupPrefix in the application iFX to filter the membership
    /// reconciliation only to those groups.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ReconcileMembershipsResponse> ReconcileUserMembershipsAsync(ReconcileMembershipsRequest request)
    {
        ReconcileMembershipResult result = new();
        ReconcileMembershipsResponse response = new(request, result);
        
        var validationProblems = new ReconcileMembershipRequestValidator()
            .Validate(request);

        if(validationProblems.Any())
        {
            foreach(var error in validationProblems)
            {
                error.Site = $"{nameof(UserAccessEntraProvider)}.{nameof(ReconcileUserMembershipsAsync)}";
                response.AddError(error);
            }
            // If any of the identified problems are fatal, we need to stop now.
            if(response.HasErrors)
            {
                response.Completed = false;
                return response;
            }
        }

        var payload = request.Payload;

        try
        {
            var user = await MsGraph.Users[payload!.UserId]
                .GetAsync();

            var allGroups = await MsGraph.Groups
                .GetAsync();

            var requiredGroups = allGroups?.Value?
                .Where(g=> payload.ExpectedGroups.Contains(g.DisplayName))
                .ToList();

            if (user == null)
            {
                response.AddError(new ServiceError
                {
                    Severity = ErrorSeverity.Error,
                    Message = "No user was found with the provided UserId.",
                    Site = "UserAccessEntraProvider.ReconcileUserMembershipsAsync",
                    ErrorKind = "NotFound"
                });
                return response;
            }

            var userGroups = await GetGroupsForUser(payload.UserId);
            string?[] userGroupNames = userGroups
                ?.Select(g=> g.DisplayName)
                ?.ToArray()??Array.Empty<string>();

            // Now, we need to determine:  Which groups need to be added
            // and which groups need to be removed.
            var groupsNamesToAdd = Reconciler.GetMissingMembers(payload.ExpectedGroups, userGroupNames);
            var groupNamesToRemove = Reconciler.GetMissingMembers(userGroupNames, payload.ExpectedGroups);

            // Now we need to get the GroupIds for the two lists.
            List<string> groupIdsToJoin = allGroups?.Value
                ?.Where(g=> g.Id!=null && groupsNamesToAdd.Contains(g.DisplayName))
                ?.Select(g=> g.Id!)
                ?.ToList()??new List<string>();

            List<string> groupIdsToLeave = allGroups?.Value
                ?.Where(g=> g.Id!=null && groupNamesToRemove.Contains(g.DisplayName))
                ?.Select(g=> g.Id!)
                ?.ToList()??new List<string>();

            if(groupIdsToJoin.Any())
            {
                response.MembershipsAdded = await AddUserToGroups(payload.UserId, groupIdsToJoin);
            }

            if(groupIdsToLeave.Any())
            {
                response.MembershipsRemoved = await RemoveUserFromGroups(payload.UserId, groupIdsToLeave);
            }

            response.Completed = true;
        }
        catch (Exception ex)
        {
            string errorMessage = $"An exception occurred while reconciling memberships for user {request.Payload?.UserId}.";
            _logger?.LogError(ex, errorMessage);
            ServiceError processFailure = new()
            {
                Severity = ErrorSeverity.Error,
                Message = errorMessage,
                Site = $"{nameof(UserAccessEntraProvider)}.{nameof(ReconcileUserMembershipsAsync)}",
                ErrorKind = $"{ServiceErrorKinds.StepFailed}.{IdentityServiceConstants.ErrorKinds.ExternalApiError}"
            };
            response.AddError(processFailure);
            response.Completed = false;
        }

        return response;
    }

    private async Task<List<Group>> GetGroupsForUser(string userId)
    {
        var groups = await MsGraph.Users[userId]
                        .MemberOf
                        .GetAsync();

        List<Group> userAppGroups = new();
        if (groups?.Value?.Any() ?? false)
        {
            foreach (var group in groups.Value)
            {
                var grp = group as Group;
                if (grp != null
                   && (grp?.DisplayName?.StartsWith(EntraGroups.AppGroupPrefix) ?? false))
                {
                    userAppGroups.Add(grp);
                }
            }
        }
        return userAppGroups;
    }

    private async Task<int> AddUserToGroups(string userIdToAdd, IEnumerable<string> groupNames)
    {
        int added = 0;
        string oDataRef = $"https://graph.microsoft.com/v1.0/users/{userIdToAdd}";
        foreach (var group in groupNames)
        {
            ReferenceCreate rc = new ReferenceCreate
            {
                OdataId = oDataRef
            };

            await MsGraph.Groups[group].Members.Ref
                .PostAsync(rc);
            added++;
        }
        return added;
    }

    private async Task<int> RemoveUserFromGroups(string userId, IEnumerable<string> groups)
    {
        int removed = 0;
        foreach (var group in groups)
        {
            await MsGraph.Groups[group]
                .Members[userId]
                .Ref
                .DeleteAsync();
            removed++;
        }
        return removed;
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
