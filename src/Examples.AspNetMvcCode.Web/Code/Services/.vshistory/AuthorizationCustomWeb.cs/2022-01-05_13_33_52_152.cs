namespace Comunica.ProcessManager.Web.Code;

public class AuthorizationCustomWeb : IAuthorizationCustomWeb
{
    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;
    private readonly IAuthorizationService _authorizationService;

    public AuthorizationCustomWeb(
        IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , IAuthorizationService authorizationService
        )
    {
        _httpContextAccessorCustomWeb = httpContextAccessorCustomWeb;
        _authorizationService = authorizationService;
    }

    private async Task<AuthorizationResult> InnerCheckPolicy(string policyName)
    {
        return await _authorizationService.AuthorizeAsync(
                                    _httpContextAccessorCustomWeb.HttpContext.User
                                    , policyName
                                    ).ConfigureAwait(false);
    }



    private async Task<AuthorizationResult> InnerTenantHasRegisteredConfigAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.TenantHasRegisteredConfig).ConfigureAwait(false);
    }
    public async Task<bool> TenantHasRegisteredConfigAsync()
    {
        AuthorizationResult authorizationResult = await InnerTenantHasRegisteredConfigAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }

    private async Task<AuthorizationResult> InnerEnableRegistrationForUsersAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.EnableRegistrationForUsers).ConfigureAwait(false);
    }
    public async Task<bool> EnableRegistrationForUsersAsync()
    {
        AuthorizationResult authorizationResult = await InnerEnableRegistrationForUsersAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }


    private async Task<AuthorizationResult> InnerTenantHasAnonymousConfigAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.TenantHasAnonymousConfig).ConfigureAwait(false);
    }
    public async Task<bool> TenantHasAnonymousConfigAsync()
    {
        AuthorizationResult authorizationResult = await InnerTenantHasAnonymousConfigAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }

    private async Task<AuthorizationResult> InnerTenantHasSsoOnlyAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.TenantHasSsoOnly).ConfigureAwait(false);
    }
    public async Task<bool> TenantHasSsoOnlyAsync()
    {
        AuthorizationResult authorizationResult = await InnerTenantHasSsoOnlyAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }

    private async Task<AuthorizationResult> InnerTenantHasSsoOptionalAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.TenantHasSsoOptional).ConfigureAwait(false);
    }
    public async Task<bool> TenantHasSsoOptionalAsync()
    {
        AuthorizationResult authorizationResult = await InnerTenantHasSsoOptionalAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }
    private async Task<AuthorizationResult> InnerTenantHasSsoAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.TenantHasSso).ConfigureAwait(false);
    }
    public async Task<bool> TenantHasSsoAsync()
    {
        AuthorizationResult authorizationResult = await InnerTenantHasSsoAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }


    private async Task<AuthorizationResult> InnerUserAccessedWithLoginAndPasswordAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.UserAccessedWithLoginAndPassword).ConfigureAwait(false);
    }
    public async Task<bool> UserAccessedWithLoginAndPasswordAsync()
    {
        AuthorizationResult authorizationResult = await InnerUserAccessedWithLoginAndPasswordAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }

    private async Task<AuthorizationResult> InnerUserIsSupervisorAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.UserIsSupervisor).ConfigureAwait(false);
    }
    /// <summary>
    /// evaluate from user claims
    /// </summary>
    /// <returns></returns>
    /// <remarks>use this method only for synchronous methods, in views use async versione</remarks>
    public bool UserIsSupervisor()
    {
        AuthorizationResult authResult = AsyncHelper.RunSync(() => InnerUserIsSupervisorAsync());
        return authResult.Succeeded;
    }
    public async Task<bool> UserIsSupervisorAsync()
    {
        AuthorizationResult authorizationResult = await InnerUserIsSupervisorAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }


    public async Task<bool> UserIsNotLoggedOrIsNotSupervisorAsync()
    {
        return !_httpContextAccessorCustomWeb.HttpContext.User.Identity.IsAuthenticated
            || !await UserIsSupervisorAsync().ConfigureAwait(false);
    }


    private async Task<AuthorizationResult> InnerUserIsSupervisorWithRegisteredConfigAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.UserIsSupervisorWithRegisteredConfig).ConfigureAwait(false);
    }
    public async Task<bool> UserIsSupervisorWithRegisteredConfigAsync()
    {
        AuthorizationResult authorizationResult = await InnerUserIsSupervisorWithRegisteredConfigAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }


    private async Task<AuthorizationResult> InnerUserIsAdminAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.UserIsAdmin).ConfigureAwait(false);
    }
    public async Task<bool> UserIsAdminAsync()
    {
        AuthorizationResult authorizationResult = await InnerUserIsAdminAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }
    public bool UserIsAdmin()
    {
        AuthorizationResult authResult = AsyncHelper.RunSync(() => InnerUserIsAdminAsync());
        return authResult.Succeeded;
    }


    private async Task<AuthorizationResult> InnerUserIsAdminOnlyAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.UserIsAdminOnly).ConfigureAwait(false);
    }
    public async Task<bool> UserIsAdminOnlyAsync()
    {
        AuthorizationResult authorizationResult = await InnerUserIsAdminOnlyAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }
    public bool UserIsAdminOnly()
    {
        AuthorizationResult authResult = AsyncHelper.RunSync(() => InnerUserIsAdminOnlyAsync());
        return authResult.Succeeded;
    }

    private async Task<AuthorizationResult> InnerUserHasAccessWithLoginCodeAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.UserHasAccessWithLoginCode).ConfigureAwait(false);
    }
    public bool UserHasAccessWithLoginCode()
    {
        AuthorizationResult authResult = AsyncHelper.RunSync(() => InnerUserHasAccessWithLoginCodeAsync());
        return authResult.Succeeded;
    }


    private async Task<AuthorizationResult> InnerUserLoggedInThroughSsoAsync()
    {
        return await InnerCheckPolicy(PoliciesKeys.UserLoggedInThroughSso).ConfigureAwait(false);
    }
    public async Task<bool> UserLoggedInThroughSsoAsync()
    {
        AuthorizationResult authorizationResult = await InnerUserLoggedInThroughSsoAsync().ConfigureAwait(false);
        return authorizationResult.Succeeded;
    }

    /// <summary>
    /// Decide appropriate landing page route by role
    /// </summary>
    /// <param name="idItemFromLoginCode">optional, needed only for one role</param>
    /// <param name="languageIso">optional; override language. Use only with caution</param>
    /// <returns></returns>
    public RouteValueDictionary GetLandingPageByRole(
        long? idItemFromLoginCode
        , string languageIso
        )
    {
        RouteValueDictionary route = new();
        if (languageIso.StringHasValue())
        {
            route.Add(RouteParams.Language, languageIso);
        }

        if (UserIsAdminOnly())
        {
            Log.Logger.Information($"using {nameof(GetLandingPageByRole)} for admin");
            route.Add(RouteParams.Controller, MvcComponents.CtrlAccountAdministration);
            route.Add(RouteParams.Action, MvcComponents.ActSupervisorManagement);
            return route;
        }

        if (UserIsSupervisor())
        {
            Log.Logger.Information($"using {nameof(GetLandingPageByRole)} for supervisor");
            route.Add(RouteParams.Controller, MvcComponents.CtrlSearch);
            route.Add(RouteParams.Action, MvcComponents.ActSearchNew);
            return route;
        }

        if (UserHasAccessWithLoginCode())
        {
            Log.Logger.Information($"using {nameof(GetLandingPageByRole)} for login with code");
            if (idItemFromLoginCode.Invalid())
            {
                Log.Logger.Error($"{nameof(GetLandingPageByRole)} id item empty for user with login code role");
            }
            route.Add(RouteParams.Controller, MvcComponents.CtrlItemManagement);
            route.Add(RouteParams.Action, MvcComponents.ActViewAndManage);
            route.Add(ParamsNames.IdItem, idItemFromLoginCode.ToString());
            return route;
        }

        //every other role
        Log.Logger.Information($"using {nameof(GetLandingPageByRole)} default");
        route.Add(RouteParams.Controller, MvcComponents.CtrlProcesses);
        route.Add(RouteParams.Action, MvcComponents.ActWelcome);
        return route;
    }
}
