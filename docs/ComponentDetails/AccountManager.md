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
 * Accepts the inbound data for a CustomerSubcription manipulation and an ActivityContext identifier.
 * Validates that inbound data against the necessary Current states for the objects being manipulated.
 * Runs whichever Strategy is identified by the Activity Context.
   * Known ActivityContexts:
     * ProvisionSubscription
     * RenewSubscription
     * PauseSubscription
     * ResumeSubscription
     * DiscontinueSubscription
     * CancelSubscription

## Nerd Stuff
### Namespaces
 * TddSaasAdmin.Managers.CustomerAccount
 

### Interfaces
 * ICustomerAccountManager
