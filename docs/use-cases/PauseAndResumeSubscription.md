# Pause and Resume Subscription
[Back](/docs/UseCases.md "Back to Use Case List")

---

## Process:
1.  Pause / Resume subscription process is always initiated by a Person. (Either Customer or Administrator)
  *  The Person navigates to their Subscription Management UI (Either within Ecom Service or Admin System)
  *  Enters the date to pause and (optionally) the date to resume.
  *  Completes the interaction by submitting the form.
2. The data is sent to the Ecom Service's backend system where it is processed.
3. Ecom System sends an Event Message to the Admin System via Webhook invocation.
4. AdminSystem updates that Person's local account details.
5. During the "Paused" time, if the Person logs in, they will not be able to use any of the "Paid" features of the application.  
