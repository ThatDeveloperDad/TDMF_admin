# DomainConstants namespace

**If you're going to use the DomainUtilities namespace from ThatDeveloperDad.iFX**

I HIGHLY recommend defining the various formal names for the Entities and Entity Attributes as constants, in some Project that is global to your solution.  

For example:  

**BEFORE:**  
```csharp
[DomainEntity(
    entityName: "ApplicationUser",   // <=== Magic String, eeeew!
    declaringArchetype: ComponentArchetype.ResourceAccess)]
public class UserIdentityResource : IdiomaticType
{
    public UserIdentityResource()
    {
        UserId = string.Empty;
        DisplayName = string.Empty;
    }

    public UserIdentityResource(string userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
    }

    [EntityAttribute("UserId")]
    public string UserId { get; init; }
    
    [EntityAttribute("DisplayName")]
    public string DisplayName { get; init; }
}
```
Since those Magic Strings are going to be used in potentially MANY places in our Detailed Design, and changing every instance of a Magic String is a PAIN IN THE NECK, creating some centralized place where we can get to those values is going to be a BIG HELP!  


**AFTER:**
```csharp
[DomainEntity(
    entityName: Entities.ApplicationUser.EntityName,
    declaringArchetype: ComponentArchetype.ResourceAccess)]
public class UserIdentityResource : IdiomaticType
{
    public UserIdentityResource()
    {
        UserId = string.Empty;
        DisplayName = string.Empty;
    }

    public UserIdentityResource(string userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
    }

    [EntityAttribute(Entities.ApplicationUser.Attributes.UserId)]
    public string UserId { get; init; }
    
    [EntityAttribute(Entities.ApplicationUser.Attributes.DisplayName)]
    public string DisplayName { get; init; }
}

```


When adding constants to describe Domain Models in this namespace, we'll use a Partial Class
called, "SaaSDomain".

It will in turn contain nested public static classes named for the DomainEntities that get modeled throughout the solution.

Each DomainEntity class must include a "DomainEntity" string constant that gives the name for that entity, as well as an Attributes inner class that identifies the Attribute Names of the important properties of the Entity.

These are consumed by:
Component Code, in the DataContracts that represent a "Local Idiomatic Form" of the DomainEntity.

and the 

system-framework's DomainUtilities code.

Each Entity described in this manner should be in its own file.  
As a favor to future you, and your collaborators, let's use the naming scheme of:  

`SaaSDomain_[DomainEntityName]`  

and a rudimentary version of this concept would look like this:  

```csharp
public partial class SaaSDomain
{
    public static class ApplicationUser
    {
        public const string EntityName = "ApplicationUser";

        public static class Attributes
        {
            public const string UserId = "UserId";
            public const string DisplayName = "DisplayName";
        }
    }
}
```