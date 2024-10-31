# Validation Engine

## Normal Person Stuff
### Purpose:
The Validation Engine is a component used by the Account manager to assess whether a Custoemr's Subscription is in a state that allows for the Modification Request that the Manager is handling.  

There are a couple contexts in which Validation can be done.
1. Ensure Completeness and Compliance with the Data Requirements for the Requested Process.
   * This simply means that all of the information that we need to complete the Request is present, and in a correct format so that the backend system can do its work without running into any "Uh-oh, I need this piece of data, and it wasn't given to me.  I need to panic now."
   * Within this context, we are ensuring that ALL the fields that are necessary for the Process to execute have a Value.
   * AND that each Value is in the expected format.
     * i.e.:  Dates are presented as Dates, Numbers are presented as Numbers, etc...  (Believe it not, these things go sideways all the time.)
2. Once we've made sure that the incoming data "Looks Right", we need to make sure that the existing State of whatever we're changing can accept the incoming change.
   * For Example, we have a "Business Rule" that states that each Customer Account may have zero or exactly one subscriptions attached to it.
   * The incoming "NewSusbcription" request identifies a Customer who's "Already Got One!  Is Very Nice!"
   * Event though the Information in the Request is Complete, and in the Correct Formats, that request cannot be processed against the identified Customer's Account.
  
These types of checks are called Validations, which are based on and formed from Rules.  (These rules can be defined by Business Policy, or event technical requirements of the System itself.)

## Nerd Stuff

***Note***  This component has been documented and implemented in a different repository.  
[My Rules Utility and Validation Engine modules](https://github.com/ThatDeveloperDad/components-rules)

