# Provide a way for a Product Operator to maintain or create new LLM Function objects and store those in the Product Database.

**Currently, if I want to alter the Describe NPC  Prompt, i have to modify, re-test, and redeploy the whole thing.**  
**The prompt info belongs in the database.**

## Process
1. Present a User Interface that:
  * Allows the user to enter the Prompt Template.
  * Allows the user to specify the replacement tokens for variables within the prompt.
  * Allows the user to give "test" values to those varibales and run the prompt against the selected language model.
  * Stores the Template, Variable List, Test Variable Values, Prompt Properties (Temp, Top-K, Top-P, MaxResultTokens, etc...) in a database table for reference & dev history.
  * Allows "Deployment" of an LLM Function to the Application's Prompt Storage.
2. Collect the Data required to form an "AI Function"
3. Pass that data to a Language Model.
4. Display the Output from the LLM.
5. Save that execution of the prompt under construction.  Include the Token Consumption counts with each execution of the Prompt under Development.  (We'll want that metric to keep ourselves under control.  It's easy to spend a LOT of money on AI Tokens if it's not in front of our faces.  Also R&D Cost Reporting if that's something we care about.)
6. Migrate the Function's data to the "Actual" data store used by the AI Workload Manager & Client Workload Provider.

**This is a significant departure from how The DM's Familiar manages the AI Prompt in place currently.**
When an AI Generation is requested, the current system:
1. Instantiates a New ClientWorkloadProvider.
2. The Workload Provider is given a Proxy to the AiWorkloadManager, and registers the LLM Functions with the AI Manager.
3. These Functions are hardcoded in the Client's WorkloadProvider.

### Code Changes for Existing Product
I'll need to:  
* Define a Storage Table for the Product's AI Functions.
* Create a Resource Access component that the application's Workload Provider can use to load the relevant Function Definitions.
* CACHE THE FUNCTION DEFINITIONS!!!
* For each Function that the code uses, we'll need to also specify which Version of the Function the Code knows how to handle.
* We WILL need to have some Static Identifier that the Functions used by The Product can use to load the appropriate Function Definition.  (I'm thinking FunctionName is what we want to use.  That's a Feature-Design-Time decision.  Named Functions will need to be versioned in the DB, and that version will need to be honored by the Code.)
* Adjust the Ai Manager to validate Execution Requests.  i.e.:  There will be some RAG values that MUST be populated, and some that can be optional.  We'll need to make sure the Code can tell the difference between those, and report back or compensiate as the incoming values change.

### Prompt Engineering Workspace Notes
**Prompt Template Format**
This will be a plain text value, with "ReplacementTokens" injected for the incoming Variable Values.  These Replacement Tokens will be added using a common format, that we can use a RegEx to identify during Prompt Development.
* The standard format for a required Replacement Value will be {{$VariableName}}.  The VariableName must be unique within the Prompt it's used for.  (Injecting that variable at multiple places in the Prompt Template is perfectly valid.)
* We will use a trailing ? on Variable Names to identify those values as optional.  {{$VariableName?}}
* When we use optional variables in the template, we should include language in the Prompt Instructions that the AI should either ignore or generate a value to act as those placeholders.

Once the Prompt Template has been entered, allow the user to "Scan For Variables."
This will produce a list of those variables with formatting to easily determine which are required, and which are not.
Add UI elements to allow the user to enter values for those Replacement Vars, and finally execute the prompt using those values.

FOR EXAMPLE:
```
You are a helpful assistant to a busy Game Master.
For this request, you will use the Seed Attributes that are provided as
a JSON string to create a new Character for a Fantasy setting.

Please create a suitable name for this character, and BRIEF descriptions
of thier appearance, personality, and current circumstances.  Keep your 
descriptions short and to the point, maybe one or two sentences for each
category.  Finally, please add a 3 or 4 sentence paragraph describing how
their background contributed to their current personality and circumstance.

Seed Attributes
============
{{$npcJson}}
============
```
In this case, the npcJson value is required.

Another way we could express this is:
```
You are a helpful assistant to a busy Game Master.
For this request, you will use the Seed Attributes that are provided as
a JSON string to create a new Character for a Fantasy setting. Use the
supplied Seed Attributes to create this character.  For any attribute that
does not have a seed value, refer to the provided option lists for suggested values
and choose one that makes sense for the character.  If an option list does not exist,
please make something up that would make sense for a fantasy world.

Please create a suitable name for this character, and BRIEF descriptions
of thier appearance, personality, and current circumstances.  Keep your 
descriptions short and to the point, maybe one or two sentences for each
category.

INSTRUCTIONS:  
Output Format must be VALID JSON as specified in the OUTPUT SCHEMA.  Do not add additional text, commentary, or other decoration outside of the JSON object you create.
Name: Please create that fits the provided Species.  Keep it to Given and Surname.  Do not include titles or honorifics.
Appearance:  Write one sentence describing their appearance and mode of dress.  Base this description on the provided data and their current circumstance.
Personality:  Create a short description of their personality, synthesized from their species, background, and profession.  Be brief, one or two short sentences is sufficient.
Background:  Describe their early life using the background data provided.  Be brief, two to three short sentences is enough.  Mention how their background led to their current circumstances.
CurrentCircumstance:  Base this on their profession.  Be sure to link their background to this circumstance.  If they are retired, mention that.  Be brief; two or three SHORT sentences are enough.

Seed Attributes
============
Profession:  {{$profession?}}
Background:  {{$background?}}
Species:  {{$species?}}
============

OptionLists
============
Profession:  [{{$professionList?}}]
Background:  [{{$backgroundList?}}]
Species:  [{{$speciesList}}]
============
```
In this case, only the speciesList is a required input value.  The profession and background Lists are optional, as are all of the seed Attributes, giving the LLM much more freedom in what it does.

## Activities
### Prompt Development and Iteration
1. Scan Prompt for Replacement Tokens
2. Identify which Tokens are Optional & Required.
   * (?<=\{\{\$)[^\{\}]+(?<!\?)(?=\}\}) can be used to identify REQUIRED replacementTokens.
   * (?<=\{\{\$)[^\{\}]+\?(?=\}\}) is the regex we'll use to identify OPTIONAL replacementTokens.
4. Render UI to collect values for those tokens.
5. Pass Prompt & FunctionArguments to LLM Provider.
6. Display Results when they arrive.
7. Save the results to the PromptDevelopment area of our storage service.

### Deploy Prompt to "product" prompt storage.
**This will be slightly different process for New AiFunctions that will be created for Existing AiFunctions.**  
**We're going to need to use Semantic Versioning here, so that the Code knows to use the correct version of the FunctionDefinition, as those Definitions eveolve.**   

Plain language:  When the required input or expected Output from a prompt changes, we'll need to update the code to account for those material changes, or things will crash.  

**New Functions**
Create the FunctionDefinition object, using V1 as the version string.
Save it through the Product's AiFunction storage provider.

**Existing Functions**
Compare the Variable lists between the "New" version, and the "Old" version.
If the REQUIRED Variables or Expected Output Format have changed, Treat as a major Version increment.
If the OPTIONAL variables have changed and Output Format has not, Treat as a Minor Version increment.
If only the instructions within the prompt have changed, Increment as a Patch.
Save the updated FunctionDefinition through the Product's AiFunction Storage Provider.


