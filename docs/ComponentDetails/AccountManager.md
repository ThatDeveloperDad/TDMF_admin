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
  * Adds the user to the "Paid Users" group in Entra.
  * Enqueue a "Thank you for choosing The DM's Familiar" email.
* RenewSubscription
  * Validates the request data for correctness against the Customer and their Subscription.
  * Updates the ActiveThrough date.
  * Performs what Quota Resets are specified by the SubscirptionTemplate for their current Sub.
  * Enqueue a "Thank you for your continued support" email.
* PauseSubscription
  * Validates the request for Correctness against the Customer's current state.
  * Sets the ActiveThrough date to moment of execution.
  * Removes the Customer from the PaidUsers group in Entra.
  * Stores the remaining Days, and Quota Amounts with their subscription so that they can be restored when the Subscription time Resumes.
  * Sets Any current Quotas to read as "fully consumed"
  * Enqueue a "Have fun storming the castle!" email w/ Pre-selected Resume date, and explanation of how "Pausing" the subscriptino affects their access.
* ResumeSubscription
  * Validate the request data for correctness against the current Customer and Subscription States.
  * Retrieve the "What's Left" information for the Subscription Days and any attached Quotas.
  * Update the subscirption ActiveThough date to Moment of Execution + DaysRemaining.
  * Update the quotas to restore their state when the subscription was Paused.
  * Add the user to the "Paid Users" group in Entra.
  * Enqueue a "Welcome Back" email w/ new Billing date, and current quota amounts.
* DiscontinueSubscription
 * Validate the Request Data for correctness against the current Custoemr & Subscription states.
 * Set the "will renew" flag on the subcription to false.
 * Send a "We're going to miss you!" email with an explanation of what will happen when, and what access the custoemr will retain after their current paid time elapses.
* CancelSubscription
 * Validate the request against current state.
 * Set the ActiveThrough date to end of day for moment of execution.
 * Sets any Quotas to "Fully Consumed"
 * Removes the user from the "Paid Users" group in Entra.
 * Send a "We're sorry to see you go, maybe we can still be friends" email explaining that their Paid access has been removed, but they will retain read access to anything they've created for a period of time.
* CalculateProrate
 * *Will I need this?*
 * Spec this out IF REQUIRED.
