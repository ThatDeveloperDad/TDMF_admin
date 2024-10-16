# Discontinue Subscription
[Back](/docs/UseCases.md "Back to Use Case List")

---
## Important Points
**Discontinue refers specifically to the Customer Request that we no longer bill them on an ongoing basis.**
* They will retain access to the Paid Features of the product through the end of their current Billing Period.
* When that period lapses, they will retain read-only access to those items they have created within the Product.

## Process
1. Discontinue Subscription will always be intiated by a Person.  (Either the Subscribing Customer, or an Administrator)
2. The Process will be initiated either through the Account Portal provided by the E-Commerce System, or the Administrative System we're building here (request will route to ECom SystemAPI and come back to our system for local processing.)
3. Once the request has been submitted to the ECom Services and Processed there, that service will communicate the event to the Local Admin System via Webhook.
4. The Local Admin System will make the necessary changes to the Customer's Subscription, Quotas, and Account Permissions as needed.
5. Send an email to the Customer confirming that their Subscription has been Discontinued.

## Activities

### Process SubscriptionDiscontinued (sic) WebHook request
1. Verify that the sender of the Webhook Request is valid & Authorized.
  1. If NOT verified, return the HTTP Status Code as specified in E-Commerce Vendor's integration docs.
  2. If Verified, continue.
2. Validate that the event data is well formed and complete for the requested operation.
  1. If the Data IS NOT valid, return the HTTP Status Code as specified in E-Commerce Vendor's integration docs.
  2. If the Data IS VALID, forward the message to the Administreative API for local system processing.

### Handle SubscriptionDiscontinued request at Administrative API
1. Validate that the Discontinue event data is Correct for the Customer and Subscription Product identified in the request.
2. If NOT Valid, we're going to need to perform some kind of Compensating activity to get our systems back in sync with the E-Commerce Service's systems.
3. If Valid:
  1. Update the Customer's Subscription object to stop billing and updating the Quota on the day after their last Paid day.

### Reconcile Expiring Subscriptions with the Identity Service.
1. On a Daily basis, we need to retrieve a list of Users who no longer belong in the PaidUsers groups in the Identity Service.
2. Remove any of the users that have been identified buthave not yet been removed.
