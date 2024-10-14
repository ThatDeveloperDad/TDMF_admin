# Volatilities & External Systems

| Term | Explantaion |
|-----|-----|
| The Product | In the context of this repository & project, "The Product" refers to the Software product branded as "The DM's Familiar". |
| The Business | In the context of this documentation, "The Business" refers to "That Developer Dad" as a commercial entity. |


## Volatilities

### Identity Management
- Who are my Users?
- Keep their personal information safe, but accessible to those systems that are necessary for the operation of The Business.
- Allow as much self-service activity as possible.  (Reset Password, manage Profile, etc...)

### Payment Management
- How do My Users pay me for the services they receive?
- How do I protect their payment information while still allowing The Business to do things like automatic Monthly Billing?

### Product Capabilities
- As new capabilities are added, many will be included in the base Subscription Product.
- Some very advanced (not yet imagined) features may require a higher priced tier, as those features may have a higher delivery cost.

### Supporting Data for the Product Itself
- AI Prompts, Legal Value Lists, (DB stored RuleSets???)
- Associations between:  Product/Subscription -> Quota-bound Features, etc...
- Usage metrics & Operational data
  - Token Usage by Prompt, User, Time Slices, etc...
  - DB Usage by Entity / User, etc...
  - Error Logs????

 ### These things can be further generalized into:
 - Customer Management
   - Identity
   - Profile
   - Account Status
   - Payments
   - Email Messaging 
 - Product Maintenance
   - Product & Subscription data
   - Data that Drives the application.
 - Integrations
   - First Party Services  (The application itself)
   - Third Party Services  (LemonSqueezy, MS Entra, etc...)

## External Systems

### Identity Management  
The DM's Familiar uses MS Entra (formerly called Azure AD B2C) as its Authentication and Authorization Store.
User objects are created witin the Directory and placed into Entra Groups which are used to control who can access which features in The Product.

### The Store / Payment Processor / Email Distribution, etc...
Of the services available for these types of use cases, I'm leaning toward LemonSqueezy.  
The service provides API endpoints for the User / Subscription Management / Payment processes that I need.  
LemonSqueezy does not offer a .Net SDK for those Endpoints.  Consider making the API wrapper for THIS system available as an independent codebase.
They also handle Sales Tax calculation, collection, and remittance so I don't have to.
They offer PCI compliance out of the box, and none of my customer's sensitive payment Info would be stored in databases that I own.

### The Product itself (The DM's Familiar)
Need to be able to update Subscription & Quota data for each user.
Want to be able to manage the information that's displayed on the Products pages within the app's website.
Want to be able to manage the AI Prompts used by the product from this subsystem as well.
