using System;
using DevDad.SaaSAdmin.iFX;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using DevDad.SaaSAdmin.UserIdentity.EntraB2C;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace TestConsole.OtherTests;

public class EntraAccessTests
{
    private MsGraphOptions msGraphOptions;
    private IUserIdentityAccess _entraProvider;
    private ILogger? _logger;
    private ILoggerFactory? _loggerFactory;

    public EntraAccessTests(IConfiguration config, ILogger logger)
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        _logger = logger;
        msGraphOptions = new MsGraphOptions();
        var graphSettings = config.GetSection("CustomerAccountManager:IUserIdentityAccess");
        graphSettings.Bind(msGraphOptions);

        _entraProvider = new UserAccessEntraProvider(msGraphOptions, _loggerFactory);
    }

    public void RunTests()
    {
        string myId = "a0b66013-a5ef-462f-a812-3eb4aeacff66";
        string geekDadId = "eb4668e2-941a-480b-b132-d9300e9e6124";

        string[] paidGroupList = new string[]
        {
            EntraGroups.FreeUsers,
            EntraGroups.PaidUsers
        };

        string[] freeGroups = new string[]
        {
            EntraGroups.FreeUsers
        };
        // Test 1:  Check MY groups against the paid user groups.
        // should pass OK, because there should be no changes.
        ReconcileGroups(myId, paidGroupList);

        // Test 2:  use GeekDad as the test account.
        ReconcileGroups(geekDadId, paidGroupList);
        
        // Reconcile against the "free Only" group list.
        ReconcileGroups(geekDadId, freeGroups);

    }

    public void ReconcileGroups(string userId, string[] expectedGroups)
    {
        _logger?.LogInformation("Reconciling Groups for User: {userId}", userId);
        ReconcileMembershipsData requestData = new()
        {
            UserId = userId,
            ExpectedGroups = expectedGroups
        };
        ReconcileMembershipsRequest request = new("explorationTesting", requestData);
        ReconcileMembershipsResponse response = _entraProvider.ReconcileUserMembershipsAsync(request).Result;
        
        if(response.Payload == false)
        {
            _logger?.LogError("ReconcileMemberships failed for User: {userId}", userId);
            foreach(var error in response.ErrorReport)
            {
                _logger?.LogError(error);
            }

            return;
        }
        
        _logger?.LogInformation("ReconcileMembershipsResponse finished: {response}", response.Payload);
    }

}
