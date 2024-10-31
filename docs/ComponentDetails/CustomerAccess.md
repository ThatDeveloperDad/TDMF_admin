# CustomerAccess

## Purpose
Provides methods that Read and Write Customer objects to and from their Storage Resource(s)


## Nerd Stuff
This Component, like other ResourceAccess components, will be assembled from 2 or more separate Class Library assemblies.  

The main assembly that will be referenced by the Manager will be:
[ProjectRootNameSpace].CustomerAccess.Abstractions

I'll need to include some wat to access Customer information from the following stores:
Microsoft Entra ID
Azure Table Storage (replacement for SQL in this application.)
