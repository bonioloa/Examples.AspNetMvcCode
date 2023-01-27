namespace Examples.AspNetMvcCode.Logic;

public record UserFoundLgc(
    long UserId
    , string Login
    , string Surname
    , string Name
    , string Email
    , UserProfile UserProfileType
    , bool IsTenantManaged
    );