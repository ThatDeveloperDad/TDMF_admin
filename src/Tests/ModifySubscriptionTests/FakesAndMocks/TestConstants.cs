using System;
using System.Net.NetworkInformation;

namespace ModifySubscriptionTests.FakesAndMocks;

public class TestConstants
{
    public const string FakeIdentityVendor = "TestingFake";
    public class AccountIds
    {
        public static string UnknownUser = "unknown";
        public static string NewIdNoProfile = "newIdNoProfile";
        public static string FreeUser = "freeUser";
        public static string ActivePaid = "activePaid";
        public static string ExpiredPaid = "expiredPaid";
        public static string CancelledPaid = "cancelledPaid";
    }
}
