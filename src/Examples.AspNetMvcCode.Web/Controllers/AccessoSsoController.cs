using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.Util;

namespace Examples.AspNetMvcCode.Web.Controllers;

//authorizations must be given by action because some action in this controller must be given anonymous access
[AllowAnonymous]
public class AccessoSsoController : BaseContextController
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ILogger<AccessoSsoController> _logger;
    private readonly ContextApp _contextApp;
    private readonly ContextTenant _contextTenant;
    private readonly ContextUser _contextUser;

    private readonly ICryptingLogic _logicCrypting;
    private readonly ITenantConfiguratorLogic _logicTenantConfigurator;
    private readonly IUserConfiguratorLogic _logicUserConfigurator;
    private readonly IAuditLogic _logicAudit;

    private readonly IUrlHelper _urlHelper;
    private readonly IAuthorizationCustomWeb _webAuthorizationCustom;
    private readonly IMainLocalizer _localizer;
    private readonly IHtmlFormToModelMapperWeb _webHtmlFormToModelMapper;

    public AccessoSsoController(
        IWebHostEnvironment hostingEnvironment
        , ILogger<AccessoSsoController> logger
        , ContextApp contextApp
        , ContextTenant contextTenant
        , ContextUser contextUser
        , ICryptingLogic logicCrypting
        , ITenantConfiguratorLogic logicTenantConfigurator
        , IUserConfiguratorLogic logicUserConfigurator
        , IAuditLogic logicAudit
        , IUrlHelper urlHelper
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IAuthorizationCustomWeb webAuthorizationCustom
        , IMainLocalizer localizer
        , IHtmlFormToModelMapperWeb webHtmlFormToModelMapper
        ) : base(webHttpContextAccessor)
    {
        _hostingEnvironment = hostingEnvironment;
        _logger = logger;
        _contextApp = contextApp;
        _contextTenant = contextTenant;
        _contextUser = contextUser;
        _logicCrypting = logicCrypting;
        _logicTenantConfigurator = logicTenantConfigurator;
        _logicUserConfigurator = logicUserConfigurator;
        _logicAudit = logicAudit;
        _urlHelper = urlHelper;
        _webAuthorizationCustom = webAuthorizationCustom;
        _localizer = localizer;
        _webHtmlFormToModelMapper = webHtmlFormToModelMapper;
    }




    [Authorize]
    [Authorize(Policy = PoliciesKeys.UserShouldHaveTenantProfile)]
    [HttpPost]
    public IActionResult LoginUtenteSso(int ssoConfigId)
    {
        TenantProfileLgc tenantProfile =
            _logicTenantConfigurator.ValidateAndSetTenantContext(
                _contextTenant.Token.Clean()
                , _webHttpContextAccessor.HttpContext.Connection.RemoteIpAddress
                );

        SsoConfigLgc ssoConfig =
            _logicTenantConfigurator.GetSsoConfigByIdOrDefault(ssoConfigId);

        //we also save language because, by our metadata definition,
        //Idp has an already localized url to post after login so we have to save current language here,
        //before delegating authentication to Idp
        Saml2Configuration saml2Configuration = BuildConfigWithIdpInfo(ssoConfig);


        Dictionary<string, string> relayStateQuery =
            new()
            {
                //{ ParamsNames.ReturnUrl, Url.Content("~/") }
                { RouteParams.Language, _contextApp.CurrentCultureIsoCode },
                { ParamsNames.TenantToken, _contextTenant.Token },
                { ParamsNames.SsoConfigId, ssoConfigId.ToString() }
            };


        switch (ssoConfig.MethodSpToIdp)
        {
            case SsoMethodSpToIdp.Get:
                Saml2RedirectBinding redirBinding = new();
                redirBinding.SetRelayStateQuery(relayStateQuery);

                return redirBinding.Bind(new Saml2AuthnRequest(saml2Configuration))
                                   .ToActionResult();


            case SsoMethodSpToIdp.Post:
                Saml2PostBinding postBinding = new();
                postBinding.SetRelayStateQuery(relayStateQuery);

                return postBinding.Bind(new Saml2AuthnRequest(saml2Configuration))
                                  .ToActionResult();

            default:
                throw new PmWebException($"Unrecognized {nameof(SsoMethodSpToIdp)} value '{ssoConfig.MethodSpToIdp}' ");
        }
    }

    //WARNING: inside this method we don't have session and authentication data (sso does not resend cookies)
    //so we must log in again tenant 
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]//this action must ignore antiforgery because call comes from customer auth site (cross site)
    [HttpPost]
    public IActionResult ConsumeAssertion(IFormCollection form)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ConsumeAssertion) }
                });



        SsoLoginModel ssoLoginModel = _webHtmlFormToModelMapper.MapPostSsoStateData(form);

        //we don't use output, call is needed to initialize tenant context
        _logicTenantConfigurator.ValidateAndSetTenantContext(
                ssoLoginModel.Token
                , _webHttpContextAccessor.HttpContext.Connection.RemoteIpAddress
                );

        SsoConfigLgc ssoConfig =
            _logicTenantConfigurator.GetSsoConfigByIdOrDefault(ssoLoginModel.SsoConfigId);

        Saml2AuthnResponse saml2AuthnResponse = new(BuildConfigWithIdpInfo(ssoConfig));

        _logger.LogInformation("auth response created successfully");

        Saml2PostBinding binding = new();
        OperationResultViewModel modelMessage;
        try
        {
            _logger.LogDebug("ReadSamlResponse. Verifying auth status code...");

            //read necessary to get response status code / auth result
            binding.ReadSamlResponse(Request.ToGenericHttpRequest(), saml2AuthnResponse);

            _logger.LogInformation("ReadSamlResponse success");

            if (saml2AuthnResponse != null && saml2AuthnResponse.Status != Saml2StatusCodes.Success)
            {
                Saml2StatusCodes status = saml2AuthnResponse.Status;
                _logger.LogError(
                    "Request error - SAML Response status: {Status}"
                    , status
                    );


                modelMessage =
                    new OperationResultViewModel
                    {
                        Success = false,
                        LocalizedMessage = "Errore autenticazione SAML"
                    };

                _webHttpContextAccessor.SessionOperationResult = modelMessage;

                return BaseRedirectToInitialLoginPage();
            }

            _logger.LogInformation("Auth success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error during ReadSamlResponse");

            modelMessage =
                new OperationResultViewModel
                {
                    Success = false,
                    LocalizedMessage = "Errore autenticazione SAML"
                };

            _webHttpContextAccessor.SessionOperationResult = modelMessage;

            return BaseRedirectToInitialLoginPage();
        }

        //decrypt and deserialize response
        binding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnResponse);


        //log xml response
        //(IMPORTANT, keep DEBUG level in logs
        //so in production is logged only when we need it
        string responseContent = saml2AuthnResponse.ToXml().OuterXml;
        _logger.LogDebug(
            "saml response '{ResponseContent}' "
            , responseContent
            );


        //save deserialized claims
        ssoLoginModel.SsoIdentity = MapIEnumerableClaim(saml2AuthnResponse.ClaimsIdentity.Claims);


        //WARNING: if we sign in here and redirect to a page that requires authorization
        //we will get an authentication error
        //no workaround found apart passing data to a view
        //and repost it with javascript
        string accessData = JsonSerializer.Serialize(ssoLoginModel);



        //encrypt necessary to prevent personal data leak in an unauthenticated page
        string accessDataEncrypted = _logicCrypting.SimpleEncrypt(accessData);
        _logger.LogDebug(
            "encrypted value '{AccessDataEncrypted}' "
            , accessDataEncrypted
            );


        return View(MvcComponents.ViewConsumeAssertion, accessDataEncrypted);
    }

    [NonAction]
    private ClaimModel MapClaim(Claim objIn)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(MapClaim) }
                });



        if (objIn is null)
        {
            string claimName = nameof(Claim);
            _logger.LogError(
                "{ClaimName} is null"
                , claimName
                );
        }

        return
            new ClaimModel
            {
                Type = objIn.Type,
                Properties = objIn.Properties,
                OriginalIssuer = objIn.OriginalIssuer,
                Issuer = objIn.Issuer,
                ValueType = objIn.ValueType,
                Value = objIn.Value,
            };
    }
    [NonAction]
    private IEnumerable<ClaimModel> MapIEnumerableClaim(IEnumerable<Claim> objIn)
    {
        return
            objIn.IsNullOrEmpty() ? new List<ClaimModel>() :
                    objIn.Select(claim => MapClaim(claim))
                         .ToList();
    }



    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [HttpPost]
    public IActionResult PerformSsoLogin(string accessData)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(PerformSsoLogin) }
                });



        _logger.LogInformation(
            "received encrypted'{AccessData}' "
            , accessData
            );


        string accessDataDecrypted = _logicCrypting.SimpleDecrypt(accessData);


        SsoLoginModel ssoLoginModel = JsonSerializer.Deserialize<SsoLoginModel>(accessDataDecrypted);

        _logger.LogInformation("Deserialized successfully");


        TenantProfileLgc tenantProfile =
            _logicTenantConfigurator.ValidateAndSetTenantContext(
                ssoLoginModel.Token
                , _webHttpContextAccessor.HttpContext.Connection.RemoteIpAddress
                );

        SsoConfigLgc ssoConfig =
            _logicTenantConfigurator.GetSsoConfigByIdOrDefault(ssoLoginModel.SsoConfigId);

        IEnumerable<Claim> identityClaims = MapIEnumerableClaimModel(ssoLoginModel.SsoIdentity);

        UserProfileLgc userProfile =
            _logicUserConfigurator.ValidateAndConfigureSsoLogin(
                identityClaims
                , ssoConfig
                );

        TenantProfileModel tenantProfileModel = tenantProfile.MapFromLogicToWeb();
        UserProfileModel userProfileModel = userProfile.MapFromLogicToWeb();


        if (!_webHttpContextAccessor.SignInSso(
                identityClaims
                , tenantProfileModel
                , userProfileModel
                ))
        {
            _logger.LogError("FAIL: sign in SSO");

            return BaseRedirectToInitialLoginPage();
        }


        _logger.LogInformation("SUCCESS: sign in SSO");

        _logicAudit.LogUserLogin();

        return
            RedirectToRoute(
               _webAuthorizationCustom.GetLandingPageByRole(null, ssoLoginModel.LanguageIso)
               );
    }



    [NonAction]
    private Claim MapClaimModel(ClaimModel objIn)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(MapClaimModel) }
                });
        string inputObjectName = nameof(ClaimModel);



        if (objIn is null)
        {
            _logger.LogError(
                "{InputObjectName} is null"
                , inputObjectName
                );
        }


        return
            new Claim(
                type: objIn.Type
                , value: objIn.Value
                , valueType: objIn.ValueType
                , issuer: objIn.Issuer
                , originalIssuer: objIn.OriginalIssuer
                );
    }

    [NonAction]
    private IEnumerable<Claim> MapIEnumerableClaimModel(IEnumerable<ClaimModel> objIn)
    {
        return
            objIn.IsNullOrEmpty() ? new List<Claim>() :
                objIn.Select(claimModel => MapClaimModel(claimModel))
                     .ToList();
    }


    [Authorize]
    [Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
    [HttpGet]
    public IActionResult SingleLogout()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SingleLogout) }
                });

        _logger.LogDebug("CALL");



        SsoConfigLgc ssoConfig =
           _logicTenantConfigurator.GetSsoConfigByIdOrDefault(_contextUser.SsoConfigId);

        Saml2Configuration saml2Configuration = BuildConfigWithIdpInfo(ssoConfig);


        bool emptySingleLogoutDestination =
            saml2Configuration.SingleLogoutDestination is null
            || !Uri.IsWellFormedUriString(saml2Configuration.SingleLogoutDestination.ToString(), UriKind.RelativeOrAbsolute);

        Saml2LogoutRequest remoteLogoutRequest = null;
        if (!emptySingleLogoutDestination)
        {
            //User is safe to pass directly, only session and nameid will be sent to IdP
            remoteLogoutRequest = new(saml2Configuration, User);


            //log xml response
            string requestContent = remoteLogoutRequest.ToXml().OuterXml;
            _logger.LogDebug(
                "sso logout request created '{RequestContent}' "
                , requestContent
                );
        }



        //we are forced to write logout here
        //because we don't have any context in logged out action
        //complete identity logout will happen in logged out
        _logicAudit.LogLogout();

        long userId = _contextUser.UserIdLoggedIn;
        string singleLogoutDestinationName = nameof(saml2Configuration.SingleLogoutDestination);

        _logger.LogInformation(
            "userId '{UserId}' will be logged out; try log out to remote IdP if {SingleLogoutDestinationName} is specified in configuration"
            , userId
            , singleLogoutDestinationName
            );


        if (!emptySingleLogoutDestination)
        {
            //the LoggedOut action will not redirect so we must ensure complete logout
            _webHttpContextAccessor.LogoutClearSessionAndPersonalCookies();

            Saml2PostBinding binding = new();
            return binding.Bind(remoteLogoutRequest).ToActionResult();
        }

        return BaseRedirectToInitialLoginPage();
    }


    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public IActionResult LoggedOut()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(LoggedOut) }
                });

        _logger.LogDebug("CALL");



        //no way to deserialize saml response,
        //relay state from SingleLogout response is not guaranteed
        //so we are not able to find what tenant and what sso Config to use to deserialize response status

        //Saml2Configuration saml2Configuration = BuildConfigWithIdpInfo();
        //var binding = new Saml2PostBinding();
        //binding.Unbind(Request.ToGenericHttpRequest(), new Saml2LogoutResponse(saml2Configuration));

        _logger.LogInformation("SSO logged out LOCALLY");

        return View(MvcComponents.ViewLoggedOut);
    }


    /// <summary>
    /// SP metadata are intended by tenant, even if certificates are all the same,it's
    /// better to keep them separate to implement possible changes without affecting other SSO tenants
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [Authorize(Policy = PoliciesKeys.UserShouldHaveTenantProfile)]
    [HttpGet]
    public IActionResult SpMetadata()
    {
        Saml2Configuration saml2Configuration = BuildBaseConfiguration();

        return
            new Saml2Metadata(
                new EntityDescriptor(saml2Configuration)
                {
                    //set same validity as certificate
                    ValidUntil = (saml2Configuration.SigningCertificate.NotAfter - DateTime.Now).Days,

                    SPSsoDescriptor =
                        new SPSsoDescriptor
                        {
                            WantAssertionsSigned = true,

                            SigningCertificates =
                                new X509Certificate2[]
                                {
                                    saml2Configuration.SigningCertificate
                                },

                            EncryptionCertificates =
                                new X509Certificate2[]
                                {
                                    saml2Configuration.DecryptionCertificate
                                },

                            SingleLogoutServices =
                                new SingleLogoutService[]
                                {
                                    new SingleLogoutService
                                    {
                                        Binding = ProtocolBindings.HttpPost,

                                        Location =
                                            _urlHelper.AbsoluteAction(MvcComponents.ActSingleLogout, MvcComponents.CtrlAccessSso ),

                                        ResponseLocation =
                                            _urlHelper.AbsoluteAction(MvcComponents.ActLoggedOut, MvcComponents.CtrlAccessSso ),
                                    }
                                },

                            NameIDFormats = new Uri[] { NameIdentifierFormats.Unspecified },//NameIdentifierFormats.X509SubjectName

                            AssertionConsumerServices =
                                new AssertionConsumerService[]
                                {
                                    new AssertionConsumerService
                                    {
                                        Binding = ProtocolBindings.HttpPost,//don't allow GET method for this action

                                        Location =
                                            _urlHelper.AbsoluteAction(MvcComponents.ActConsumeAssertion, MvcComponents.CtrlAccessSso ),
                                    }
                                },
                        },

                    ContactPersons =
                        new List<ContactPerson>()
                        {
                            new ContactPerson(ContactTypes.Technical)
                            {
                                Company = "company",
                                GivenName = "companyName",
                                EmailAddress = "support@email",
                            }
                        }
                }
            )
            .CreateMetadata()
            .ToActionResult();
    }




    #region shared private methods

    private const string CertificatesSubpath = @"wwwSsoSpCerts\";

    [NonAction]
    private X509Certificate2 GetSpCertificate()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetSpCertificate) }
                });



        X509Certificate2 ssoCertificate = null;

        string rootPath = _hostingEnvironment.ContentRootPath + CertificatesSubpath;
        try
        {
            string certPath = Path.Combine(rootPath, _contextTenant.SsoSpCertificatePath);

            _logger.LogDebug(
                " cert path '{CertPath}' "
                , certPath
                );

            ssoCertificate = CertificateUtil.Load(certPath, _contextTenant.SsoSpCertificatePassword);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex
                , "ERROR - certificate load fail. RootPath {RootPath}"
                , rootPath
                );
        }

        return ssoCertificate;
    }


    [NonAction]
    private Saml2Configuration BuildBaseConfiguration()
    {
        X509Certificate2 ssoCertificate = GetSpCertificate();

        Saml2Configuration saml2Configuration =
            new()
            {
                SigningCertificate = ssoCertificate,
                DecryptionCertificate = ssoCertificate,

                //sp certificate issuer
                Issuer = _contextTenant.SsoSpDomain
            };

        saml2Configuration.AllowedAudienceUris.Add(_contextTenant.SsoSpDomain);

        return saml2Configuration;
    }


    [NonAction]
    private Saml2Configuration BuildConfigWithIdpInfo(
        SsoConfigLgc ssoConfig
        )
    {
        Saml2Configuration saml2Configuration = BuildBaseConfiguration();

        EntityDescriptor idpEntityDescriptor = new();

        switch (ssoConfig.IdpMetadataType)
        {
            case SsoIdpMetadataType.Xml:
                idpEntityDescriptor.ReadIdPSsoDescriptor(ssoConfig.IdpMetadataContent);
                break;

            case SsoIdpMetadataType.Url:
                //DA TESTARE
                idpEntityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(ssoConfig.IdpMetadataContent));
                break;

        }

        saml2Configuration.SingleSignOnDestination =
            idpEntityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;

        if (idpEntityDescriptor.IdPSsoDescriptor.SingleLogoutServices.HasValues())
        {
            saml2Configuration.SingleLogoutDestination =
                idpEntityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location;
        }

        saml2Configuration.SignatureValidationCertificates.AddRange(
            idpEntityDescriptor.IdPSsoDescriptor.SigningCertificates
            );

        return saml2Configuration;
    }
    #endregion
}