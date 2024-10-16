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
6. The Admin Service will zero out any remaining quotas, immedaitely remove that user from the Paid Access groups.

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

### CalculateRefund Request arrives at Administrative API (???)

### CancelSubscription event arrives at Administrative API

