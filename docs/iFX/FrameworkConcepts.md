# Framework Concepts

## SystemObject
Is a Base class that will be inherited by any other class that represents some kind of meaningful Entity within the context of one or more Processes defined by the System.  
For Example:  CustomerProfile, CustomerSubscription, UserQuota, SubscriptionTemplate, etc...

## IStrategy<T> where T:SystemObject  
Describes a behavior (or set of closely related behaviors) that are performed on the Specified Type (T).  
If a particular implementation of an Action is materially different from another implementation of the same Action on the Same SystemObject, it MUST BE defined as a discrete implementation of that Action.
