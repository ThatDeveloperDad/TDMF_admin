# New Customer Signup

**When a person Signs Up for the first time...**

1. Person navigates to site.
2. Person clicks on Sign Up.
3. Create Login & User Profile with Identity Provider.  (MS Entra)
   * Need to set up other Identity providers like FB/Meta, Google, Apple, etc...
   * Entra will behave as the LOCAL STORE for users/profiles.  (It takes care of getting the necessary info from those other providers via OAuth.)
4. Create a new UserProfile record in The Product's database.
5. Load Product Details for the Customer's selection from the database.
6. App creates a CheckOut session w/ Ecom service via API POST w/ Customer & Product details.
   * Ecom service replies to API Call w/ a Cart URL for THIS purchase
   * App loads the URL in a new window (or overlay, if that's easy.)
  
**IF ECOMMERCE TRANSACTION IS SUCCESSFUL:**
1. Ecommerce Service will send a Purchase event (need more details from ecommerce API Docs) via Webhook(???)
2. Webhook verifies the sender of the message, validates the data format.  If checks pass, event data is forwarded to the Administrative API.
3. Admin API receives the Event Data and:
  * Validates the inbound data for Correctness.  (We need this phase in place, even though we havent imagined the scenarios where this data would be invalid yet.)

**IF SUBSCRIPTION IS VALID**
1. Create User Subscription Record in database.
2. Create UserQuota records in database.
3. Send a Thank You & Getting Started email via the ecommerce service.
