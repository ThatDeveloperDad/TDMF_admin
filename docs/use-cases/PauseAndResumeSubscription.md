# Pause and Resume Subscription
[Back](/docs/UseCases.md "Back to Use Case List")

---

## Process:
1.  Pause / Resume subscription process is always initiated by a Person. (Either Customer or Administrator)
  *  The Person navigates to their Subscription Management UI (Either within Ecom Service or Admin System)
  *  Enters the date to pause and (optionally) the date to resume. <sup>1</sup>
  *  Completes the interaction by submitting the form.
2. The data is sent to the Ecom Service's backend system where it is processed.
3. Ecom System sends an Event Message to the Admin System via Webhook invocation.
4. AdminSystem updates that Person's local account details.
5. During the "Paused" time, if the Person logs in, they will not be able to use any of the "Paid" features of the application.

## Activities:
### Receive SubscriptionPaused (sic) event at WebHook
1. Webhook receives Event from External Service
2. Webhook verifies the sender of ther Event.
 1. If sender is not verified, return either HTTP400 or HTTP403 (per Third Party Service's Documentation)
 2. If Sender IS verified, validate that the Event Data is well-formed, and correctly formatted.
  1. If Event Data IS NOT well-formed, return HTTP400 to EcomService (need to confirm this with their Documentation)
  2. If Event Data IS Valid, forward to the Administrative API

### Receive SubscriptionPaused event at the Administrative API<sup>2</sup>
1. Validate that the Data in the Request is Correct for the affected Customer and Product.
 1. If Not Valid, Log & (Do something with the ECom System to roll back that change.  Review Docs for details.)
 2. If Valid, Update the Customer's Subscription & Quota records in the Product's local database.
  * Set Subscription Status to Paused.
  * Set Subscription Renewal Date to NULL  (maybe)
  * Need to make the Product code aware that a subscription may have different Statuses.
   * Must block access to Paid Features during the "Paused" time.
2. Send an email to the Customer notifying them that their subscription has been Paused, will Resume on (DATE), and that their new Billing Date will be (Recalculated Billing Date)

### Receive SubscriptionResumed (sic) event at WebHook
1. Same steps as for SubscriptionPaused.
2. Upon verification and validation, forward to appropriate Endpoint on the Administrative API

### Receive SubscriptionResumed event at Administrative API
1. Validate that the Data in the Request is Correct for the requested operation on the supplied Customer & Subscription identifiers.
2. Calculate the new billing date for the Subscription Renewal date.
3. Update the Subscription Status to be Active once more.
4. Restore the Quota Data to the values it had when it was paused.

### Send PauseSubscription command from Admin System
1. Find Customer, Identify Subscription to be Paused.
2. Send a request to the E-Commerce System's API to "pause" the identified subscription.

### Footnotes:
1. If possible via the Ecom Service, limit the duration that a Subscription can be paused.  Also limit the number of times that a subscription may be paused during a year.  (Need to define year.  Calendar year, or trailing 12-month period?)
2. Receiving Events at the Administrative API does not necessarily mean we are implementing a Message Bus system.  It's an unfortunate ambiguity of English that suggests that.  For now, the intetion is an HTTP Call.  This may change during Architecture & Detailed Design phases of the Planning work.
