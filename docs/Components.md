# System Components for the Admin Subsystem.

## Presentation
*  **Webhooks**  
  (Azure Function App)
  High Priority

* **Admin UI**
  (Blazor?  MAUI?  AspNetCore MVC???)
  (deferred, may never happen)

## BFF (Backend for Front-ends)
* Accounts WorkloadProvider
  (a .NetCore *Minimal* API)

## "Backend" System Components
***Will start off as simple .Net projects/interfaces/classes.  May get discrete hosting eventually, IF NEEDED***
### Managers  
* **AccountManager**
  Implements the Subscription focused Use Cases.  
  Ensures the inbound request data is Correct for the Process to be executed
  Routes that data to the appropriate Engines & Resource components to execute the Process.

### Engines
* **ProrateEngine**
* **ValidationEngine**

### ResourceAccess
* **CustomerAccess**
  Loads a Customer from any of: Entitlement, Identity, or E-Commerce providers
  Can initiate a Save operation against any of: Entitlement, Identity or E-Commerce providers.
* **SubscriptionAccess**
  Loads a Customer Subscription from any of:  Entitlement, or E-Commerce providers
  Can initiate a Save operation against any of:  Entitlement, Identity or E-Commerce providers.
* **StoreGateway**
  Used to create new "process sessions" within the E-Commerce Service.  Responds with a URL to a page within that service where a Customer can complete the operation.
* **RulesAccess**
  Reads * Writes the rules used to govern Validity and Correctness of Inbound ResourceData for a variety of Contexts.

### ResourceProviders
* **MsEntraProvider**
  Used For Authentication & Authorization.
  Primary Source of Truth for AuthN & AuthZ.
  Primary System of Record for user Identity & Group membership.
* **LemonSqueezyProvider**
  Can query Customer & Subscription & Transaction history from E-Commerce Service.
  Primarily acts as the SOURCE of incoming AccountManagement use cases.
  ***Can*** Initiate Cart sessions via HTTP Post, providing a unique URL to the Cart in the response.
* **EntitlementsProvider**
  Is the Local Database that holds the information necessary to operate the Product when being used by a Subscriber.
  Is A system of record, but mostly for reference during day-to-day operations.
  Is NOT the Source of Truth for Identity, or Subscription Status.

## Static Architecture
![A Static Architecture Diagram for a SaaS product Admin System](docs/images/AdminSystem_StaticArch.png "Admin Static Architecture")
