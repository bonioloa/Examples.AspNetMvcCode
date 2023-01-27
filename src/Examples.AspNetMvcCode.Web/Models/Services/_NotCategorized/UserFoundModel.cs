namespace Examples.AspNetMvcCode.Web.Models;

public record UserFoundModel(
    long UserId
    , string Login
    , string Surname
    , string Name
    , string Email
    , UserProfile UserProfileType
    , bool IsTenantManaged
    );