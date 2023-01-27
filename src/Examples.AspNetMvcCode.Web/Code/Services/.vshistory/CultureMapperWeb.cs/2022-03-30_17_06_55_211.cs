namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// use this class to execute localization/culture code strictly for application
/// </summary>
public class CultureMapperWeb : ICultureMapperWeb
{
    private static readonly CultureViewModel ItalianCultureModel =
       new()
       {
           DisplayText = "ITALIANO",
           CultureIsoCode = AppConstants.CultureItalianIsoCode,
           IconCode = "it",//fixed by flag-icons.js library 
           Selected = false
       };
    private static readonly CultureViewModel EnglishCultureModel =
        new()
        {
            DisplayText = "ENGLISH",
            CultureIsoCode = AppConstants.CultureEnglishIsoCode,
            IconCode = "gb",//fixed by flag-icons.js library 
            Selected = false
        };
    private static readonly CultureViewModel SpanishCultureModel =
        new()
        {
            DisplayText = "ESPAÑOL",
            CultureIsoCode = AppConstants.CultureSpanishIsoCode,
            IconCode = "es",//fixed by flag-icons.js library 
            Selected = false
        };
    private static readonly IList<CultureViewModel> SupportedCultureViewModels =
        new List<CultureViewModel>
        {
                    ItalianCultureModel,
                    EnglishCultureModel,
                    SpanishCultureModel,
        };



    private readonly ILogger<CultureMapperWeb> _logger;
    private readonly IOptions<RequestLocalizationOptions> _optionRequestLocalization;
    private readonly ContextApp _contextApp;
    private readonly ContextTenant _contextTenant;
    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;

    public CultureMapperWeb(
        ILogger<CultureMapperWeb> logger
        , IOptions<RequestLocalizationOptions> optionRequestLocalization
        , ContextApp contextApp
        , ContextTenant contextTenant
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        )
    {
        _logger = logger;
        _optionRequestLocalization = optionRequestLocalization;
        _contextApp = contextApp;
        _contextTenant = contextTenant;
        _httpContextAccessorCustomWeb = httpContextAccessorCustomWeb;
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
        string requestLanguage = _httpContextAccessorCustomWeb.ContextLanguage;

        //NOTE: we need to retrieve url path part because request culture 
        //just validates if culture has the correct format but can't force
        //redirects. Class is a singleton so contextApp is not available to check
        //if culture in url can be enabled.
        //So we need to make here the validation and redirect if provided culture is not enabled for application
        //when request culture is not in enabled cultures
        string[] parts = _httpContextAccessorCustomWeb.HttpContext.Request.Path.Value.Split('/');
        string pathCulture =
            parts.Length > AppConstants.UrlPathCultureIndex
            ? parts[AppConstants.UrlPathCultureIndex]
            : string.Empty;

        bool doRedirect = false;
        //handle when tenant configured languages are not available (anonymous pages)
        if (_contextTenant.ValidatedDbCulturesIsoCodes.IsNullOrEmpty())
        {
            if (requestLanguage.Empty()
                || !WebAppUtility.TryGetCultureInfo(
                        requestLanguage
                        , out CultureInfo requestCulture
                        )
                || !_optionRequestLocalization.Value.SupportedCultures.Contains(requestCulture)
                || !requestLanguage.EqualsInvariant(pathCulture))
            {
                _contextApp.CurrentCultureIsoCode =
                    _optionRequestLocalization.Value.SupportedCultures[0].TwoLetterISOLanguageName;
                //do redirect ONLY if culture is present in path to prevent infinite redirects when base address is used
                doRedirect = pathCulture.StringHasValue();
                return doRedirect;
            }
            else //language is ok, fill context. 
                 //We don't do any further identity checks because it's 
                 //authorization job to do it before this method
            {
                _contextApp.CurrentCultureIsoCode = requestLanguage;
                doRedirect = false;
                return doRedirect;
            }
        }
        else //validated db languages available (tenant context)
        {
            if (requestLanguage.Empty()
                || !WebAppUtility.TryGetCultureInfo(
                        requestLanguage
                        , out CultureInfo requestCulture
                        )
                || !_contextTenant.ValidatedDbCulturesIsoCodes.Any(vc => vc.Equals(requestCulture.TwoLetterISOLanguageName))
                || !requestLanguage.EqualsInvariant(pathCulture))
            {
                _contextApp.CurrentCultureIsoCode = _contextTenant.ValidatedDbCulturesIsoCodes[0];
                //do redirect ONLY if culture is present in path to prevent infinite redirects when base address is used
                doRedirect = !pathCulture.Empty();
                return doRedirect;
            }
            else
            {
                _contextApp.CurrentCultureIsoCode = requestLanguage;
                doRedirect = false;
                return doRedirect;
            }
        }
    }


    public IList<CultureViewModel> GetEnabledByContextOrAppConfig()
    {
        _logger.LogAppDebug("CALL");

        IEnumerable<string> currentEnabledCulturesIsoCodes =
            _contextTenant.ValidatedDbCulturesIsoCodes.HasValues()
                ? _contextTenant.ValidatedDbCulturesIsoCodes
                : _optionRequestLocalization.Value.SupportedUICultures.Select(c => c.TwoLetterISOLanguageName);

        WebAppUtility.TryGetCultureInfo(
            _httpContextAccessorCustomWeb.ContextLanguage
            , out CultureInfo requestCulture
            );

        IList<CultureViewModel> supportedLanguageModel = new List<CultureViewModel>();

        CultureViewModel tmpCultureModel;
        IEnumerable<CultureViewModel> tmpSearchCulture;
        foreach (string cultureIsoCode in currentEnabledCulturesIsoCodes)
        {
            tmpSearchCulture = SupportedCultureViewModels.Where(msc => msc.CultureIsoCode.EqualsInvariant(cultureIsoCode));
            if (tmpSearchCulture.IsNullOrEmpty())
            {
                _logger.LogAppError($"configured culture '{cultureIsoCode}' not found in enabled culture models of {nameof(CultureMapperWeb)}");
                throw new WebAppException();
            }
            if (tmpSearchCulture.Count() > 1)
            {
                _logger.LogAppError($"configured culture '{cultureIsoCode}' is defined multiple times in culture models of {nameof(CultureMapperWeb)}");
                throw new WebAppException();
            }

            tmpCultureModel = tmpSearchCulture.First();
            tmpCultureModel.Selected = cultureIsoCode.EqualsInvariant(requestCulture.TwoLetterISOLanguageName);

            supportedLanguageModel.Add(tmpCultureModel);
        }

        return supportedLanguageModel;
    }



    public IList<string> GetAppSupportedCulturesList()
    {
        return
            _optionRequestLocalization.Value.SupportedUICultures
                .Select(c => c.TwoLetterISOLanguageName)
                .ToList();
    }
}