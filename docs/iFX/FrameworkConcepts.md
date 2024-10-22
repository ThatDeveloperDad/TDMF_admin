# Framework Concepts

## SystemObject
 * Is a Base class that will be inherited by any other class that represents some kind of meaningful Entity within the context of one or more Processes defined by the System.  
 * For Example:  CustomerProfile, CustomerSubscription, UserQuota, SubscriptionTemplate, etc...

## IStrategy<T> where T:SystemObject  
 * Describes a behavior (or set of closely related behaviors) that are performed on the Specified Type (T).  
 * If a particular implementation of an Action is materially different from another implementation of the same Action on the Same SystemObject, it MUST BE defined as a discrete implementation of that Action.
 * Has an ActionName that identifies the "thing" the strategy performs.
 * IStrategy<T> Implementations will be defined as Internal classes within the Component Project that uses them.

## Manager Components will HAVE an instance/implementation of:
 * IStrategyHandler<T> for each SystemObject the Manager deals with.
 * IProxyFactory will be a dictionary of <string, Type>
   * WHERE:
     * string is the nameof(dependencyInterface)
     * Type is the concrete Type to be used when calling the "remote" component.
 * "Remote" components that are used by IStrategy implementations will receive the Proxies to their required remote services when those Strategies are initialized.
