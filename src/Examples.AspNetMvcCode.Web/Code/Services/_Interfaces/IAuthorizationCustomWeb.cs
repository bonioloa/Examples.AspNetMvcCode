namespace Examples.AspNetMvcCode.Web.Code;

public interface IAuthorizationCustomWeb
{
    Task<bool> EnableRegistrationForUsersAsync();
    Task<bool> TenantHasAnonymousConfigAsync();
    Task<bool> TenantHasSsoAsync();
    Task<bool> TenantHasRegisteredConfigAsync();
    Task<bool> TenantHasSsoOptionalAsync();
    Task<bool> UserAccessedWithLoginAndPasswordAsync();
    Task<bool> UserIsAdminAsync();
    Task<bool> UserIsNotLoggedOrIsNotSupervisorAsync();
    Task<bool> UserIsSupervisorAsync();
    Task<bool> UserIsSupervisorWithRegisteredConfigAsync();
    Task<bool> UserLoggedInThroughSsoAsync();
    RouteValueDictionary GetLandingPageByRole(long? itemId, string languageIso);
    Task<bool> UserIsAdminOnlyAsync();
    Task<bool> UserIsAdminTenantAsync();
    Task<bool> UserIsAdminApplicationAsync();
    RouteViewModel GetLandingPageRouteByRole(long? itemIdFromLoginCode, string languageIso);
}