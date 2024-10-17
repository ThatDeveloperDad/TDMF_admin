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
5. Save that execution of the prompt under construction.
6. Migrate the Function's data to the "Actual" data store used by the AI Workload Manager & Client Workload Provider.
