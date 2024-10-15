# Use Cases

**These Use Cases may need to be available not only through a UI, but also through an API.**

*Note:* 
| Term | Explantaion |
|-----|-----|
| **The Product** | In the context of this repository & project, "The Product" refers to the Software product branded as "The DM's Familiar". |
| **The Business** | In the context of this documentation, "The Business" refers to "That Developer Dad" as a commercial entity. |
| **The Store** | In this documentation, "The Store" refers to whichever payment processor / ecommerce service has been selected. |

## This subsystem needs to:
- Handle the backend Tasks that occur when a Customer:
  - Signs up for the first time
  - Renews their monthly subscription
  - Pauses their monthly subscription
  - Discontinues their monthly subscription (at end of current biulling period)
  - Cancels their subscription with pro-rated refund (immediately)
    - Hopefully, this will be an exception case.  This gets complicated.

- Allow a system administrator to:
  - Enter details about the Products and Subscriptions that are being offered through the Store. (Most ecommerce services hold this data locally and "let us know" when a sale is made.)
    - This will allow us to automatically provision the resources for custoemrs as they sign up.
    - FOR NOW!!!  We will set up the products and subscriptions in the ecommerce service first, then manually add them to our local administrative system.  If there's a need to move in the other direction later, we can always build that out in the future.
  - Create and manage the AI Prompt information that is used in The Product for existing festures as well as new features, as they are added to the system.
    - Token consumption used while iterating on prompt definitions and templates must be logged in the existing TokenConsumption logs, and marked as distinct from the Product's Functions.
    - In addition to the Token Consumption logged as R&D in the system, the Prompt Engineering feature must also record each iteration of a prompt as it evolves.  These records shall include:  The Prompt Template, any Data that is injected into that template, the rendered prompt itself, and the output from the LLM.
     - This will allow the Prompt Developers to see the effects of iterative changes to the AI Prompts as they're made, and provide a history of those iterations.
   - When an LLM Function under development has been deemed worthy, it should be "deployed" to a different storage table in the database from which the Llm Manager in The Product will read in the available Functions.
     - We'll need to be careful with this, because if we change the required Parameters in the prompt template, without changing the code that calls that AI Function, Very Bad Things will happen at RunTime.
  
    
### These are "How" details.
- Manage the Groups defined in MS Entra that are used to control access to different areas of The DM's Familiar
- Manage Membership in those Groups
- Manage "System Local" data about the Products and Subscriptions that are available for sale in the External Store
- Manage the AI Prompts (and eventually Prompt Workflows) that are used by the different features provided by The DM's Familiar
- Manage user Accounts within The DM's Familiar
  - Allow same account manipulations as provided by the Store
- ~Send Email notifications to the Users, using those Features provided by The Store~
