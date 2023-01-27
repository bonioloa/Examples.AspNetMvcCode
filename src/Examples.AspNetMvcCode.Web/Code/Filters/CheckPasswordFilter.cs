namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// this filter has always maximum priority
/// </summary>
public class CheckPasswordFilter : IActionFilter
{
    private readonly ILogger<CheckPasswordFilter> _logger;
    private readonly ContextUser _contextUser;
    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;
    private readonly IMainLocalizer _localizer;

    public CheckPasswordFilter(
       ILogger<CheckPasswordFilter> logger
        , ContextUser contextUser
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IMainLocalizer localizer
       )
    {
        _logger = logger;
        _contextUser = contextUser;
        _webHttpContextAccessor = webHttpContextAccessor;
        _localizer = localizer;
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



        if (_contextUser.PasswordIsExpired)
        {
            string controllerName = context.GetController();
            string actionName = context.GetAction();

            _logger.LogWarning(
                "controller: '{ControllerName}', action '{ActionName}' : password expired, redirecting to password change page"
                , controllerName
                , actionName
                );

            //don't show any error for now
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel()
                {
                    LocalizedMessage =_localizer[nameof(LocalizedStr.FilterWarningPasswordExpired)]
                };

            context.Result =
                new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                            {RouteParams.Controller, MvcComponents.CtrlAccountCredentials},
                            {RouteParams.Action, MvcComponents.ActChangePassword}
                    }
                );

            return;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        //nothing
    }
}