namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// use this class to execute localization/culture code strictly for application
/// </summary>
public class CultureMapperWeb : ICultureMapperWeb
{
    private readonly ILogger<CultureMapperWeb> _logger;
    private readonly IOptions<RequestLocalizationOptions> _optionRequestLocalization;
    private readonly ContextApp _contextApp;
    private readonly ContextTenant _contextTenant;
    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;

    public CultureMapperWeb(
        ILogger<CultureMapperWeb> logger
        , IOptions<RequestLocalizationOptions> optionRequestLocalization
        , ContextApp contextApp
        , ContextTenant contextTenant
        , IHttpContextAccessorWeb webHttpContextAccessor
        )
    {
        _logger = logger;
        _optionRequestLocalization = optionRequestLocalization;
        _contextApp = contextApp;
        _contextTenant = contextTenant;
        _webHttpContextAccessor = webHttpContextAccessor;
    }



    /// <summary>
    /// validate request language against 
    /// database languages or, if user has not made tenant access, app languages.
    /// If language is not valid set the first language and return true to redirect
    /// </summary>
    /// <returns>
    /// flag redirect
    /// </returns>
    public bool SetCultureAndDetectIfRedirectNeeded()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(SetCultureAndDetectIfRedirectNeeded) }
                });

        _logger.LogDebug("CALL");



        string requestLanguage = _webHttpContextAccessor.ContextLanguage;

        //NOTE: we need to retrieve url path part because request culture 
        //just validates if culture has the correct format but can't force
        //redirects. Class is a singleton so contextApp is not available to check
        //if culture in url can be enabled.
        //So we need to make here the validation and redirect if provided culture is not enabled for application
        //when request culture is not in enabled cultures
        string[] parts = _webHttpContextAccessor.HttpContext.Request.Path.Value.Split('/');

        //EXCEPTION for captcha image url.
        //If not present it will cause a infinite redirect
        if (parts.Contains("dntcaptchaimage") && parts.Contains("show"))
        {
            return false;
        }

        string pathCulture =
            parts.Length > LocalizationConstants.UrlPathCultureIndex
            ? parts[LocalizationConstants.UrlPathCultureIndex]
            : string.Empty;


        bool doRedirect = false;
        CultureInfo requestCulture;

        //handle when tenant configured languages are not available (anonymous pages)
        if (_contextTenant.ValidatedDbCulturesIsoCodes.IsNullOrEmpty())
        {
            if (requestLanguage.Empty()
                || !TryGetCultureInfo(
                        requestLanguage
                        , out requestCulture
                        )
                || !_optionRequestLocalization.Value.SupportedCultures.Contains(requestCulture)
                || !requestLanguage.EqualsInvariant(pathCulture))
            {
                _contextApp.CurrentCultureIsoCode =
                    _optionRequestLocalization.Value.SupportedCultures[0].TwoLetterISOLanguageName;

                //do redirect ONLY if culture is present in path to prevent infinite redirects when base address is used
                return
                    pathCulture.StringHasValue();
            }

            //language is ok, fill context. 
            //We don't do any further identity checks because it's 
            //authorization job to do it before this method
            _contextApp.CurrentCultureIsoCode = requestLanguage;
            doRedirect = false;

            return doRedirect;
        }

        //validated db languages available (tenant context)
        if (requestLanguage.Empty()
            || !TryGetCultureInfo(
                    requestLanguage
                    , out requestCulture
                    )
            || !_contextTenant.ValidatedDbCulturesIsoCodes.Any(vc => vc.EqualsInvariant(requestCulture.TwoLetterISOLanguageName))
            || !requestLanguage.EqualsInvariant(pathCulture))
        {
            _contextApp.CurrentCultureIsoCode = _contextTenant.ValidatedDbCulturesIsoCodes[0];

            //do redirect ONLY if culture is present in path to prevent infinite redirects when base address is used
            return
                pathCulture.StringHasValue();
        }

        _contextApp.CurrentCultureIsoCode = requestLanguage;
        doRedirect = false;

        return doRedirect;
    }


    public IList<CultureViewModel> GetEnabledByContextOrAppConfig()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetEnabledByContextOrAppConfig) }
                });

        _logger.LogDebug("CALL");



        IEnumerable<string> currentEnabledCulturesIsoCodes =
            _contextTenant.ValidatedDbCulturesIsoCodes.HasValues()
                ? _contextTenant.ValidatedDbCulturesIsoCodes
                : _optionRequestLocalization.Value.SupportedUICultures.Select(c => c.TwoLetterISOLanguageName);

        TryGetCultureInfo(
            _webHttpContextAccessor.ContextLanguage
            , out CultureInfo requestCulture
            );


        CultureViewModel[] cultureViewModels = BuildDefaultSupportedCultures();

        IList<CultureViewModel> supportedLanguageModel = new List<CultureViewModel>();

        foreach (string cultureIsoCode in currentEnabledCulturesIsoCodes)
        {
            IEnumerable<CultureViewModel> cultureFound =
                cultureViewModels.Where(msc => msc.CultureIsoCode.EqualsInvariant(cultureIsoCode));

            if (cultureFound.IsNullOrEmpty())
            {
                throw new PmWebException(
                    $"configured culture '{cultureIsoCode}' not found in enabled culture models of {nameof(CultureMapperWeb)}"
                    );
            }

            if (cultureFound.Count() > 1)
            {
                throw new PmWebException(
                    $"configured culture '{cultureIsoCode}' is defined multiple times in culture models of {nameof(CultureMapperWeb)}"
                    );
            }


            CultureViewModel tmpCultureModel = cultureFound.Single();

            tmpCultureModel.Selected = cultureIsoCode.EqualsInvariant(requestCulture.TwoLetterISOLanguageName);


            supportedLanguageModel.Add(tmpCultureModel);
        }

        return supportedLanguageModel;
    }


    private static CultureViewModel[] BuildDefaultSupportedCultures()
    {
        CultureViewModel italianCultureModel =
            new()
            {
                DisplayText = "ITALIANO",
                CultureIsoCode = SupportedCulturesConstants.IsoCodeItalian,
                IconCode = "it",//fixed by flag-icons.js library 
                Selected = false
            };

        CultureViewModel englishCultureModel =
           new()
           {
               DisplayText = "ENGLISH",
               CultureIsoCode = SupportedCulturesConstants.IsoCodeEnglish,
               IconCode = "gb",//fixed by flag-icons.js library 
               Selected = false
           };

        CultureViewModel spanishCultureModel =
            new()
            {
                DisplayText = "ESPAÑOL",
                CultureIsoCode = SupportedCulturesConstants.IsoCodeSpanish,
                IconCode = "es",//fixed by flag-icons.js library 
                Selected = false
            };

        return
            new CultureViewModel[]
            {
                italianCultureModel,
                englishCultureModel,
                spanishCultureModel,
            };
    }



    public IList<string> GetAppSupportedCulturesList()
    {
        return
            _optionRequestLocalization.Value.SupportedUICultures
                .Select(c => c.TwoLetterISOLanguageName)
                .ToList();
    }


    /// <summary>
    /// validate culture code and fallback to default if not valid
    /// </summary>
    /// <param name="cultureIsoCode">code to validate</param>
    /// <param name="culture"></param>
    /// <returns></returns>
    private static bool TryGetCultureInfo(string cultureIsoCode, out CultureInfo culture)
    {
        try
        {
            culture = CultureInfo.GetCultureInfo(cultureIsoCode);
            return true;
        }
        catch (CultureNotFoundException)
        {
            culture = SupportedCulturesConstants.CultureDefault;
        }

        return false;
    }
}