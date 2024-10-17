# Cancel Subscription
[Back](/docs/UseCases.md "Back to Use Case List")  

**Canceling a Subscription involves a pro-rated refund of the most recent Payment and immediate revocation of access to Paid Features**
**Cancel should be an exceptional case.**

## Process
1. Cancel should be initiated only by a Customer or Product Administrator.
2. It can be initiated either through the E-Commerce Service's Account Portal, or the Administrative System.
3. The Prorate amount is calculated <sup>1</sup>
4. The Refund is made to the Customer by the ECom System.
5. Upon completion of that activity, the ECom Service will notify the Admin Service via WebHook.
6. The Admin Service will zero out any remaining quotas, immediately remove that user from the Paid Access groups.
7. Send a "We're sorry to see you go, but 

### Footnotes
1. Proration calculation should consider any Quotaed Usage during the period being pro-rated.
  * We don't want anyone signing up, using up their allotment of AI Tokens, then quitting and requesting a refund on the same day.
  * Need to research how we can get the ECom system to request that more complicated Prorate from our System. 

## Activities

### Ecom System has initated the Refund and notifies the Adminstrative System of the Cancellation via WebHook
1. WebHook code verifies the sender of the message.
  1. If NOT Verified, return error code to sender.
  2. If Verified, continue.
2. Validate that the Notification Data is Complete and Proerly Formatted. (Basic Validation)
  1. If Not Valid, return error code to sender.
  2. If Valid, Forward the Request to the Administrative API.

### CancelSubscription event arrives at Administrative API
1. Retrieve the identified User and Subscription data from storage.
2. Validate the request data against the user's current subscription state.
 * Make sure that the incoming data is "Correct" for making the requested change to the User's Subscription.
 * If the request data IS NOT correct, perform some kind of compensating activity against the E-Com System.
 * If the request Data IS correct, proceed with the Cancel Subscription Activity.
3. Update the Customer's Subscription to end TODAY.
   Zero out any remaining Quota allocations.
   Remove the customer from the IdP User Groups that allow access to the Paid Features of the application.
4. Send the "Sorry to see you go" email and explain that access has been revoked.   

### CalculateRefund Request arrives at Administrative API (???)
1. DaysRemining =  number of Days remaining in the current Billing Cycle.
2. ConsumptionFactor = percentage of their Token Allotment that has been consumed.
3. RefundableAmount = Calculate the Amount they paid, minus the Transaction Fees.
4. ProRated = RefundableAmount - (DaysRemaining * PricePerDay)  (Floor is Zero)
5. Refund = (ProRated * ConsumptionFactor)-Fees  (Minimum value of refund is $0.00; no negative refund amounts).

