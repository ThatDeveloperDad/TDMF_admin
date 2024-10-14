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

## External Systems

### Identity Store  
The DM's Familiar uses MS Entra (formerly called Azure AD B2C) as its Authentication and Authorization Store.
User objects are created witin the Directory and placed into Entra Groups which are used to control who can access which features in The Product.
