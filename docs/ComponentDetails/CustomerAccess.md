# CustomerAccess

## Purpose
Provides methods that Read and Write Customer objects to and from their Storage Resource(s)


## Nerd Stuff
This Component, like other ResourceAccess components, will be assembled from 2 or more separate Class Library assemblies.  

The main assembly that will be referenced by the Manager will be:
[ProjectRootNameSpace].CustomerAccess.Abstractions

### Interfaces
####ICustomerAccess

### Classes

####Customer
**Properties:**  
  .Id  (We're going to use the EntraId as the "primary" identifier for each Customer within the system.)  
  .Ids  
  .Profile  
  .Subscription  
    .SubscriptionHistory  

#### CustomerIds
This will contain a collection of the Ids that are associated with this Customer object in the various 3rd party Systems we'll integrate with.  (Currently only Entra and LemonSqueezy)  Each ID will have a VendorName and the Id Value.
#### CustomerProfile  

#### Subscription  

#### SubscriptionHistory
  

I'll need to include some way to access Customer information from the following stores:  
 * Microsoft Entra ID  
 * Azure Table Storage (replacement for SQL in this application.)  

The Schema of the Customer Objects to be stored is TBD, and will initially be loosely based on the Customer object defined by the LemonSqueezy API.  

All implementations of ICustomerAccess will have a dependency on the MS-Entra wrapper component, and implement the Storage Tasks against a specific Storage Tech.  (i.e.:  Sql Server, Files, Table Storage, etc....)  
This application will only implement against Azure Table Storage.
