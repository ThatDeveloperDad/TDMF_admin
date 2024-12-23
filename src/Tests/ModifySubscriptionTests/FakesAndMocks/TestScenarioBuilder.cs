using System;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.iFX;
using Consts = ModifySubscriptionTests.FakesAndMocks.TestConstants;

namespace ModifySubscriptionTests.FakesAndMocks;

public class TestScenarioBuilder
{
    private ManageSubscriptionRequest _scenario;
    private SubscriptionActionDetail _actionDetail;

    public TestScenarioBuilder()
    {
        _scenario = new ManageSubscriptionRequest("UnitTesting");
        _actionDetail = new SubscriptionActionDetail();
        _actionDetail.VendorName = ExternalServiceVendors.ThatDeveloperDad;
    }

    public ManageSubscriptionRequest Build()
    {
        _scenario.Payload = _actionDetail;
        return _scenario;
    }

    public TestScenarioBuilder CreateFree()
    {
        _actionDetail.ActionName = SubscriptionChangeKinds.ActivityKind_Create;
        _actionDetail.SubscriptionSku = SubscriptionIdentifiers.SKUS_TDMF_FREE;
        _actionDetail.RequestSource = ChangeRequestSource.AdminBackend;

        return this;
    }

    public TestScenarioBuilder CreatePaid()
    {
        _actionDetail.ActionName = SubscriptionChangeKinds.ActivityKind_Create;
        _actionDetail.SubscriptionSku = SubscriptionIdentifiers.SKUS_TDMF_PAID_MONTHLY;
        _actionDetail.RequestSource = ChangeRequestSource.AdminBackend;

        return this;
    }

    public TestScenarioBuilder RenewFree()
    {
        _actionDetail.ActionName = SubscriptionChangeKinds.ActivityKind_Renew;
        _actionDetail.SubscriptionSku = SubscriptionIdentifiers.SKUS_TDMF_FREE;
        _actionDetail.RequestSource = ChangeRequestSource.AdminBackend;

        return this;
    }

    public TestScenarioBuilder RenewPaid()
    {
        _actionDetail.ActionName = SubscriptionChangeKinds.ActivityKind_Renew;
        _actionDetail.SubscriptionSku = SubscriptionIdentifiers.SKUS_TDMF_PAID_MONTHLY;
        _actionDetail.RequestSource = ChangeRequestSource.AdminBackend;

        return this;
    }

    public TestScenarioBuilder CancelFree()
    {
        _actionDetail.ActionName = SubscriptionChangeKinds.ActivityKind_Cancel;
        _actionDetail.SubscriptionSku = SubscriptionIdentifiers.SKUS_TDMF_FREE;
        _actionDetail.RequestSource = ChangeRequestSource.AdminBackend;

        return this;
    }

    public TestScenarioBuilder CancelPaid()
    {
        _actionDetail.ActionName = SubscriptionChangeKinds.ActivityKind_Cancel;
        _actionDetail.SubscriptionSku = SubscriptionIdentifiers.SKUS_TDMF_PAID_MONTHLY;
        _actionDetail.RequestSource = ChangeRequestSource.AdminBackend;

        return this;
    }

    public TestScenarioBuilder WithUnknownUser()
    {
        _actionDetail.CustomerProfileId = Consts.AccountIds.UnknownUser;
        _actionDetail.VendorSuppliedCustomerId = Consts.AccountIds.UnknownUser;
        return this;
    }

    public TestScenarioBuilder WithBrandNewUser()
    {
        _actionDetail.CustomerProfileId = Consts.AccountIds.NewIdNoProfile;
        _actionDetail.VendorSuppliedCustomerId = Consts.AccountIds.NewIdNoProfile;
        return this;
    }

    public TestScenarioBuilder WithFreeUser()
    {
        _actionDetail.CustomerProfileId = Consts.AccountIds.FreeUser;
        _actionDetail.VendorSuppliedCustomerId = Consts.AccountIds.FreeUser;
        return this;
    }

    public TestScenarioBuilder WithActivePaidUser()
    {
        _actionDetail.CustomerProfileId = Consts.AccountIds.ActivePaid;
        _actionDetail.VendorSuppliedCustomerId = Consts.AccountIds.ActivePaid;
        return this;
    }

    public TestScenarioBuilder WithExpiredPaidUser()
    {
        _actionDetail.CustomerProfileId = Consts.AccountIds.ExpiredPaid;
        _actionDetail.VendorSuppliedCustomerId = Consts.AccountIds.ExpiredPaid;
        return this;
    }

    public TestScenarioBuilder WithCancelledPaidUser()
    {
        _actionDetail.CustomerProfileId = Consts.AccountIds.CancelledPaid;
        _actionDetail.VendorSuppliedCustomerId = Consts.AccountIds.CancelledPaid;
        return this;
    }
}
