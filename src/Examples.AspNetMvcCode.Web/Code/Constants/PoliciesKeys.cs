namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// see StartupAuthorizationExtensions
/// </summary>
public static class PoliciesKeys
{
    public const string UserShouldHaveTenantProfile = "UserShouldHaveTenantProfile";

    public const string TenantHasAnonymousConfig = "TenantHasAnonymousConfig";
    public const string TenantHasRegisteredConfig = "TenantHasRegisteredConfig";
    public const string TenantHasTwoFactorAuthenticationEnabled = "TenantHasTwoFactorAuthenticationEnabled";

    public const string TenantHasSsoOnly = "TenantHasSsoOnly";
    public const string TenantHasSsoOptional = "TenantHasSsoOptional";
    public const string TenantHasSso = "TenantHasSso";

    public const string UserShouldHaveCompleteProfile = "UserShouldHaveCompleteProfile";


    public const string UserAccessedWithLoginAndPassword = "UserAccessedWithLoginAndPassword";
    public const string UserIsSupervisor = "UserIsSupervisor";
    public const string UserIsSupervisorWithRegisteredConfig = "UserIsSupervisorWithRegisteredConfig";
    public const string UserHasNotAccessWithLoginCode = "UserHasNotAccessWithLoginCode";
    public const string UserHasAccessWithLoginCode = "UserHasAccessWithLoginCode";

    public const string UserIsNotLoggedInForSimpleAnonymousInsert = "UserIsNotLoggedInForSimpleAnonymousInsert";
    public const string IfUserHasLoginCodeMustMatchCurrentItem = "IfUserHasLoginCodeMustMatchCurrentItem";

    public const string UserIsOnlyAdminTenant = "UserIsAdminTenantOnly";

    public const string UserIsAdmin = "UserIsAdmin";
    public const string UserIsAdminOnly = "UserIsAdminOnly";
    public const string UserIsAdminTenant = "UserIsAdminTenant";
    public const string UserIsAdminApplication = "UserIsAdminApplication";

    public const string EnableRegistrationForUsers = "EnableRegistrationForUsers";

    public const string UserLoggedInThroughSso = "UserLoggedInThroughSso";

}