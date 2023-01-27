namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// fornisce accesso a httpcontext e per costruzione a: sessione, temp data, autenticazione e cookies.
/// </summary>
/// <remarks>
/// potrebbe sembrare un servizio scoped ma in realtà è un singleton. 
/// E' possibile quindi definire per questo servizio solo dipendenze singleton,
/// i livelli inferiori causeranno immediatamente una circular reference exception.
/// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
/// </remarks>
public class HttpContextAccessorWeb : HttpContextAccessor, IHttpContextAccessorWeb
{
    private readonly ILogger<HttpContextAccessorWeb> _logger;
    private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
    private readonly IOptions<WebArchitectureSettings> _optWebArchitecture;

    public HttpContextAccessorWeb(
        ILogger<HttpContextAccessorWeb> logger
        , ITempDataDictionaryFactory tempDataDictionaryFactory
        , IOptions<WebArchitectureSettings> optWebArchitecture
        ) : base()
    {
        _logger = logger;
        _tempDataDictionaryFactory = tempDataDictionaryFactory;
        _optWebArchitecture = optWebArchitecture;
    }




    #region get for route parameters (NOT AVAILABLE at authentication phase)

    /// <summary>
    /// WARNING: will be available only AFTER authentication
    /// </summary>
    public string ContextLanguage
    {
        get
        {
            if (base.HttpContext != null)
            {
                return base.HttpContext.Features.Get<IRequestCultureFeature>()
                                                .RequestCulture.Culture.TwoLetterISOLanguageName;
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// WARNING: will be available only AFTER authentication
    /// </summary>
    public string ContextController
    {
        get
        {
            return base.HttpContext != null
                ? base.HttpContext.GetRouteValue(RouteParams.Controller) as string
                : string.Empty;
        }
    }

    /// <summary>
    /// WARNING: will be available only AFTER authentication
    /// </summary>
    public string ContextAction
    {
        get
        {
            return base.HttpContext != null
                ? base.HttpContext.GetRouteValue(RouteParams.Action) as string
                : string.Empty;
        }
    }
    #endregion


    #region route & querystring custom methods and properties

    private IDictionary<string, string> GetRouteUrlGenerationFromContext()
    {
        return
            GetRouteParamsForUrlGeneration(
                ContextAction
                , ContextController
                , ContextRequestQuerystring
                );
    }

    private static IDictionary<string, string> GetRouteParamsForUrlGeneration(
        string action
        , string controller
        , IQueryCollection querystring
        )
    {
        IDictionary<string, string> wbQuerystring = querystring.ToDictionaryApp();

        wbQuerystring.Add(RouteParams.Action, action);
        wbQuerystring.Add(RouteParams.Controller, controller);

        return wbQuerystring;
    }


    private IQueryCollection ContextRequestQuerystring
    {
        get
        {
            return base.HttpContext?.Request?.Query != null ? base.HttpContext.Request.Query : null;
        }
    }

    /// <summary>
    /// build route dictionary using actual route querystring
    /// Further parameters can be added to dictionary after calling this method
    /// ReturnUrl parameter will always be discarded from resulting route
    /// </summary>
    /// <returns></returns>
    public IDictionary<string, string> ContextRequestQuerystringToDictionary()
    {
        return ContextRequestQuerystring.ToDictionaryApp();
    }


    /// <summary>
    /// build new route starting from querystring and forcing language route parameter.
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    public IDictionary<string, string> GetContextRequestQuerystringWithLanguage(string cultureIsoCode)
    {
        IDictionary<string, string> routeVars = ContextRequestQuerystringToDictionary();

        //querystring recovered, now we only need to add language
        routeVars.TryAdd(RouteParams.Language, cultureIsoCode);

        return routeVars;
    }



    #region gestione link back 
    //NOTA: salvataggio url visitati potrebbe essere fatto meglio ma risulterebbe abbastanza complessa da realizzare
    //per il momento questa struttura (chiave per ogni "indietro" del sito)
    //assolve ai nostri scopi in modo semplice

    //salviamo in sessione l'ultima azione fatta
    //utile per creare i link per i bottoni "indietro" 
    //NOTA 1: l'utilizzo deve essere subordinato solo a precise azioni in get
    //NOTA 2: non viene salvato il linguaggio, in modo che venga usato quello di contesto
    //quando viene generato link tramite action
    private void SaveUrlRoute(string savingKey)
    {
        IDictionary<string, string> route =
               GetRouteUrlGenerationFromContext();

        base.HttpContext.Session.Set(savingKey, route);
    }


    //utile per creare azione per indietro
    //linguaggio non è presente nel dizionario
    private void GetUrlRoute(
        string savingKey
        , out string action
        , out string controller
        , out IDictionary<string, string> routeValues //this contains only querystring values
        )
    {
        IDictionary<string, string> route =
            base.HttpContext.Session.Get<IDictionary<string, string>>(savingKey);

        //fallback if session value is null. Wrapping method will assign the intended default values
        if (route is null)
        {
            action = null;
            controller = null;
            routeValues = new Dictionary<string, string>();
        }
        else //session value found, recover values for link construction
        {
            action = route[RouteParams.Action];
            route.Remove(RouteParams.Action);

            controller = route[RouteParams.Controller];
            route.Remove(RouteParams.Controller);

            routeValues = route;
        }
        return;
    }



    private const string SessionNameBackTenantLogin = "BackTenantLogin";

    public void SaveRouteForBackTenantLogin()
    {
        SaveUrlRoute(SessionNameBackTenantLogin);
    }

    public RouteViewModel GetRouteForBackTenantLogin(bool removeTenantTokenFromQuerystring)
    {
        GetUrlRoute(
            SessionNameBackTenantLogin
            , out string action
            , out string controller
            , out IDictionary<string, string> routeValues
            );

        if (removeTenantTokenFromQuerystring
            && routeValues.HasValues()
            && routeValues.ContainsKey(ParamsNames.TenantToken))
        {
            routeValues.Remove(ParamsNames.TenantToken);
        }


        if (action.Empty() || controller.Empty())
        {
            action = MvcComponents.ActLoginTenant;
            controller = MvcComponents.CtrlAccessMain;
            //routeValues is in the worst case an initialized dictionary with no value.
            //it will not be an issue
        }


        return
            new RouteViewModel()
            {
                Controller = controller,
                Action = action,
                QueryStringValues = routeValues,
            };
    }

    public bool IsTenantLoginWithoutLeftPanel()
    {
        RouteViewModel route = GetRouteForBackTenantLogin(false);

        return
            route.Action == MvcComponents.ActLoginTenantNoLogo;
    }



    private const string SessionNameBackItemViewAndManage = "BackItemViewAndManage";
    public void SaveRouteForBackItemViewAndManage()
    {
        SaveUrlRoute(SessionNameBackItemViewAndManage);
    }

    public RouteViewModel GetRouteForBackItemViewAndManage()
    {
        GetUrlRoute(
            SessionNameBackItemViewAndManage
            , out string action
            , out string controller
            , out IDictionary<string, string> routeValues
            );

        if (action.Empty() || controller.Empty())
        {
            return GetRouteForBackTenantLogin(false);
        }


        return
            new RouteViewModel()
            {
                Controller = controller,
                Action = action,
                QueryStringValues = routeValues,
            };
    }



    private const string SessionNameBackUserViewAndManage = "BackUserViewAndManage";
    public void SaveRouteForBackUserViewAndManage()
    {
        SaveUrlRoute(SessionNameBackUserViewAndManage);
    }

    public RouteViewModel GetRouteForBackUserViewAndManage()
    {
        GetUrlRoute(
            SessionNameBackUserViewAndManage
            , out string action
            , out string controller
            , out IDictionary<string, string> routeValues
            );

        if (action.Empty() || controller.Empty())
        {
            return GetRouteForBackTenantLogin(false);
        }


        return
            new RouteViewModel()
            {
                Controller = controller,
                Action = action,
                QueryStringValues = routeValues,
            };
    }
    #endregion


    public RedirectToRouteResult ClearSessionAndCookieAndGetRedirectForLogin()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ClearSessionAndCookieAndGetRedirectForLogin) }
                });

        _logger.LogDebug("CALL");



        RouteViewModel route = GetRouteForBackTenantLogin(false);

        RouteValueDictionary routeToRedirect =
            new(route.QueryStringValues)
            {
                { RouteParams.Controller, route.Controller },
                { RouteParams.Action, route.Action },
            };

        LogoutClearSessionAndPersonalCookies();

        return new RedirectToRouteResult(routeToRedirect);
    }
    #endregion



    private const string SessionNameOperationResult = "OperationResult";
    public OperationResultViewModel SessionOperationResult
    {
        get
        {
            OperationResultViewModel model =
                HttpContext.Session.Get<OperationResultViewModel>(SessionNameOperationResult);

            return model;
        }
        set
        {
            HttpContext.Session.Set(SessionNameOperationResult, value);
        }
    }
    public void SessionRemoveOperationResult()
    {
        HttpContext.Session.Remove(SessionNameOperationResult);
    }



    #region authentication



    /// <summary>
    /// add claims for tenant authentication. This can be considered an "partial" login 
    /// because most of site pages requires also user claims.
    /// Creates authentication cookie with claims
    /// </summary>
    /// <param name="tenantProfile"></param>
    /// <returns></returns>
    public bool SignInTenant(TenantProfileModel tenantProfile)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SignInTenant) }
                });

        _logger.LogInformation("CALL");



        Guard.Against.Null(tenantProfile, nameof(tenantProfile));

        IList<Claim> tenantClaims = MapTenantClaims(tenantProfile);

        return
            NewSignInAndBindToSession(tenantClaims);
    }

    private static IList<Claim> MapTenantClaims(TenantProfileModel tenantProfile)
    {
        Guid sessionUserKey = Guid.NewGuid();//bind authentication to session to prevent fixation

        IList<Claim> tenantClaims =
            new List<Claim>()
            {
                new Claim(ClaimsKeys.Identifier, sessionUserKey.ToString()),
                new Claim(ClaimsKeys.TenantProfile, JsonSerializer.Serialize(tenantProfile)),
            };

        //set up additional claims to handle policies in the simplest way possible
        switch (tenantProfile.Type)
        {
            case ConfigurationType.Anonymous:

                tenantClaims.Add(
                    new Claim(ClaimsKeys.TenantHasAnonymousConfig, "S")
                    );
                break;


            case ConfigurationType.Registered:

                tenantClaims.Add(
                    new Claim(ClaimsKeys.TenantHasRegisteredConfig, "S")
                    );

                if (tenantProfile.DisableRegistrationForUsers)
                {
                    tenantClaims.Add(
                        new Claim(ClaimsKeys.DisableRegistrationForUsers, "S")
                        );
                }
                break;
        }

        if (tenantProfile.TwoFactorAuthenticationEnabled)
        {
            tenantClaims.Add(
                new Claim(ClaimsKeys.TenantHasTwoFactorAuthenticationEnabled, "S")
                );
        }


        switch (tenantProfile.SsoLoginMode)
        {
            case SsoLoginMode.OnlyThroughSso:

                tenantClaims.Add(
                    new Claim(ClaimsKeys.TenantHasSsoOnly, "S")
                    );
                break;

            case SsoLoginMode.SsoOptional:

                tenantClaims.Add(
                    new Claim(ClaimsKeys.TenantHasSsoOptional, "S")
                    );
                break;
        }

        return tenantClaims;
    }



    /// <summary>
    /// add claims for complete login and update authentication cookie information
    /// </summary>
    /// <param name="userProfile"></param>
    /// <returns></returns>
    public bool SignInUser(UserProfileModel userProfile)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SignInUser) }
                });

        _logger.LogInformation("CALL");



        Guard.Against.Null(userProfile, nameof(userProfile));

        AddInfoToSignedInIdentity(MapUserClaims(userProfile));

        return true;
    }

    private static IList<Claim> MapUserClaims(UserProfileModel userProfile)
    {
        IList<Claim> userClaims =
            new List<Claim>
            {
                new Claim(
                    ClaimsKeys.UserProfile
                    , JsonSerializer.Serialize(userProfile)
                    )
            };

        //set up additional claims to handle policies in the simplest way possible
        switch (userProfile.AccessType)
        {
            case AccessType.BasicRoleUserAnonymousForInsert:
                userClaims.Add(new Claim(ClaimsKeys.BasicRoleUserAnonymousForInsert, "S"));
                break;

            case AccessType.BasicRoleUserAnonymousWithLoginCode:
                userClaims.Add(new Claim(ClaimsKeys.BasicRoleUserAnonymousWithLoginCode, "S"));
                userClaims.Add(
                    new Claim(
                        ClaimsKeys.ItemIdFromLoginCode
                        , userProfile.ItemIdFromLoginCode.ToString()
                        )
                    );//così possiamo validare in userpolicy
                break;

            case AccessType.BasicRoleUserRegistered:
                userClaims.Add(new Claim(ClaimsKeys.BasicRoleUserRegistered, "S"));
                break;

            case AccessType.SupervisorWithAnonymousConfig:
                userClaims.Add(new Claim(ClaimsKeys.SupervisorWithAnonymousConfig, "S"));
                break;

            case AccessType.SupervisorWithRegisteredConfig:
                userClaims.Add(new Claim(ClaimsKeys.SupervisorWithRegisteredConfig, "S"));
                break;

            default:
                throw new PmWebException($"Unrecognized {nameof(userProfile.AccessType)} '{userProfile.AccessType}' ");
        }

        if (userProfile.IsAlsoAdminTenant)
        {
            if (userProfile.AssignedSupervisorRolesList.IsNullOrEmpty())
            {
                userClaims.Add(new Claim(ClaimsKeys.UserIsOnlyAdminTenant, "S"));
            }
            else
            {
                userClaims.Add(new Claim(ClaimsKeys.UserIsAlsoAdminTenant, "S"));
            }
        }

        if (userProfile.ExclusiveRoleType == ExclusiveRole.AdminApplication)
        {
            userClaims.Add(new Claim(ClaimsKeys.UserIsAdminApplication, "S"));
        }

        return userClaims;
    }



    public bool SignInSso(
        IEnumerable<Claim> ssoClaims
        , TenantProfileModel tenantProfile
        , UserProfileModel userProfile
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SignInSso) }
                });

        _logger.LogInformation("CALL");



        Guard.Against.Null(tenantProfile, nameof(tenantProfile));

        Guard.Against.Null(userProfile, nameof(userProfile));

        IList<Claim> tenantClaims = MapTenantClaims(tenantProfile);
        IList<Claim> userClaims = MapUserClaims(userProfile);

        IList<Claim> claims = tenantClaims.Concat(userClaims).ToList();

        //save also ssoClaims, necessary for logout
        claims = claims.Concat(ssoClaims).ToList();

        //also a claim type in case we need only the sso claims list
        claims.Add(
            new Claim(
                ClaimsKeys.SsoClaimTypesList
                , JsonSerializer.Serialize(ssoClaims.Select(sc => sc.Type))
                )
            );

        return
            NewSignInAndBindToSession(claims);
    }



    /// <summary>
    /// remove claims and implicitly invalidate/delete authentication cookie
    /// </summary>
    private void SignOut()
    {
        if (base.HttpContext != null)
        {
            AsyncHelper.RunSync(async () =>
                await base.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme
                    )
                    .ConfigureAwait(false)
                );
        }
    }

    private static Guid GetSessionIdentifier(IList<Claim> tenantClaims)
    {
        IEnumerable<string> results =
            tenantClaims.Where(c => c.Type == ClaimsKeys.Identifier)
                        .Select(c => c.Value);

        if (results.IsNullOrEmpty())
        {
            throw new PmWebException($"Claim type '{ClaimsKeys.Identifier}' not found");
        }


        string rawGuid = results.First();

        if (rawGuid.Empty() || !Guid.TryParse(rawGuid, out Guid sessionIdentifier))
        {
            throw new PmWebException($"Session identifier is empty or is not a Guid, a Guid value must be provided");
        }

        return sessionIdentifier;
    }

    private bool NewSignInAndBindToSession(IList<Claim> claims)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(NewSignInAndBindToSession) }
                });

        _logger.LogDebug("CALL - create new auth");



        AuthenticationProperties authProperties =
            new()
            {
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow,
            };


        AsyncHelper.RunSync(async () =>
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
                , new ClaimsPrincipal(
                        new ClaimsIdentity(
                        claims
                        , CookieAuthenticationDefaults.AuthenticationScheme
                        )
                    )
                , authProperties
                ).ConfigureAwait(false)
            );


        SessionIdentifier = GetSessionIdentifier(claims);

        _logger.LogInformation(
            "signed in - session id {SessionIdentifier}"
            , SessionIdentifier
            );

        return true;
    }

    private bool AddInfoToSignedInIdentity(IList<Claim> claims)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(AddInfoToSignedInIdentity) }
                });



        AuthenticateResult authResult =
            AsyncHelper.RunSync(async () =>
                (await HttpContext.AuthenticateAsync()
                                 .ConfigureAwait(false))
                                 );

        if (!authResult.Succeeded)
        {
            _logger.LogError("authentication for claims adding failed");

            return false;
        }

        ClaimsIdentity claimsIdentity = (ClaimsIdentity)authResult.Principal.Identity;

        claimsIdentity.AddClaims(claims);//now authResult.Principal has userClaims

        AsyncHelper.RunSync(
            async () =>
                await HttpContext.SignInAsync(
                       authResult.Principal
                       , authResult.Properties
                       ).ConfigureAwait(false)
            );

        _logger.LogInformation("sign in to existing success");

        return true;
    }
    #endregion


    public bool IsLoggedOrHasSession()
    {
        return HttpContext != null
            && (HttpContext.User.Identity.IsAuthenticated
                || HttpContext.Session.Keys.HasValues());
    }



    #region cleanup session and cookie 

    //errors in tempdata need to be preserved
    public void LogoutClearSessionAndPersonalCookies()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(LogoutClearSessionAndPersonalCookies) }
                });

        _logger.LogDebug("CALL");



        if (base.HttpContext.User.Identity.IsAuthenticated)
        {
            SignOut();//this always removes identity cookie
        }

        //IMPORTANT: we can't delete session if a message has to be displayed
        if (SessionOperationResult is null)
        {
            ClearSessionAndInvalidateSessionCookie();
        }
        //else clear all session apart operation result
    }

    private void ClearSessionAndInvalidateSessionCookie()
    {
        if (base.HttpContext != null)
        {
            if (base.HttpContext?.Session != null)
            {
                base.HttpContext.Session?.Clear();
            }

            InvalidateCookie(_optWebArchitecture.Value.CookiesPrefix + CookieConstants.SessionName, httpOnly: true);
        }
    }
    #endregion


    #region cookie generic methods

    private string ReadCookie(string cookieName)
    {
        return base.HttpContext.Request.Cookies[cookieName];
    }

    private void SetCookie(string cookieName, DateTime expiration, bool httpOnly)
    {
        CookieOptions option = new();

        if (base.HttpContext?.Request.Cookies[cookieName] != null)
        {
            option.Expires = expiration;
            option.HttpOnly = httpOnly;
            option.SameSite = SameSiteMode.Strict;
            option.Secure = true;

            base.HttpContext?.Response.Cookies.Append(cookieName, string.Empty, option);
        }

        return;
    }

    /// <summary>
    /// this method is safer instead of cookie Delete method.
    /// Sometimes browser does not delete properly cookie. 
    /// Setting cookie expired ensures that it will be overridden 
    /// </summary>
    /// <param name="cookieName"></param>
    /// <param name="httpOnly"></param>
    private void InvalidateCookie(string cookieName, bool httpOnly)
    {
        SetCookie(cookieName, DateTime.Now.AddDays(-1), httpOnly);

        return;
    }
    #endregion


    public bool IsBannerCookiePolicyDismissed()
    {
        //exceptions in banner showing.
        //After sso calls, cookie is not available, so it's better to not try detection and don't show banner at all
        if (ContextController.EqualsInvariant(MvcComponents.CtrlAccessSso)
            && ContextAction.EqualsInvariant(MvcComponents.ActLoggedOut))
        {
            return true;
        }


        if (ContextController.EqualsInvariant(MvcComponents.CtrlAccessSso)
            && ContextAction.EqualsInvariant(MvcComponents.ActConsumeAssertion))
        {
            return true;
        }

        string val = ReadCookie(_optWebArchitecture.Value.CookiesPrefix + CookieConstants.BannerDismissed);


        return
            val.StringHasValue() && val == CookieConstants.ValueTrue;
    }



    private const string SessionNameIdentifier = "SessionNameIdentifier";
    /// <summary>
    /// used for validation of session with authentication cookie
    /// </summary>
    public Guid SessionIdentifier
    {
        get
        {
            return base.HttpContext.Session.GetGuid(SessionNameIdentifier);
        }
        set
        {
            base.HttpContext.Session.SetGuid(SessionNameIdentifier, value);
        }
    }



    #region other application session properties


    private const string SessionNameProcessId = "SessionNameProcessId";
    public long SessionProcessId
    {
        get
        {
            return base.HttpContext.Session.GetLongSafe(SessionNameProcessId);
        }
        set
        {
            base.HttpContext.Session.SetInt64(SessionNameProcessId, value);
        }
    }


    private const string SessionNameProcessLogoFileName = "SessionNameProcessLogoFileName";
    public string SessionProcessLogoFileName
    {
        get
        {
            return base.HttpContext.Session.GetString(SessionNameProcessLogoFileName);
        }
        set
        {
            base.HttpContext.Session.SetString(SessionNameProcessLogoFileName, value);
        }
    }


    private const string SessionNameHasSingleProcessConfiguration = "SessionNameHasSingleProcessConfiguration";
    public bool SessionHasSingleProcessConfiguration
    {
        get
        {
            return base.HttpContext.Session.GetBooleanSafe(SessionNameHasSingleProcessConfiguration);
        }
        set
        {
            base.HttpContext.Session.SetBoolean(SessionNameHasSingleProcessConfiguration, value);
        }
    }


    private const string SessionNameItemIdCurrentlyManagedByUser =
        "SessionNameItemIdCurrentlyManagedByUser";
    public long SessionItemIdCurrentlyManagedByUser
    {
        get
        {
            return base.HttpContext.Session.GetLongSafe(SessionNameItemIdCurrentlyManagedByUser);
        }
        set
        {
            base.HttpContext.Session.SetInt64(SessionNameItemIdCurrentlyManagedByUser, value);
        }
    }
    #endregion




    #region temp data properties and methods


    //create properties, don't use this object outside this class
    private ITempDataDictionary TempData
    {
        get
        {
            return _tempDataDictionaryFactory.GetTempData(base.HttpContext);
        }
    }

    #region account temp data properties

    private const string TempDataNameLoginUser = "TempDataLoginUser";
    public UserLoginViewModel TempDataOnceLoginUser
    {
        get
        {
            return
                TempData.Get<UserLoginViewModel>(TempDataNameLoginUser);
        }
        set
        {
            TempData.Put(TempDataNameLoginUser, value);
        }
    }


    private const string TempDataNameUserProfile = "TempDataUserProfile";
    public UserProfileModel TempDataOnceUserProfile
    {
        get
        {
            return
                TempData.Get<UserProfileModel>(TempDataNameUserProfile);
        }
        set
        {
            TempData.Put(TempDataNameUserProfile, value);
        }
    }


    private const string TempDataNameChangePasswordResult =
        "TempDataChangePasswordResult";
    public UserChangePasswordResultViewModel TempDataOnceChangePasswordResult
    {
        get
        {
            return
                TempData.Get<UserChangePasswordResultViewModel>(TempDataNameChangePasswordResult);
        }
        set
        {
            TempData.Put(TempDataNameChangePasswordResult, value);
        }
    }


    private const string TempDataNameRecoverResult = "TempDataRecoverResult";
    /// <summary>
    /// save data so it survives next redirect only once
    /// </summary>
    public bool TempDataOnceRecoverSuccess
    {
        get
        {
            return
                TempData.GetBooleanSafe(TempDataNameRecoverResult);
        }
        set
        {
            TempData.SetBoolean(TempDataNameRecoverResult, value);
        }
    }


    private const string TempDataNameRegister = "TempDataRegister";
    /// <summary>
    /// save data so it survives next redirect only once
    /// </summary>
    public UserRegistrationViewModel TempDataOnceRegister
    {
        get
        {
            return
                TempData.Get<UserRegistrationViewModel>(TempDataNameRegister);
        }
        set
        {
            TempData.Put(TempDataNameRegister, value);
        }
    }


    private const string TempDataNameProblemReport = "TempDataProblemReport";
    /// <summary>
    /// save data so it survives next redirect only once
    /// </summary>
    public ProblemReportViewModel TempDataOnceProblemReport
    {
        get
        {
            return
                TempData.Get<ProblemReportViewModel>(TempDataNameProblemReport);
        }
        set
        {
            TempData.Put(TempDataNameProblemReport, value);
        }
    }


    private const string TempDataNameUserNewSupervisorSave =
        "TempDataUserNewSupervisorSave";
    public UserNewSupervisorSaveViewModel TempDataOnceUserNewSupervisorSave
    {
        get
        {
            return
                TempData.Get<UserNewSupervisorSaveViewModel>(TempDataNameUserNewSupervisorSave);
        }
        set
        {
            TempData.Put(TempDataNameUserNewSupervisorSave, value);
        }
    }

    #endregion
    #endregion
}