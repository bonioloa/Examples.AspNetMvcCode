namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// use this filter for actions that require processId in temp data
/// </summary>
//https://code-maze.com/action-filters-aspnetcore/
public class RequiresProcessIdFilter : IActionFilter
{
    private readonly ILogger<RequiresProcessIdFilter> _logger;
    private readonly ContextUser _contextUser;

    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;
    private readonly MainLocalizer _localizer;

    public RequiresProcessIdFilter(
        ILogger<RequiresProcessIdFilter> logger
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

        if (_httpContextAccessorCustomWeb.SessionProcessId.Invalid())
        {
            string controllerName = context.GetController();
            string actionName = context.GetAction();
            _logger.LogAppWarning($"controller: '{controllerName}', action '{actionName}' : missing {nameof(_httpContextAccessorCustomWeb.SessionProcessId)}, redirecting to homepage for selection");

            //we can't always show error because in homepage can be shown other modals
            if (_httpContextAccessorCustomWeb.SessionOperationResult is null
                && !_contextUser.PasswordIsExpired)
            {
                _httpContextAccessorCustomWeb.SessionOperationResult = new OperationResultViewModel()
                {
                    LocalizedMessage =
                        _localizer[nameof(LocalizedStr.ProcessIdRequiredMissingError)]
                };
            }

            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                    {
                            {RouteParams.Controller, MvcComponents.CtrlProcesses},
                            {RouteParams.Action, MvcComponents.ActWelcome}
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
