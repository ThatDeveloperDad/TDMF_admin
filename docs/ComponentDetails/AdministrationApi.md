# Administration API

## Purpose
Exposes specific functionality for the system as callable endpoint methods that can be consumed by "external" systems.
NOTE:  This API *may* not need to be exposed to the public internet.  If it's not neccessary to do so, do not expose it.
ALSO:  Whether it's exposed to the Internet or not, DO require secure and correctly authenticated and authorized connections on these endpoints.

## Normal Person Documentation

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
  
* **StartCustomerSubscription**
  * Loads the identified Customer & SubscriptionTemplate.  Validates that the incoming data in the request is Correct for the StartSubscription Activity.
  * Creates a new CustomerSubscription record with the inbound data.
  * Provisions quotas as specified by the SubscriptionTemplate for the identified SKU in the request.
  * Adds the user to the "Paid Users" group in Entra.
  * Enqueue a "Thank you for choosing The DM's Familiar" email.
* **RenewCustomerSubscription**
  * Validates the request data for correctness against the Customer and their Subscription.
  * Updates the ActiveThrough date.
  * Performs what Quota Resets are specified by the SubscirptionTemplate for their current Sub.
  * Enqueue a "Thank you for your continued support" email.
* **PauseCustomerSubscription**
  * Validates the request for Correctness against the Customer's current state.
  * Sets the ActiveThrough date to moment of execution.
  * Removes the Customer from the PaidUsers group in Entra.
  * Stores the remaining Days, and Quota Amounts with their subscription so that they can be restored when the Subscription time Resumes.
  * Sets Any current Quotas to read as "fully consumed"
  * Enqueue a "Have fun storming the castle!" email w/ Pre-selected Resume date, and explanation of how "Pausing" the subscriptino affects their access.
* **ResumeCustomerSubscription**
  * Validate the request data for correctness against the current Customer and Subscription States.
  * Retrieve the "What's Left" information for the Subscription Days and any attached Quotas.
  * Update the subscirption ActiveThough date to Moment of Execution + DaysRemaining.
  * Update the quotas to restore their state when the subscription was Paused.
  * Add the user to the "Paid Users" group in Entra.
  * Enqueue a "Welcome Back" email w/ new Billing date, and current quota amounts.
* **DiscontinueCustomerSubscription**
  * Validate the Request Data for correctness against the current Custoemr & Subscription states.
  * Set the "will renew" flag on the subcription to false.
  * Send a "We're going to miss you!" email with an explanation of what will happen when, and what access the custoemr will retain after their current paid time elapses.
* **CancelCustomerSubscription**
  * Validate the request against current state.
  * Set the ActiveThrough date to end of day for moment of execution.
  * Sets any Quotas to "Fully Consumed"
  * Removes the user from the "Paid Users" group in Entra.
  * Send a "We're sorry to see you go, maybe we can still be friends" email explaining that their Paid access has been removed, but they will retain read access to anything they've created for a period of time.
* **CalculateCustomerProrate**
  * *Will I need this?*
  * Spec this out IF REQUIRED.

---
## Nerd Stuff

### Purpose:
Authenticates the incoming Request.
Analyzes the Request and forwards it to the appropriate Manager operation.

### Implementation:
**Technology** ASP .Net Core WebAPI.  (Likely using MinimalAPI)
**Hosting** Azure App Service(???)
**Exposure** Non-public to start.  MAY need to expose publicly, depending on what kind of UI Application I decide to roll.

 #### Direct Dependencies
 * Logging
 * OAuth for Authentication/Authorization
 * Configuration (use environment vars, rather than AppConfig.json strategy)
 * IAccountManager
   * Passes configuration data to the Manager on Dependency Resolution.
     * Manager is responsible for instantiating its own Dependencies, based on the provided Configuration.
   * Manager will expose a DI Constructor that is to be used only when the Operational Context of the API is NOT Production-Operation.
     * i.e.:  Test Scenarios. 
