namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// this filter has always maximum priority
/// </summary>
public class CheckPasswordFilter : IActionFilter
{
    private readonly ILogger<CheckPasswordFilter> _logger;
    private readonly ContextUser _contextUser;
    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;
    private readonly MainLocalizer _localizer;

    public CheckPasswordFilter(
       ILogger<CheckPasswordFilter> logger
        , ContextUser contextUser
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , MainLocalizer localizer
       )
    {
        _logger = logger;
        _contextUser = contextUser;
        _httpContextAccessorCustomWeb = httpContextAccessorCustomWeb;
        _localizer = localizer;
    }



    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogAppDebug("CALL");

        if (_contextUser.PasswordIsExpired)
        {
            string controllerName = context.GetController();
            string actionName = context.GetAction();
            _logger.LogAppWarning($"controller: '{controllerName}', action '{actionName}' : password expired, redirecting to password change page");

            //don't show any error for now
            _httpContextAccessorCustomWeb.SessionOperationResult = new OperationResultViewModel()
            {
                LocalizedMessage =
                    _localizer[nameof(LocalizedStr.FilterWarningPasswordExpired)]
            };
            context.Result = new RedirectToRouteResult(
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
