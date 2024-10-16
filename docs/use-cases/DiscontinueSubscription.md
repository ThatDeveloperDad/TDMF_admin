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


## Activities
