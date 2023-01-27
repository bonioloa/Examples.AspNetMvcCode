namespace Examples.AspNetMvcCode.Web.Models;

public record UserEditSupervisorViewModel(
    long UserId
    , bool UserMustBeIncludedInManagedBeforeEdits
    , bool IsActive
    , string Login
    , string Name
    , string Surname
    , string Email
    , RolesSelectionViewModel RolesSelection
    );