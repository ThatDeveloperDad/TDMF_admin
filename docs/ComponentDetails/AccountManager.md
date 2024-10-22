# AccountManager
## Purpose
This component will use the internal engines and resource access components to compose and control the different Activities that encompass Subscirption and Account management across the different systems engaged in providing the Product. (The DM's Familiar)

## Behaviors
* CreateProfile
  * Sets up a User Profile for a newly signed up user.
  * Adds that user to the "Free" group in Entra.
* SaveProfileChanges
  * Saves any changes to the User's Profile within The DM's Familiar.
  * This data will be limited, and include only things like:
    * DisplayName
    * Any Ids used by external services
    * We will avoid storing any "formal" PII in the ProductDB.  As much PII that can be hidden away from MY code inside MS-Entra should be.
* LoadProfile
  * Deferred.  Only needed for an AdminUI program.
  
* StartSubscription
  * Loads the identified Customer & SubscriptionTemplate.  Validates that the incoming data in the request is Correct for the StartSubscription Activity.
  * Creates a new CustomerSubscription record with the inbound data.
  * Provisions quotas as specified by the SubscriptionTemplate for the identified SKU in the request.
* RenewSubscription
  * Validates the request data for correctness against the Customer and their Subscription.
  * Updates the ActiveThrough date.
  * Performs what Quota Resets are specified by the SubscirptionTemplate for their current Sub.
* PauseSubscription
  * Validates the request for Correctness against the Customer's current state.
  * Sets the ActiveThrough date to moment of execution.
  * Removes the Customer from the PaidUsers group in Entra.
  * Stores the remaining Days, and Quota Amounts with their subscription so that they can be restored when the Subscription time Resumes.
  * Sets Any current Quotas to read as "fully consumed"
* ResumeSubscription
  * Validate the request data for correctness against the current Customer and Subscription States.
* DiscontinueSubscription
* CancelSubscription
* CalculateProrate
