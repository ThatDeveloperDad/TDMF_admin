using System.Text;
using DevDad.SaaSAdmin.AccountManager;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.Catalog.Abstractions;
using DevDad.SaaSAdmin.Catalog.HardCoded;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;
using ModifySubscriptionTests.FakesAndMocks;

namespace ModifySubscriptionTests;

public class ModifySubscriptionUnitTests
{
    


    private IAccountManager ConfigureTestInstance()
    {
        var identityService = new FakeIdentityAccess();
        var accountService = new FakeAccountAccess();
        var catalogService = new StaticCatalogProvider();
        var loggerFactory = LoggerFactory.Create(builder => 
        {
            builder.AddConsole();
        });

        return new CustomerAccountManager(loggerFactory, identityService, accountService, catalogService);
    }

#region CreateSubscription Senarios
    [Fact]
    public void TestCreateFree_UnknownUser_ExpectValidationFailure()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CreateFree()
            .WithUnknownUser()
            .Build();
        string expectedError = IdentityServiceConstants.ErrorKinds.UnknownIdentity;

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

    [Fact]
    public void TestCreatePaid_UnknownUser_ExpectValidationFailure()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CreatePaid()
            .WithUnknownUser()
            .Build();
        string expectedError = IdentityServiceConstants.ErrorKinds.UnknownIdentity;

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

    [Fact]
    public void TestCreateFree_NewUser_ExpectValidationError()
    {
        // When a new Identity arrives in the system for the first time,
        // There is no existing Profile.
        // In this case, a new Profile is created, with a Free Susbcription
        // added automatically upon creation.
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CreateFree()
            .WithBrandNewUser()
            .Build();
        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_AddNewSameSku;
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

    [Fact]
    public void TestCreatePaid_NewUser_ExpectSuccess()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CreatePaid()
            .WithBrandNewUser()
            .Build();
        
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == true);
    }

    [Fact]
    public void TestCreateFree_FreeUser_ExpectValidationFailure()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CreateFree()
            .WithFreeUser()
            .Build();
        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_AddNewSameSku;

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

    [Fact]
    public void TestCreateFree_CancelledUser_ExpectSuccess()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CreateFree()
            .WithCancelledPaidUser()
            .Build();
        
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == true);
    }

    [Fact]
    public void TestCreatePaid_CancelledUser_ExpectSuccess()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CreatePaid()
            .WithCancelledPaidUser()
            .Build();
        
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == true);
    }

    [Fact]
    public void TestCreatePaid_FreeUser_ExpectSuccess()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CreatePaid()
            .WithFreeUser()
            .Build();
        
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == true);
    }


    [Fact]
    public void TestCreateFree_ExpiredPaidUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CreateFree()
            .WithExpiredPaidUser()
            .Build();
        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForStatus;
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

#endregion // Create scenarios

#region RenewSubscription Scenarios

    [Fact]
    public void TestRenewFree_UnknownUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .RenewFree()
            .WithUnknownUser()
            .Build();
        string expectedError = IdentityServiceConstants.ErrorKinds.UnknownIdentity;

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

    [Fact]
    public void TestRenewPaid_ActivePaidUser_ExpectSuccess()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .RenewPaid()
            .WithActivePaidUser()
            .Build();
        
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == true);
    }

    [Fact]
    public void TestRenewPaid_ExpiredPaidUser_ExpectSuccess()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .RenewPaid()
            .WithExpiredPaidUser()
            .Build();
        
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == true);
    }

    [Fact]
    public void TestRenewPaid_CancelledPaidUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .RenewPaid()
            .WithCancelledPaidUser()
            .Build();
        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForStatus;
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }
    
    // Once suspendSubscription is implemented, Add test for renew Suspended user - should fail.

#endregion // RenewSubscription Scenarios

#region Expire scenarios

    [Fact]
    public void TestExpireFree_UnknownUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .ExpireFree()
            .WithUnknownUser()
            .Build();
        string expectedError = IdentityServiceConstants.ErrorKinds.UnknownIdentity;

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

    [Fact]
    public void TestExpireFree_NewUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .ExpireFree()
            .WithBrandNewUser()
            .Build();
        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForSubscriptionType;

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

    [Fact]
    public void TestExpireFree_FreeUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .ExpireFree()
            .WithFreeUser()
            .Build();
        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForSubscriptionType;

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        StringBuilder sb = new();
        foreach(string? error in response.ErrorReport)
        {
            sb.AppendLine(error);
        }

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError), sb.ToString());
    }

    [Fact]
    public void TestExpire_ActivePaidUser_ExpectSuccess()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .ExpirePaid()
            .WithActivePaidUser()
            .Build();
        
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == true);
    }

    [Fact]
    public void TestExpire_ExpiredPaidUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .ExpirePaid()
            .WithExpiredPaidUser()
            .Build();
        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForStatus;
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

    [Fact]
    public void TestExpire_CancelledPaidUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .ExpirePaid()
            .WithCancelledPaidUser()
            .Build();
        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForStatus;
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

    // Add test for Expire Suspended user - should fail.
    // Don't do this until we implement Suspend - we may not.

#endregion // Expire scenarios

#region Cancel scenarios
    
    [Fact]
    public void TestCancelFree_UnknownUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CancelFree()
            .WithUnknownUser()
            .Build();
        string expectedError = IdentityServiceConstants.ErrorKinds.UnknownIdentity;

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError));
    }

    [Fact]
    public void TestCancelFree_NewUser_ExpectValidationError()
    {
        //Arrange
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CancelFree()
            .WithBrandNewUser()
            .Build();
        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForSubscriptionType;

        //Act
        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        // Get error messages into a string for easier debugging.
        StringBuilder sb = new();
        foreach(string? error in response.ErrorReport)
        {
            sb.AppendLine(error);
        }

        // Assert
        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError), sb.ToString());
    }

    [Fact]
    public void TestCancelFree_FreeUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CancelFree()
            .WithFreeUser()
            .Build();
        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForSubscriptionType;

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        // Get error messages into a string for easier debugging.
        StringBuilder sb = new();
        foreach(string? error in response.ErrorReport)
        {
            sb.AppendLine(error);
        }

        // Assert
        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError), sb.ToString());
    }

    [Fact]
    public void TestCancel_ActivePaidUser_ExpectSuccess()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CancelPaid()
            .WithActivePaidUser()
            .Build();

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == true);
        Assert.True(response.HasErrors == false);
    }

    [Fact]
    public void TestCancel_ExpiredPaidUser_ExpectSuccess()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CancelPaid()
            .WithExpiredPaidUser()
            .Build();

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        // Assert
        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == true);
        Assert.True(response.HasErrors == false);
    }

    [Fact]
    public void TestCancel_CancelledPaidUser_ExpectValidationError()
    {
        IAccountManager managerToTest = ConfigureTestInstance();
        var request = new TestScenarioBuilder()
            .CancelPaid()
            .WithCancelledPaidUser()
            .Build();

        string expectedError = AccountManagerConstants.ModifySubscriptionErrors.Validation_ActivityNotValidForStatus;

        var response = managerToTest.ManageCustomerSubscriptionAsync(request)?.Result;

        // Get error messages into a string for easier debugging.
        StringBuilder sb = new();
        foreach(string? error in response.ErrorReport)
        {
            sb.AppendLine(error);
        }

        // Assert
        Assert.NotNull(response);
        Assert.True(response.SubscriptionWasUpdated == false);
        Assert.True(ResponseContainsError(response, expectedError), sb.ToString());
    }

# endregion // Cancel scenarios

    private bool ResponseContainsError(ManageSubscriptionResponse response, string errorMessage)
    {
        bool errorIsPresent = response.ErrorReport.Any(er=> er!=null &&  er.Contains(errorMessage));
        return errorIsPresent;
    }

}
