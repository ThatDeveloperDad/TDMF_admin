# CustomerAccess

## Purpose
Provides methods that Read and Write Customer objects to and from their Storage Resource(s)


## Nerd Stuff
This Component, like other ResourceAccess components, will be assembled from 2 or more separate Class Library assemblies.  

The main assembly that will be referenced by the Manager will be:
[ProjectRootNameSpace].CustomerAccess.Abstractions

### Interfaces
#### CustomerAccess.Abstractions.ICustomerAccess

**Methods**  
CustomerAccess.Abstractions.Customer? LoadCustomer(string id)
 * uses MS Graph provider to Query Entra for the information that is available there.
 * uses the same ID to query the AppStorage Customer collection.

CustomerAccess.Abstractions.Customer StoreCustomer(Customer customer)
 * Any properties in the Profile that have been changed and are stored in Entra will be updated.
 * The incoming customer will have its changes applied to the Customer instance that is in storage.
 * The customer will be re-loaded after those operations complete, and be returned to the caller.

### Classes

#### Customer
**Properties:**  
 * .Id  (We're going to use the EntraId as the "primary" identifier for each Customer within the system.)
 * .Ids
 * .Profile
 * .Subscription
   * .SubscriptionHistory  

#### CustomerIds
This will contain a collection of the Ids that are associated with this Customer object in the various 3rd party Systems we'll integrate with.  (Currently only Entra and LemonSqueezy)  Each ID will have a VendorName and the Id Value.  

#### CustomerProfile  
Holds the information that we need to either share with 3rd party service providers or gets displayed in the Application.
Keep PII to the absolute minimum here.
 * .DisplayPame
 * *other properties that make sense to store here for efficiency's sake.*
(We might not want to query Entra every time we make a call to the E-Com api.  Querying Entra WILL be my first strategy though, because if it works and isn't too slow, Imma keep it.)

#### CustomerSubscription  
Contains the Subscription information for a given Customer.

#### SubscriptionHistory
Contains a list of SubscriptionActions that have been taken over the course of the current Subscription's lifetime.  
i.e:  Renewal, Pause/Resume, Cancellation, etc...

I'll need to include some way to access Customer information from the following stores:  
 * Microsoft Entra ID  
 * Azure Table Storage (replacement for SQL in this application.)  

The Schema of the Customer Objects to be stored is TBD, and will initially be loosely based on the Customer object defined by the LemonSqueezy API.  

All implementations of ICustomerAccess will have a dependency on the MS-Entra wrapper component, and implement the Storage Tasks against a specific Storage Tech.  (i.e.:  Sql Server, Files, Table Storage, etc....)  
This application will only implement against Azure Table Storage.
