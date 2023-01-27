namespace Examples.AspNetMvcCode.Web.Code;

public class AuthorizationCustomWeb : IAuthorizationCustomWeb
{
    private readonly ILogger<AuthorizationCustomWeb> _logger;
    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;
    private readonly IAuthorizationService _authorizationService;

    public AuthorizationCustomWeb(
        ILogger<AuthorizationCustomWeb> logger
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IAuthorizationService authorizationService
        )
    {
        _logger = logger;
        _webHttpContextAccessor = webHttpContextAccessor;
        _authorizationService = authorizationService;
    }




    private async Task<AuthorizationResult> InnerCheckPolicyAsync(string policyName)
    {
        return
            await _authorizationService.AuthorizeAsync(
                        _webHttpContextAccessor.HttpContext.User
                        , policyName
                        ).ConfigureAwait(false);
    }



    private async Task<AuthorizationResult> InnerTenantHasRegisteredConfigAsync()
    {
        return await InnerCheckPolicyAsync(PoliciesKeys.TenantHasRegisteredConfig).ConfigureAwait(false);
    }
    public async Task<bool> TenantHasRegisteredConfigAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerTenantHasRegisteredConfigAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }



    private async Task<AuthorizationResult> InnerEnableRegistrationForUsersAsync()
    {
        return await InnerCheckPolicyAsync(PoliciesKeys.EnableRegistrationForUsers).ConfigureAwait(false);
    }
    public async Task<bool> EnableRegistrationForUsersAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerEnableRegistrationForUsersAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }



    private async Task<AuthorizationResult> InnerTenantHasAnonymousConfigAsync()
    {
        return await InnerCheckPolicyAsync(PoliciesKeys.TenantHasAnonymousConfig).ConfigureAwait(false);
    }
    public async Task<bool> TenantHasAnonymousConfigAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerTenantHasAnonymousConfigAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }



    private async Task<AuthorizationResult> InnerTenantHasSsoOptionalAsync()
    {
        return await InnerCheckPolicyAsync(PoliciesKeys.TenantHasSsoOptional).ConfigureAwait(false);
    }
    public async Task<bool> TenantHasSsoOptionalAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerTenantHasSsoOptionalAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }



    private async Task<AuthorizationResult> InnerTenantHasSsoAsync()
    {
        return
            await InnerCheckPolicyAsync(PoliciesKeys.TenantHasSso).ConfigureAwait(false);
    }
    public async Task<bool> TenantHasSsoAsync()
    {
        AuthorizationResult authorizationResult = await InnerTenantHasSsoAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }


    private async Task<AuthorizationResult> InnerUserAccessedWithLoginAndPasswordAsync()
    {
        return
            await InnerCheckPolicyAsync(PoliciesKeys.UserAccessedWithLoginAndPassword).ConfigureAwait(false);
    }
    public async Task<bool> UserAccessedWithLoginAndPasswordAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerUserAccessedWithLoginAndPasswordAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }



    private async Task<AuthorizationResult> InnerUserIsSupervisorAsync()
    {
        return await InnerCheckPolicyAsync(PoliciesKeys.UserIsSupervisor).ConfigureAwait(false);
    }
    public async Task<bool> UserIsSupervisorAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerUserIsSupervisorAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }



    public async Task<bool> UserIsNotLoggedOrIsNotSupervisorAsync()
    {
        return
            !_webHttpContextAccessor.HttpContext.User.Identity.IsAuthenticated
            || !await UserIsSupervisorAsync().ConfigureAwait(false);
    }



    private async Task<AuthorizationResult> InnerUserIsSupervisorWithRegisteredConfigAsync()
    {
        return await InnerCheckPolicyAsync(PoliciesKeys.UserIsSupervisorWithRegisteredConfig).ConfigureAwait(false);
    }
    public async Task<bool> UserIsSupervisorWithRegisteredConfigAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerUserIsSupervisorWithRegisteredConfigAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }


    private async Task<AuthorizationResult> InnerUserIsAdminAsync()
    {
        return
            await InnerCheckPolicyAsync(PoliciesKeys.UserIsAdmin).ConfigureAwait(false);
    }
    public async Task<bool> UserIsAdminAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerUserIsAdminAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }



    private async Task<AuthorizationResult> InnerUserIsAdminOnlyAsync()
    {
        return
           await InnerCheckPolicyAsync(PoliciesKeys.UserIsAdminOnly).ConfigureAwait(false);
    }
    public async Task<bool> UserIsAdminOnlyAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerUserIsAdminOnlyAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }



    private async Task<AuthorizationResult> InnerUserIsAdminTenantAsync()
    {
        return await InnerCheckPolicyAsync(PoliciesKeys.UserIsAdminTenant).ConfigureAwait(false);
    }
    public async Task<bool> UserIsAdminTenantAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerUserIsAdminTenantAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }


    private async Task<AuthorizationResult> InnerUserIsAdminApplicationAsync()
    {
        return await InnerCheckPolicyAsync(PoliciesKeys.UserIsAdminApplication).ConfigureAwait(false);
    }
    public async Task<bool> UserIsAdminApplicationAsync()
    {
        AuthorizationResult authorizationResult =
            await InnerUserIsAdminApplicationAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }



    private async Task<AuthorizationResult> InnerUserHasAccessWithLoginCodeAsync()
    {
        return
            await InnerCheckPolicyAsync(PoliciesKeys.UserHasAccessWithLoginCode).ConfigureAwait(false);
    }



    private async Task<AuthorizationResult> InnerUserLoggedInThroughSsoAsync()
    {
        return
            await InnerCheckPolicyAsync(PoliciesKeys.UserLoggedInThroughSso).ConfigureAwait(false);
    }
    public async Task<bool> UserLoggedInThroughSsoAsync()
    {
        AuthorizationResult authorizationResult = await InnerUserLoggedInThroughSsoAsync().ConfigureAwait(false);

        return authorizationResult.Succeeded;
    }



    /// <summary>
    /// Decide appropriate landing page route by role
    /// </summary>
    /// <param name="itemIdFromLoginCode">optional, needed only for one role</param>
    /// <param name="languageIso">optional; override language. Use only with caution</param>
    /// <returns></returns>
    public RouteValueDictionary GetLandingPageByRole(
        long? itemIdFromLoginCode
        , string languageIso
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetLandingPageByRole) }
                });



        RouteViewModel routeModel =
            GetLandingPageRouteByRole(
                itemIdFromLoginCode
                , languageIso
                );

        Guard.Against.Null(routeModel, nameof(routeModel));

        RouteValueDictionary route =
            new()
            {
                { RouteParams.Controller, routeModel.Controller },
                { RouteParams.Action, routeModel.Action }
            };

        if (routeModel.QueryStringValues.HasValues())
        {
            foreach (string key in routeModel.QueryStringValues.Keys)
            {
                route.Add(key, routeModel.QueryStringValues[key]);
            }
        }

        return route;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId">mandatory only if user is getting access using his anonymous report login code</param>
    /// <param name="languageIso"></param>
    /// <returns></returns>
    public RouteViewModel GetLandingPageRouteByRole(
        long? itemId
        , string languageIso
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetLandingPageRouteByRole) }
                });


        RouteViewModel routeModel =
            new()
            {
                QueryStringValues = new Dictionary<string, string>()
            };


        if (languageIso.StringHasValue())
        {
            routeModel.QueryStringValues.Add(RouteParams.Language, languageIso);
        }


        if (UserIsAdminOnly())
        {
            _logger.LogInformation("generating page for admin only");

            routeModel.Controller = MvcComponents.CtrlAdministration;
            routeModel.Action = MvcComponents.ActAvailableActions;

            return routeModel;
        }


        if (UserIsSupervisor())
        {
            _logger.LogInformation("generating page for supervisor");

            routeModel.Controller = MvcComponents.CtrlSearch;
            routeModel.Action = MvcComponents.ActSearchNew;

            return routeModel;
        }


        if (UserHasAccessWithLoginCode())
        {
            _logger.LogInformation("generating page for user with login code");

            if (itemId.Invalid())
            {
                _logger.LogInformation("id item not positive log for user with login code role");
            }

            routeModel.Controller = MvcComponents.CtrlItemManagement;
            routeModel.Action = MvcComponents.ActViewAndManage;

            routeModel.QueryStringValues.Add(ParamsNames.ItemId, itemId.ToString());

            return routeModel;
        }

        //every other role
        _logger.LogInformation("generating welcome page");

        routeModel.Controller = MvcComponents.CtrlProcesses;
        routeModel.Action = MvcComponents.ActWelcome;

        return routeModel;
    }
}