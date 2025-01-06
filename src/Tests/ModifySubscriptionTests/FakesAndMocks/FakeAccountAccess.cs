using System;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.iFX;
using DevDad.SaaSAdmin.UserAccountAccess.Abstractions;
using ThatDeveloperDad.iFX.ServiceModel;
using Consts = ModifySubscriptionTests.FakesAndMocks.TestConstants;

namespace ModifySubscriptionTests.FakesAndMocks;

public class FakeAccountAccess : IUserAccountAccess
{
    // Fake backing store for the UserAccountResource objects.
    private Dictionary<string, UserAccountResource?> _accounts = new Dictionary<string, UserAccountResource?>();

    public FakeAccountAccess()
    {
        PopulateFakeAccounts();
    }

    public Task<UserAccountResource?> LoadUserAccountAsync(string accountId)
    {
        var foundIt = _accounts.TryGetValue(accountId, out UserAccountResource? account);
        
        return Task.FromResult(account);
    }

    public Task<(UserAccountResource?, ServiceError?)> SaveUserAccountAsync(UserAccountResource userAccount)
    {
        _accounts[userAccount.UserId] = userAccount;
        (UserAccountResource?, ServiceError?) result = (userAccount, null);
        return Task.FromResult(result);
    }

    private void PopulateFakeAccounts()
    {
        _accounts.Add(Consts.AccountIds.UnknownUser, null);

        _accounts.Add(Consts.AccountIds.NewIdNoProfile, null);

        _accounts.Add(
            Consts.AccountIds.FreeUser, 
            new UserAccountResource()
            {
                UserId = Consts.AccountIds.FreeUser,
                SubscriptionStatus = "Active",
                Subscription = new AccountSubscriptionResource()
                {
                    UserId = Consts.AccountIds.FreeUser,
                    SKU = SubscriptionIdentifiers.SKUS_TDMF_FREE,
                    StartDateUtc = DateTime.UtcNow.AddMonths(-2),
                    EndDateUtc = DateTime.UtcNow.AddMonths(-2),
                    WillRenew = true,
                    CurrentStatus = "Active",
                    Quotas = new UserQuotaResource()
                    {
                        UserId = Consts.AccountIds.FreeUser,
                        AiGenerations = new AppResourceQuota()
                        {
                            MeteredResource = MeteredResourceKinds.NpcAiDetail,
                            Budget = 0,
                            Consumption = 0
                        },
                        Storage = new AppResourceQuota()
                        {
                            MeteredResource = MeteredResourceKinds.NpcStorage,
                            Budget = 5,
                            Consumption = 0
                        },
                    },
                    History = new List<SubscriptionActivityResource>()
                    {
                        new SubscriptionActivityResource()
                        {
                            ActivityKind = SubscriptionChangeKinds.ActivityKind_Create,
                            ActivityDateUTC = DateTime.UtcNow.AddMonths(-2),
                            Comment = "Free Tier Subscription Created"
                        }
                    }
                },
                ExternalIds = new List<UserIdResource>()
                {
                    new UserIdResource()
                    {
                        UserIdAtVendor = Consts.AccountIds.FreeUser,
                        VendorName = Consts.FakeIdentityVendor
                    }
                }
            });

        _accounts.Add(
            Consts.AccountIds.ActivePaid, 
            new UserAccountResource()
            {
                UserId = Consts.AccountIds.ActivePaid,
                SubscriptionStatus = "Active",
                Subscription = new AccountSubscriptionResource()
                {
                    UserId = Consts.AccountIds.ActivePaid,
                    SKU = SubscriptionIdentifiers.SKUS_TDMF_PAID_MONTHLY,
                    StartDateUtc = DateTime.UtcNow.AddDays(-58),
                    EndDateUtc = DateTime.UtcNow.AddDays(2),
                    WillRenew = true,
                    CurrentStatus = "Active",
                    Quotas = new UserQuotaResource()
                    {
                        UserId = Consts.AccountIds.ActivePaid,
                        AiGenerations = new AppResourceQuota()
                        {
                            MeteredResource = MeteredResourceKinds.NpcAiDetail,
                            Budget = 100,
                            Consumption = 68
                        },
                        Storage = new AppResourceQuota()
                        {
                            MeteredResource = MeteredResourceKinds.NpcStorage,
                            Budget = 100,
                            Consumption = 54
                        },
                    },
                    History = new List<SubscriptionActivityResource>()
                    {
                        new SubscriptionActivityResource()
                        {
                            ActivityKind = SubscriptionChangeKinds.ActivityKind_Create,
                            ActivityDateUTC = DateTime.UtcNow.AddDays(-58),
                            Comment = "Paid Subscription Created"
                        },
                        new SubscriptionActivityResource()
                        {
                            ActivityKind = SubscriptionChangeKinds.ActivityKind_Renew,
                            ActivityDateUTC = DateTime.UtcNow.AddDays(-28),
                            Comment = "Subscription Renewed"
                        }
                    }
                },
                ExternalIds = new List<UserIdResource>()
                {
                    new UserIdResource()
                    {
                        UserIdAtVendor = Consts.AccountIds.ActivePaid,
                        VendorName = Consts.FakeIdentityVendor
                    }
                }
            });

        _accounts.Add(
            Consts.AccountIds.ExpiredPaid, 
            new UserAccountResource()
            {
                UserId = Consts.AccountIds.ExpiredPaid,
                SubscriptionStatus = SubscriptionStatuses.Expired,
                Subscription = new AccountSubscriptionResource()
                {
                    UserId = Consts.AccountIds.ExpiredPaid,
                    SKU = SubscriptionIdentifiers.SKUS_TDMF_PAID_MONTHLY,
                    StartDateUtc = DateTime.UtcNow.AddDays(-62),
                    EndDateUtc = DateTime.UtcNow.AddDays(-2),
                    WillRenew = true,
                    CurrentStatus = SubscriptionStatuses.Expired,
                    Quotas = new UserQuotaResource()
                    {
                        UserId = Consts.AccountIds.ExpiredPaid,
                        AiGenerations = new AppResourceQuota()
                        {
                            MeteredResource = MeteredResourceKinds.NpcAiDetail,
                            Budget = 100,
                            Consumption = 68
                        },
                        Storage = new AppResourceQuota()
                        {
                            MeteredResource = MeteredResourceKinds.NpcStorage,
                            Budget = 100,
                            Consumption = 54
                        },
                    },
                    History = new List<SubscriptionActivityResource>()
                    {
                        new SubscriptionActivityResource()
                        {
                            ActivityKind = SubscriptionChangeKinds.ActivityKind_Create,
                            ActivityDateUTC = DateTime.UtcNow.AddDays(-62),
                            Comment = "Paid Subscription Created"
                        },
                        new SubscriptionActivityResource()
                        {
                            ActivityKind = SubscriptionChangeKinds.ActivityKind_Renew,
                            ActivityDateUTC = DateTime.UtcNow.AddDays(-32),
                            Comment = "Subscription Renewed"
                        }
                    }
                },
                ExternalIds = new List<UserIdResource>()
                {
                    new UserIdResource()
                    {
                        UserIdAtVendor = Consts.AccountIds.ExpiredPaid,
                        VendorName = Consts.FakeIdentityVendor
                    }
                }
            });

        _accounts.Add(
            Consts.AccountIds.CancelledPaid, 
            new UserAccountResource()
            {
                UserId = Consts.AccountIds.CancelledPaid,
                SubscriptionStatus = SubscriptionStatuses.Cancelled,
                Subscription = new AccountSubscriptionResource()
                {
                    UserId = Consts.AccountIds.CancelledPaid,
                    SKU = SubscriptionIdentifiers.SKUS_TDMF_PAID_MONTHLY,
                    StartDateUtc = DateTime.UtcNow.AddDays(-62),
                    EndDateUtc = DateTime.UtcNow.AddDays(-2),
                    WillRenew = false,
                    CurrentStatus = SubscriptionStatuses.Cancelled,
                    Quotas = new UserQuotaResource()
                    {
                        UserId = Consts.AccountIds.CancelledPaid,
                        AiGenerations = new AppResourceQuota()
                        {
                            MeteredResource = MeteredResourceKinds.NpcAiDetail,
                            Budget = 0,
                            Consumption = 68
                        },
                        Storage = new AppResourceQuota()
                        {
                            MeteredResource = MeteredResourceKinds.NpcStorage,
                            Budget = 0,
                            Consumption = 54
                        },
                    },
                    History = new List<SubscriptionActivityResource>()
                    {
                        new SubscriptionActivityResource()
                        {
                            ActivityKind = SubscriptionChangeKinds.ActivityKind_Create,
                            ActivityDateUTC = DateTime.UtcNow.AddDays(-62),
                            Comment = "Paid Subscription Created"
                        },
                        new SubscriptionActivityResource()
                        {
                            ActivityKind = SubscriptionChangeKinds.ActivityKind_Renew,
                            ActivityDateUTC = DateTime.UtcNow.AddDays(-32),
                            Comment = "Subscription Renewed"
                        },
                        new SubscriptionActivityResource()
                        {
                            ActivityKind = SubscriptionChangeKinds.ActivityKind_Cancel,
                            ActivityDateUTC = DateTime.UtcNow.AddDays(-2),
                            Comment = "Subscription Cancelled"
                        }
                    }
                },
                ExternalIds = new List<UserIdResource>()
                {
                    new UserIdResource()
                    {
                        UserIdAtVendor = Consts.AccountIds.CancelledPaid,
                        VendorName = Consts.FakeIdentityVendor
                    }
                }
            });
    }
}
