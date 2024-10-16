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
### Receive SubscriptionPaused (sic) event in WebHook
1. Webhook receives Event from External Service
2. Webhook verifies the sender of ther Event.
 1. If sender is not verified, return either HTTP400 or HTTP403 (per Third Party Service's Documentation)
 2. If Sender IS verified, validate that the Event Data is well-formed, and correctly formatted.
  1. If Event Data is NOT well-formed, return HTTP400 to EcomService (need to confirm this with their Documentation)
  2. If Event Data IS Valid, forward to the Administrative API

### Receive SubscriptionPaused event at the Administrative API

### Send PauseSubscription command from Admin System

### Footnotes:
1. If possible via the Ecom Service, limit the duration that a Subscriptino can be paused.  Also limit the number of times that a subscription may be paused during a calendar year.
