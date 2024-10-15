# Subscription Renewal
[Back](/docs/UseCases.md "Back to Use Case List")  

**When the Ecommerce Service renews a customer's monthly subscription:**  

1. E-Commerce Service auto-renews a Customer's Subscription
2. E-Com Services sends a notification to This System about the transaction attempt via WebHook call.
3. WebHook code verifies the source of the message, and validates that the message data is well-formed.

**IF THE WEBHOOM INVOCATION IS NOT VERIFIED AS FROM AN EXPECTED SENDER (Unhappy Path)**
Webhook shall return the appropriate HTTP Code.  Most likely a 400 (Bad Request), a 403 (Forbidden)  
The API Documentation for the Ecommerce Service may specify which error codes are expected from these WebHook Calls.
Unverified Sender event should be logged w/ any information we can get about the HTTP Request.

**IF THE DATA IN THE WEBHOOK INVOCATION IS NOT WELL FORMED(Unhappy Path)**
WebHook shall respond to the incoming request with the appropriate HTTP Error Code, as above.  
Add details once this research has been done.
That we received Malformed Event Data should be logged w/ any info we can get from the Request, as well as which elements of the Event Data were invalid.  (Redact actual values in logs)


**IF THE WEBHOOK INVOCATION IS BOTH VERIFIED AND VALID(Happy path)**
Forward the Webhook data to the appropriate endpoint on THIS system's Admin API.
Send whichever HTTP Success Status Code in the response back to the Ecom Service.  (See their docs for what's expected here)
Log that the Event was Received and Forwarded for processing.

**When the Renewal Event Data arrives at the Administrative API Endpoint:**
1. Validate that the data in the Event is correct for a Renewal Operation. (see below for invalid scenarios)
2. Retrieve the current Subscription & Quotas for the Customer identified in the Event Data.
3. Retrieve any Product / Quota data for the SKU specified in the Event Data.
4. Update any Quotas that "refresh" each month.
5. Save the Customer's subscription back to permanent storage.
6. Send a "Thank you for your continued business" email to the customer when processing is complete.  (I know... it's not business savvy, but it IS considerate of my customers.  BE THE CHANGE!!!)  Brainstorm ways to add value with these messages on a monthly basis.  i.e.:  Tip of the Month, News about new features under development, free "samples" that I've created, etc...
7. Log the execution of the event process (successful or otherwise,) with execution times of activities & steps, etc...

**Invalid Renewal Scenarios**
- The Subscription being renewed was marked as "Discontinued" by the customer
  - Discover WHY the Ecom Service renewed it anyway & prevent this from happening to ANYONE if possible.
- The Customer doesn't have a local Login or Profile.
  - Should never happen, but it might via some weird combination of circumstances.
- The SKU in the Renewal Event Data doesn't exist in the local systems.
  - That's likely a Data Entry error and should be discovered during testing, BUT, ya know how thoat kinda thing goes.  needs to be accounted for and handled here.

For any of these exception cases, make sure to write Log Entries for analysis purposes.
