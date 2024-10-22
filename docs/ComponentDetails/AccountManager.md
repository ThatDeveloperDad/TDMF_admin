# AccountManager

## Purpose
This component composes the programmed Tasks and Steps into Business Activities that affect a Customer's Profile and Subscription.

## Normal Person Description

### Behaviors
* **CreateCustomerProfile**
  * Sets up a User Profile for a newly signed up user.
  * Adds that user to the "Free" group in Entra.
* **SaveCustomerProfileChanges**
  * Saves any changes to the User's Profile within The DM's Familiar.
  * This data will be limited, and include only things like:
    * DisplayName
    * Any Ids used by external services
    * We will avoid storing any "formal" PII in the ProductDB.  As much PII that can be hidden away from MY code inside MS-Entra should be.
* **LoadCustomerProfile**
  * Deferred.  Only needed for an AdminUI program.

* **ManageCustomerSubscription**
  * Accepts the inbound data for a CustomerSubcription manipulation and an SubscriptionAction identifier.
  * Validates that inbound data against the necessary Current states for the objects being manipulated.
  * Runs whichever Strategy is identified by the Activity Context.
    * Currently Known SubscriptionActions:
      * Provision
      * Renew
      * Pause
      * Resume
      * Discontinue
      * Cancel

---
## Nerd Stuff
### Project Name
**TssSaasAdmin.Managers.CustomerAccount**

### Project Type:  
**C# Class Library**  
**.Net 8**

### Namespaces
 * TddSaasAdmin.Managers.CustomerAccount (root)
   * We'll put the default AccountManager implementation here.
 * TddSaasAdmin.Managers.CustomerAccount.Public (folder; ONLY public interfaces and class definitions)
   * This folder will hold any interfaces or Data Classes that specify the protocols to be used by calling components.
   * These protocols may include:  Data Transfer Objects (simple .Net POCOs) or ServiceContracts (.Net Interfaces)
 * TddSaasAdmin.Managers.CustomerAccount.Strategies (folder; ONLY internal interfaces & implementations)
   * We'll put the individual Strategy implementations for the different Subscription Actions in here.

### Interfaces
#### public ICustomerAccountManager

**Methods**
 * public (CustomerProfile?, Exception?) CreateCustomerProfile(CustomerData profileData)
   * Accepts a CustomerData object, and uses it to provision a new CustomerProfile.
   * Returns either the newly provisioned CustomerProfile, or an Exception.
 * public (CustomerProfile?, Exception?) StoreCustomerProfile(CustomerProfile profile)
   * Overwrites the values of any mutable properties on the CustomerProfile object in the various Resources.
   * Returns wither the post-updated Profile or an Exception.
 * public (CustomerSubscription?, Exception?) ManageCustomerSubscription(ActionKind managementAction, ActionData data)
   * validates the inbound data to ensure that its correct for the identified managementAction.
   * Forwards that data to the matching Strategy implementation.
   * Returns either the Updated CustomerSobscription State, or an Exception.

#### public enum ActionKind
*Note:  This concept is subject to change.  I'm not sure I like it.*
 * Identifies the different Actions that can be performed against the incoming SubscriptionData
 * Current ActionKinds include:
   * Provision
   * Renew
   * Pause
   * Resume
   * Discontinue
   * Cancel

### Implementations

#### public CustomerAccountManager
**Implements** ICustomerAccountManager


**Dependencies**
 * IValidator
 * ICustomerAccess
 * ISubscriptionAccess
 * IShopGateway
 * IProrater
 * ILoggerFactory
