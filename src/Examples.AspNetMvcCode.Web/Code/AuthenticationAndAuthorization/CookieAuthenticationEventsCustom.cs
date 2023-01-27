namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// every call validates tenant authentication cookie data. 
/// User authentication will be performed by a specific policy.
/// </summary>
/// <remarks>WARNING: route data can't be used here, 
/// data are available starting from filters</remarks>
public class CookieAuthenticationEventsCustom : CookieAuthenticationEvents
{
    private readonly ILogger<CookieAuthenticationEventsCustom> _logger;
    private readonly ContextApp _contextApp;
    private readonly ContextUser _contextUser;

    private readonly ITenantConfiguratorLogic _logicTenantConfigurator;
    private readonly IUserConfiguratorLogic _logicUserConfigurator;

    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;
    private readonly IMainLocalizer _localizer;
    private readonly ICultureMapperWeb _webCultureMapper;

    public CookieAuthenticationEventsCustom(
        ILogger<CookieAuthenticationEventsCustom> logger
        , ContextApp contextApp
        , ContextUser contextUser
        , ITenantConfiguratorLogic logicTenantConfigurator
        , IUserConfiguratorLogic logicUserConfigurator
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IMainLocalizer localizer
        , ICultureMapperWeb webCultureMapper
        )
    {
        _logger = logger;
        _contextApp = contextApp;
        _contextUser = contextUser;
        _logicTenantConfigurator = logicTenantConfigurator;
        _logicUserConfigurator = logicUserConfigurator;
        _webHttpContextAccessor = webHttpContextAccessor;
        _localizer = localizer;
        _webCultureMapper = webCultureMapper;
    }


    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ValidatePrincipal) }
                });

        _logger.LogDebug("CALL");



        IEnumerable<Claim> claims = context?.Principal?.Claims;

        if (!ValidateIdentifierClaim(claims))
        {
            await DenyAuthenticationAsync(context).ConfigureAwait(false);
            return;
        }


        TenantProfileLgc authenticatedTenantProfile = DeserializeToTenantProfile(claims);

        if (authenticatedTenantProfile is null)
        {
            await DenyAuthenticationAsync(context).ConfigureAwait(false);
            return;
        }

        //initialize here because authentication is called before globalfilter
        //and this property is required in the following execution
        _contextApp.AppSupportedCulturesIsoCodes = _webCultureMapper.GetAppSupportedCulturesList();
        //also we need that requested culture is configured in _contextApp if valid
        //eventual redirect will be handled by globalfilter (executed after this method)
        _webCultureMapper.SetCultureAndDetectIfRedirectNeeded();

        //validate profile data saved in auth cookie against current db data
        OperationResultLgc resultTenant =
            _logicTenantConfigurator.ValidateProfileAndSetTenantContext(
                authenticatedTenantProfile
                , _webHttpContextAccessor.HttpContext.Connection.RemoteIpAddress
                );

        if (!resultTenant.Success)
        {
            _logger.LogWarning("tenant validation fail");

            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel()
                {
                    LocalizedMessage = _localizer[nameof(LocalizedStr.SharedErrorAccessDenied)]
                };

            await DenyAuthenticationAsync(context).ConfigureAwait(false);
            return;
        }

        //very important
        //perform validation of user profile only if we find the relative claim.
        //To enforce the claim check use the authorization policy on action and controllers
        //that need userprofile. Policy will fail authorization if claim is not found.
        UserProfileLgc authenticatedUserProfile = DeserializeToUserProfile(claims);

        if (authenticatedUserProfile is null)
        {
            return; //EXIT -  use policy on controller/action for enforcing existence of user context where needed 
        }

        //validate profile data saved in auth cookie against current db data
        UserProfileValidationLgc resultUser =
            _logicUserConfigurator.ValidateUserProfile(authenticatedUserProfile);

        if (!resultUser.Success)
        {
            _logger.LogWarning("user validation fail");

            string message =
                resultUser.UserSsoRequiresPermissions
                ? _localizer[nameof(LocalizedStr.SsoWarnUserRequiresRoles)]
                : _localizer[nameof(LocalizedStr.SharedErrorAccessDenied)];

            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel()
                {
                    LocalizedMessage = message,
                };

            await DenyAuthenticationAsync(context).ConfigureAwait(false);
            return;
        }

        //the following context properties are assigned here because
        //they are needed only in reserved area
        _contextUser.ProcessId = _webHttpContextAccessor.SessionProcessId;
        _contextUser.ItemIdCurrentlyManagedByUser = _webHttpContextAccessor.SessionItemIdCurrentlyManagedByUser;
    }


    #region private methods for validatePricipal

    //use warnings because they are not errors
    private bool ValidateIdentifierClaim(IEnumerable<Claim> claims)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ValidateIdentifierClaim) }
                });



        IEnumerable<string> claimIdentifierQueried =
           from c in claims
           where c.Type == ClaimsKeys.Identifier
           select c.Value;

        if (claimIdentifierQueried.IsNullOrEmpty())
        {
            _logger.LogWarning("identifier claim not found");
            return false;
        }


        string claimIdentifierStr = claimIdentifierQueried.FirstOrDefault();

        if (claimIdentifierStr.Empty())
        {
            _logger.LogWarning("identifier claim empty");
            return false;
        }


        Guid sessionIdentifier = _webHttpContextAccessor.SessionIdentifier;

        if (sessionIdentifier == Guid.Empty)
        {
            _logger.LogWarning("session identifier empty");
            return false;
        }

        if (Guid.TryParse(claimIdentifierStr, out Guid claimIdentifier)
            && claimIdentifier == sessionIdentifier)
        {
            return true;
        }

        _logger.LogWarning(
            "authentication fail! Claim identifier '{ClaimIdentifier}' vs session identifier '{SessionIdentifier}' mismatch"
            , claimIdentifier
            , sessionIdentifier
            );
        return false;
    }


    private TenantProfileLgc DeserializeToTenantProfile(IEnumerable<Claim> claims)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(DeserializeToTenantProfile) }
                });
        string tenantProfileClaimName = nameof(ClaimsKeys.TenantProfile);



        string profileSerialized =
            (from c in claims
             where c.Type == ClaimsKeys.TenantProfile
             select c.Value
             )
             .FirstOrDefault();

        if (profileSerialized.Empty())
        {
            _logger.LogError(
                "{TenantProfileClaimName} claim is empty"
                , tenantProfileClaimName
                );
            return null;
        }

        return
            JsonSerializer.Deserialize<TenantProfileModel>(profileSerialized)
                             .MapFromWebToLogic();
    }



    private static UserProfileLgc DeserializeToUserProfile(IEnumerable<Claim> claims)
    {
        string userProfileSerialized =
            (from c in claims
             where c.Type == ClaimsKeys.UserProfile
             select c.Value
             ).FirstOrDefault();

        return
            userProfileSerialized.Empty()
            ? null //this is an allowed result
            : JsonSerializer.Deserialize<UserProfileModel>(
                    userProfileSerialized
                    )
                    .MapFromWebToLogic();
    }



    private async Task DenyAuthenticationAsync(CookieValidatePrincipalContext context)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(DenyAuthenticationAsync) }
                });



        _logger.LogInformation("authentication rejected");

        context.RejectPrincipal();

        await context.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
                )
                .ConfigureAwait(false);
    }
    #endregion
}