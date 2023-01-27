namespace Comunica.ProcessManager.Web.Code;

public interface IAuthorizationCustomWeb
{
    Task<bool> EnableRegistrationForUsersAsync();
    Task<bool> TenantHasAnonymousConfigAsync();
    Task<bool> TenantHasSsoAsync();
    Task<bool> TenantHasRegisteredConfigAsync();
    Task<bool> TenantHasSsoOnlyAsync();
    Task<bool> TenantHasSsoOptionalAsync();
    Task<bool> UserAccessedWithLoginAndPasswordAsync();
    bool UserIsAdmin();
    Task<bool> UserIsAdminAsync();
    Task<bool> UserIsAdminOnlyAsync();
    Task<bool> UserIsNotLoggedOrIsNotSupervisorAsync();
    bool UserIsSupervisor();
    Task<bool> UserIsSupervisorAsync();
    Task<bool> UserIsSupervisorWithRegisteredConfigAsync();
    Task<bool> UserLoggedInThroughSsoAsync();
    RouteValueDictionary GetLandingPageByRole(long? idItemFromLoginCode, string languageIso);
}
