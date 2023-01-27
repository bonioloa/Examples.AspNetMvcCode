namespace Examples.AspNetMvcCode.Logic;

public class TenantConfiguratorLogic : ITenantConfiguratorLogic
{
    private readonly ILogger<TenantConfiguratorLogic> _logger;
    private readonly ContextApp _contextApp;
    private readonly ContextTenant _contextTenant;

    private readonly IDataCommandManagerTenant _dataCommandManagerTenant;
    private readonly ITenantCryptManager _cryptManagerTenant;

    private readonly IRootQueries _queryRoot;
    private readonly ICompanyGroupQueries _queryGroupCompany;
    private readonly IParametersQueries _queryParameters;
    private readonly ISsoQueries _querySso;


    public TenantConfiguratorLogic(
        ILogger<TenantConfiguratorLogic> logger
        , ContextApp contextApp
        , ContextTenant contextTenant
        , IDataCommandManagerTenant dataCommandManagerTenant
        , ITenantCryptManager cryptManagerTenant
        , IRootQueries queryRoot
        , ICompanyGroupQueries queryGroupCompany
        , IParametersQueries queryParameters
        , ISsoQueries querySso
        )
    {
        _logger = logger;
        _contextApp = contextApp;
        _contextTenant = contextTenant;
        _dataCommandManagerTenant = dataCommandManagerTenant;
        _cryptManagerTenant = cryptManagerTenant;
        _queryRoot = queryRoot;
        _queryGroupCompany = queryGroupCompany;
        _queryParameters = queryParameters;
        _querySso = querySso;
    }




    /// <summary>
    /// search token in root db, if found get company db connection string 
    /// and save it to company db manager service
    /// </summary>
    /// <param name="tenantToken"></param>
    /// <returns>ErrorMessage not empty if there is some kind of error or validation fail</returns>
    public TenantProfileLgc ValidateAndSetTenantContext(
        string tenantToken
        , IPAddress remoteIpAddress
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ValidateAndSetTenantContext) }
                });

        _logger.LogDebug("CALL");



        if (tenantToken.Empty())
        {
            return
                new TenantProfileLgc
                {
                    WarningType = WarningType.NotFound,
                    FieldToWarnList = new List<MessageField> { MessageField.TenantToken }
                };
        }


        tenantToken = tenantToken.Clean();

        TenantConfigQr tenantConfig = _queryRoot.GetTenantConfigFromToken(tenantToken);

        if (tenantConfig is null)
        {
            return
               new TenantProfileLgc
               {
                   WarningType = WarningType.NotFound,
                   FieldToWarnList = new List<MessageField> { MessageField.TenantToken }
               };
        }

        _dataCommandManagerTenant.ValidateAndInitialize(tenantConfig);


        //token validation passed
        //VERY IMPORTANT, save to context to allow data service to automatically
        //load and build connection string when needed for queries
        _contextTenant.Token = tenantToken;


        CompanyGroupQr companyGroup = _queryGroupCompany.CheckDataForSingleCompanyConfiguration();

        TenantOwnConfigurationQr tenantOwnConfig = _queryParameters.GetTenantOwnConfiguration();

        ConfigureLanguageForContextTenant(tenantOwnConfig);

        _cryptManagerTenant.InitializeWithStandardMethod(
            tenantOwnConfig.CryptKeyPart
            );

        //placed here to make language configuration available for next queries
        ConfigureLanguageForContextTenant(tenantOwnConfig);


        SsoConfig ssoConfig = GetSsoConfiguration();


        TenantProfileLgc tenantProfile =
            MapToTenantProfile(
                tenantConfig
                , companyGroup
                , tenantOwnConfig
                , ssoConfig
                );


        _contextTenant.Token = tenantProfile.Token;
        _contextTenant.Type = tenantProfile.Type;
        _contextTenant.LogoFileName = tenantProfile.LogoFileName;
        _contextTenant.TwoFactorAuthenticationEnabled = tenantProfile.TwoFactorAuthenticationEnabled;
        _contextTenant.CompanyGroupId = tenantProfile.CompanyGroupId;
        _contextTenant.DisableRegistrationForUsers = tenantProfile.DisableRegistrationForUsers;

        //note: we save this data only in context, in profile we just keep the id
        _contextTenant.SsoSpCertificatePath = ssoConfig.SsoSpCertificatePath;
        _contextTenant.SsoSpCertificatePassword = ssoConfig.SsoSpCertificatePassword;
        _contextTenant.SsoSpDomain = ssoConfig.SsoSpDomain;

        _contextTenant.SsoLoginMode = tenantProfile.SsoLoginMode;
        _contextTenant.SsoIdpConfigDict = tenantProfile.SsoIdpConfigDict;

        tenantProfile.Success = true;

        return tenantProfile;
    }



    private record SsoConfig(
        SsoLoginMode SsoLoginMode
        , long SsoSpConfigId
        , string SsoSpCertificatePath
        , string SsoSpCertificatePassword
        , string SsoSpDomain
        , Dictionary<long, string> SsoConfigDict
        );

    private SsoConfig GetSsoConfiguration()
    {
        SsoLoginMode ssoLoginMode = _queryParameters.GetSsoLoginMode();

        if (ssoLoginMode == SsoLoginMode.Local)
        {
            return
                new SsoConfig(
                    SsoLoginMode: SsoLoginMode.Local
                    , SsoSpConfigId: 0
                    , SsoSpCertificatePath: string.Empty
                    , SsoSpCertificatePassword: string.Empty
                    , SsoSpDomain: string.Empty
                    , SsoConfigDict: new Dictionary<long, string>()
                    );
        }

        SsoSpConfigQr ssoSpConfig = GetSpConfig();

        //we only need id and button text,
        //the rest of data is too big to save on cookie and completely unnecessary a this point for presentation
        IEnumerable<SsoConfigQr> ssoConfigsFound = _querySso.GetAllSsoIdpConfig();

        if (ssoConfigsFound.IsNullOrEmpty())
        {
            throw new PmLogicException($"no sso configurations found, but sso access is enabled for tenant");
        }


        Dictionary<long, string> ssoConfigDict = new();

        foreach (SsoConfigQr ssoConfig in ssoConfigsFound.OrderBy(sp => sp.Order))
        {
            if (ssoConfig.Id.Invalid())
            {
                throw new PmLogicException($"sso config id value '{ssoConfig.Id}' is invalid. Provide only positive values ");
            }


            if (ssoConfig.SsoLoginButtonDescription.Empty())
            {
                throw new PmLogicException($"sso button access description is empty for config at id '{ssoConfig.Id}' ");
            }

            ssoConfigDict.Add(ssoConfig.Id, ssoConfig.SsoLoginButtonDescription);
        }

        return
            new SsoConfig(
                SsoLoginMode: ssoLoginMode
                , SsoSpConfigId: ssoSpConfig.Id
                , SsoSpCertificatePath: ssoSpConfig.CertificatePath
                , SsoSpCertificatePassword: _cryptManagerTenant.DecryptAndCleanString(ssoSpConfig.CertificatePassword)
                , SsoSpDomain: ssoSpConfig.Domain
                , SsoConfigDict: ssoConfigDict
                );
    }

    private SsoSpConfigQr GetSpConfig()
    {
        IEnumerable<SsoSpConfigQr> ssoSpConfigsFound = _querySso.GetAllSpSsoConfig();
        if (ssoSpConfigsFound.IsNullOrEmpty())
        {
            throw new PmLogicException("sso enabled but SP was not found");
        }
        if (ssoSpConfigsFound.Count() > 1)
        {
            throw new PmLogicException("multiple SP configuration are defined, system does not support multiple SP configuration");
        }

        return ssoSpConfigsFound.First();
    }



    private void ConfigureLanguageForContextTenant(TenantOwnConfigurationQr tenantOwnConfig)
    {
        IEnumerable<string> allowedCultures =
                    _contextApp.AppSupportedCulturesIsoCodes.Intersect(tenantOwnConfig.DbCulturesIsoCodes);

        if (allowedCultures.IsNullOrEmpty())
        {
            //if this happens database has only unsupported languages configured, so this must be fixed
            //this should never happen because db languages are checked and converted to ISO 
            //when tenant information are retrieved
            throw new PmLogicException(
                @$"unsupported languages in database configuration: 
                    db values '{string.Join(',', tenantOwnConfig.DbCulturesIsoCodes)}'
                    application values '{string.Join(',', _contextApp.AppSupportedCulturesIsoCodes)}'"
                    );
        }

        //enforce language if current is not allowed by token configuration
        if (!allowedCultures.Contains(_contextApp.CurrentCultureIsoCode))
        {
            _contextApp.CurrentCultureIsoCode = allowedCultures.First();
        }
        _contextTenant.ValidatedDbCulturesIsoCodes = allowedCultures.ToList();
    }


    private static TenantProfileLgc MapToTenantProfile(
        TenantConfigQr tenantConfig
        , CompanyGroupQr companyGroup
        , TenantOwnConfigurationQr tenantOwnConfig
        , SsoConfig ssoConfig
        )
    {
        return
            new TenantProfileLgc()
            {
                Token = tenantConfig.Token,
                Type = tenantConfig.ConfigType,
                LogoFileName = companyGroup.LogoPath,
                TwoFactorAuthenticationEnabled = tenantConfig.EnableTwoFactorAuthentication,
                CompanyGroupId = companyGroup.Id,
                DisableRegistrationForUsers = tenantOwnConfig.DisableRegistrationForUsers,

                SsoLoginMode = ssoConfig.SsoLoginMode,
                SsoIdpConfigDict = ssoConfig.SsoConfigDict,
                SsoSpConfigId = ssoConfig.SsoSpConfigId
            };
    }






    public OperationResultLgc ValidateProfileAndSetTenantContext(
        TenantProfileLgc claimTenantProfileToValidate
        , IPAddress remoteIpAddress
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ValidateProfileAndSetTenantContext) }
                });

        _logger.LogDebug("CALL");



        #region validate tenantProfile data sanity

        Guard.Against.Null(claimTenantProfileToValidate, nameof(claimTenantProfileToValidate));

        Guard.Against.NullOrWhiteSpace(claimTenantProfileToValidate.Token, nameof(claimTenantProfileToValidate.Token));

        Guard.Against.InvalidInput(
            claimTenantProfileToValidate.Type
            , nameof(claimTenantProfileToValidate.Type)
            , (input) => input != ConfigurationType.Missing
            , $"value '{claimTenantProfileToValidate.Type}' is not allowed"
            );

        Guard.Against.NullOrWhiteSpace(claimTenantProfileToValidate.LogoFileName, nameof(claimTenantProfileToValidate.LogoFileName));
        #endregion

        TenantProfileLgc tenantProfile =
            ValidateAndSetTenantContext(
                claimTenantProfileToValidate.Token
                , remoteIpAddress
                );

        if (!tenantProfile.Success)
        {
            if (tenantProfile.WarningType != WarningType.None)
            {
                //in this case let error bubble up
                return
                    new OperationResultLgc()
                    {
                        WarningType = tenantProfile.WarningType,
                        FieldToWarnList = tenantProfile.FieldToWarnList,
                        ValuesAllowed = tenantProfile.ValuesAllowed,
                    };
            }
            //else throw exception
            throw new PmLogicException($"{nameof(ValidateAndSetTenantContext)} {nameof(tenantProfile.Token)} invalid");
        }

        if (claimTenantProfileToValidate.Type != tenantProfile.Type)
        {
            throw new PmLogicException($"{nameof(ValidateAndSetTenantContext)} {nameof(tenantProfile.Type)} mismatch");
        }


        return
            new OperationResultLgc() { Success = true };
    }



    public SsoConfigLgc GetSsoConfigByIdOrDefault(long ssoConfigId)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetSsoConfigByIdOrDefault) }
                });

        _logger.LogDebug("CALL");



        SsoConfigQr ssoConfig = _querySso.GetIdpBySsoConfigIdOrDefault(ssoConfigId);

        if (ssoConfig is null
            || ssoConfig.Id.Invalid()
            || ssoConfig.IdpMetadataContent.Empty())
        {
            throw new PmLogicException($"no sso config found for id '{ssoConfigId}' ");
        }

        return
            ssoConfig.MapFromDataToLogicWithNullCheck();
    }
}