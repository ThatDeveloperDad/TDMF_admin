# CustomerAccess

## Purpose
Provides methods that Read and Write Customer objects to and from their Storage Resource(s)


## Nerd Stuff
This Component, like other ResourceAccess components, will be assembled from 2 or more separate Class Library assemblies.  

The main assembly that will be referenced by the Manager will be:
[ProjectRootNameSpace].CustomerAccess.Abstractions

ICustomerAccess
Customer
  .Profile
  .Subscription
  .SubscriptionHistory

I'll need to include some way to access Customer information from the following stores:  
 * Microsoft Entra ID  
 * Azure Table Storage (replacement for SQL in this application.)  

The Schema of the Customer Objects to be stored is TBD, and will initially be loosely based on the Customer object defined by the LemonSqueezy API.  

All implementations of ICustomerAccess will have a dependency on the MS-Entra wrapper component, and implement the Storage Tasks against a specific Storage Tech.  (i.e.:  Sql Server, Files, Table Storage, etc....)  
This application will only implement against Azure Table Storage.
