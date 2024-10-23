# Validation Engine

## Normal Person Stuff
### Purpose:
A Validation Engine has responsibility for ensuring that a Data Object is not only well-formed in terms of its Schema Specification, but also in a suitable State for the operation to be performed.  

*That statement is dangerously close to some wonky nerd stuff.  I'll try and do better.*  

In the Real World, when we want to do some activity, we need to make sure that the materials used in that activity are not only in good shape, but also are in a correct condition for the activity we're about to do.  

Image we want to build some bookshelves.  We need:
 * Some Wood
 * Some way of attaching the wood together.  (Nails, Screws, Glue, Complex Joinery techniques, etc...  We'll roll with glue & screws.)
 * A Design.  (How many pieces of wood, what dimensions for the boards, etc...)
 * Tools.  (Saws, Scredrivers, Sanders, Measuring tools, etc...)

We collect our materials, and check them over to make sure we have everything we need.  (Material list is complete)
Then, we make sure that they are the correct "versions" of the tools for what we're about to do.  (Make sure things are "ready" for makng the shelves.)

If our wood is just a bunch of branches and logs, we can't proceed.  We need that wood to be in standard board shapes.  
If our screws have a different head style from our screwdrivers, or are the wrong size for connecting the boards, we have a problem.  
If the only saw we have is a chainsaw, we're goinna have a bad time.  

You get the idea.  Ensuring that the materials are all present and in the right places is only the first step.  (Us nerds call that "Well-Formed" when talking about the digital representations of Real World Stuff.)  By ensuring that our Stuff is at least well formed, we can either:  Proceed to the next set of checks (or our work,) OR we cna let you know that we don't have everything we need to Build Those Shelves. 

THEN, once we've made sure that everything on our list is present, we can do some further checking on the condition of the Things in the List, to make sure we're not going to waste time with materials that aren't in the condition that is correct for what you've asked us to do.  (We don't want to build with boards that are warped or twisted, or full of cracks.  That's not going to hold many books, and will eventually fall on the cat.  We don't want that.)

**SO...** What to do..?
In software, Validation is nothing more than asking a sequence of questions with true or false answers.  

The questions are always relevent to the Thing we're about to do, and the Stuff that we'll use to Do The Thing.  

Sometimes, these questions are the same, (or at least similar enough,) that we can ask those questions for a variety of combinations including Stuff, and Things.  It just takes a little bit of extra thought to chase those out.  

If we identify these questions, and then store them somewhere separate from the ThingStuffer in the code, that means we can change the sets of questions more easily, and perhaps we need to create fewer enshantments over all, if those questions are made flexible.  That's where the nerd stuff comes in.  (Software Applications really are enchantments.  We're giving inanimate objects special abilities using wierd words and symbols.  Yes, us nerds are wannabe wizards.)

## Nerd Stuff

Validation involves executing a series of boolean expressions against some Object.
We can abstract many of these expressions into simple functions that accept the Object.Property.Value and the TestValue.
Further, we can abstract the code that accesses that Object.Property.Value so that we don't need to hard-code that property access logic in our applications.  This makes that expressions into configuration, rather than code.

A RULE is a single comparison expression between two values.  (Some frameworks or taxonomies use the word Predicate for this concept.)  

An expression takes the form of:  

\[ActualValue\]  \[Operator\]  \[ExpectedValue\]  

WHERE:  
ActualValue is the Object.Property.Value that we're testing.
Operator is the kind of comparison we're using for the inspection.  
ExpectedValue is the Fixed Value that our comparison needs to see to be true.  

Operators can include:  Equals, GreaterThat, LessThan, Contains (for some scenarios) as well as the inverse of each of those.

In .Net languages, which are considered to be Strongly Typed, we must also ensure that the Operands (Actual and Expected values,) are of comparable types.  (We can't compare a string to an int, who knows what would happen there.)

SO... an expression has:  
 * One Operator
 * One ExpectedValue*
 * An OperandType
 * The TestOperand is passed into the expression at runtime as a parameter.

*Some rules can be built using a collection of values as the ExpectedValue.  Those Rules typically use the Set-Comparison Operators)  This expected Value should almost never be hard coded into the Expression's definition.

A RULE combines an expression with a single Property on a specific SystemObject Type.  A Rule should also (optionally) define the consequence for the expression returning False.  (In the case of a Validation Rule, perhaps a Validation Error would be added to the System Object's State somehow, maybe a collection of Validation Messages?)

Therefore, a Rule has:  
 * An Expression
 * A Target Type (that inherits from SystemObject)
 * The Property name to be used as the TestOperand on the Expression
 * An Expected Value (which is injected into the Expression when it is initialized)
 * (optionally,) some Action to take when the Expression evaluates to false.

Internal to a Rule implementation, when the Rule executes against the Target Instance, the code will get the value for the TargetProperty from that TargetInstance, and run the Rule's Expression using the property value.  If a Consequence has been configured, it too is executed against the TargetInstance.

