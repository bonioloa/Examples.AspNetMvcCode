namespace Examples.AspNetMvcCode.Logic;

public record class UserSupervisorDataLgc(
     long UserId
    , bool UserMustBeIncludedInManagedBeforeEdits
    , bool IsActive
    , string Login
    , string Name
    , string Surname
    , string Email
    , RolesSelectionLgc RolesSelection
    );