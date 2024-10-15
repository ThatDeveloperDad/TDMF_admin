# Use Cases

**These Use Cases may need to be available not only through a UI, but also through an API.**

*Note:* 
**The Product** refers to the Software Services delivered under the name *"The DM's Familiar"*
**The Store** refers to an external service provider that will handle all payment details for The Product. 

##This subsystem needs to:
- Handle the backend Tasks that occur when a Customer:
  - Signs up for the first time
  - Renews their monthly subscription
  - Pauses their monthly subscription
  - Discontinues their monthly subscription (at end of current biulling period)
  - Cancels their subscription with pro-rated refund (immediately)
    - Hopefully, this will be an exception case.  This gets complicated.

- Allow an administrator to:
  - Enter details about the Products and Subscriptions that are being offered through the Store.

- Manage the Groups defined in MS Entra that are used to control access to different areas of The DM's Familiar
- Manage Membership in those Groups
- Manage "System Local" data about the Products and Subscriptions that are available for sale in the External Store
- Manage the AI Prompts (and eventually Prompt Workflows) that are used by the different features provided by The DM's Familiar
- Manage user Accounts within The DM's Familiar
  - Allow same account manipulations as provided by the Store
- Send Email notifications to the Users, using those Features provided by The Store
