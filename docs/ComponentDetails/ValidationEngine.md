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
A Validation Engine is a code artifact THAT HAS:  
 * an organized collection of Rules that can be applied to \<some entity\> (aka SystemObject) in whichever combination is required by \<some process \> (aka: SystemAction aka: Process / Activity / Step)  
 * This collection contains an indexer, which identifies the combination of SystemObject + SystemAction. For each indexed combination, there isa set of Rules that must be satisfied for that SystemAction to proceed.

Each Rule HAS: 
 * (Property) The Type of SystemObject to which the Rule applies.
 * (Property) A string value that identifies the Property that the Rule applies to.
 * (Property) BooleanExpression that must evaluate to True.
 * (Property) Optionally, a Rule may also be configured with a Consequence Action that performs some manipulation against the SystemObject Instance when the Expression returns False.
 * (Method) A method that accepts an instance of the Type specified in the Type property and returns a Boolean.  IF the optional Consequence action has been configured, the Instance will be returned in an out parameter from this method.)

An Expression HAS: 
 * a Type specifier that defined the Type used when comparing the inbound Value To Test against the ExpectedValue
 * an Operator
 * an ExpectedValueOperand.
 * a method that accepts a value of the appropriate Type, compares it to the ExpectedValueOperand using the specified Operator, and returns a boolean value representing the outcome of the Expression Evaluation.
