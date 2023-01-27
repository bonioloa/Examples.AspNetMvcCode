namespace Comunica.ProcessManager.Web.Code;

public static class ClaimsKeys
{
    public const string Identifier = "Identifier";

    public const string TenantProfile = "TenantProfile";
    public const string TenantHasAnonymousConfig = "TenantHasAnonymousConfig";
    public const string TenantHasRegisteredConfig = "TenantHasRegisteredConfig";
    public const string TenantHasTwoFactorAuthenticationEnabled =
        "TenantHasTwoFactorAuthenticationEnabled";

    public const string TenantHasSsoOnly = "TenantHasSsoOnly";
    public const string TenantHasSsoOptional = "TenantHasSsoOptional";

    public const string UserProfile = "UserProfile";
    public const string BasicRoleUserAnonymousForInsert = "BasicRoleUserAnonymousForInsert";

    public const string BasicRoleUserAnonymousWithLoginCode =
        "BasicRoleUserAnonymousWithLoginCode";
    public const string IdItemFromLoginCode = "IdItemFromLoginCode";

    public const string BasicRoleUserRegistered = "BasicRoleUserRegistered";
    public const string SupervisorWithAnonymousConfig = "SupervisorWithAnonymousConfig";
    public const string SupervisorWithRegisteredConfig = "SupervisorWithRegisteredConfig";

    public const string UserIsAdmin = "UserIsAdmin";
    public const string UserIsAdminOnly = "UserIsAdminOnly";

    public const string DisableRegistrationForUsers = "DisableRegistrationForUsers";

    public const string SsoClaimTypesList = "SsoClaimTypesList";
}
