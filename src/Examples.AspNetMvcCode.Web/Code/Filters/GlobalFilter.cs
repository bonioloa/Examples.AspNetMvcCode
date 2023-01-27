namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// generally this filter should be used only when code 
/// must be executed against almost all app actions
/// Use simple filters for execution on a subset of actions
/// </summary>
public class GlobalFilter : IActionFilter
{
    private readonly ILogger<GlobalFilter> _logger;
    private readonly IOptionsSnapshot<WebsiteSettings> _optWebsite;
    private readonly ContextApp _contextApp;
    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;
    private readonly ICultureMapperWeb _webCultureMapper;

    public GlobalFilter(
        ILogger<GlobalFilter> logger
        , IOptionsSnapshot<WebsiteSettings> optWebsite
        , ContextApp contextApp
        , IHttpContextAccessorWeb webHttpContextAccessor
        , ICultureMapperWeb webCultureMapper
        )
    {
        _logger = logger;
        _optWebsite = optWebsite;
        _contextApp = contextApp;
        _webHttpContextAccessor = webHttpContextAccessor;
        _webCultureMapper = webCultureMapper;
    }



    public void OnActionExecuting(ActionExecutingContext context)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(OnActionExecuting) }
                });

        _logger.LogDebug("CALL");



        string controllerName = context.GetController();
        string actionName = context.GetAction();


        //set supported cultures in context
        //here because validation cookie does not get triggered on anonymous pages, 
        //so both culture and app cultures risk to not be initialized for anonymous pages
        _contextApp.AppSupportedCulturesIsoCodes = _webCultureMapper.GetAppSupportedCulturesList();



        #region language request checks 

        bool redirectNecessary = _webCultureMapper.SetCultureAndDetectIfRedirectNeeded();

        //use profile context language for redirect. 
        //Above method have already set a valid language
        if (redirectNecessary)
        {
            IDictionary<string, string> tmpRoute =
                _webHttpContextAccessor.GetContextRequestQuerystringWithLanguage(
                    _contextApp.CurrentCultureIsoCode
                    );

            tmpRoute.Add(RouteParams.Controller, controllerName);
            tmpRoute.Add(RouteParams.Action, actionName);

            context.Result =
                new RedirectToRouteResult(
                    new RouteValueDictionary(tmpRoute)
                );
            return;
        }
        #endregion



        if (_optWebsite.Value.ForceMaintenancePage
            && actionName != MvcComponents.ActMaintenance)//prevent infinite redirects cycle
        {

            IDictionary<string, string> tmpRoute =
                _webHttpContextAccessor.GetContextRequestQuerystringWithLanguage(
                    _contextApp.CurrentCultureIsoCode
                    );

            string currentLanguage = tmpRoute[RouteParams.Language];

            tmpRoute =
                new Dictionary<string, string>
                {
                    { RouteParams.Language, currentLanguage },
                    { RouteParams.Controller, MvcComponents.CtrlErrors },
                    { RouteParams.Action, MvcComponents.ActMaintenance }
                };

            context.Result =
                new RedirectToRouteResult(
                    new RouteValueDictionary(tmpRoute)
                );
        }
    }


    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}