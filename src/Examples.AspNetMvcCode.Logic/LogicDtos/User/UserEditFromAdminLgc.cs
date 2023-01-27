namespace Examples.AspNetMvcCode.Logic;

public record UserEditFromAdminLgc(
    long UserId
    , string Name
    , string Surname
    , string Email
    , string ExclusiveRole
    , IEnumerable<long> SupervisorRoles
    );