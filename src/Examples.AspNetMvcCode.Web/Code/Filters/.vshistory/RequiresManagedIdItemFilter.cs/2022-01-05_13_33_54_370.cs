namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// use this filter for actions that require processId in temp data
/// </summary>
//https://code-maze.com/action-filters-aspnetcore/
public class RequiresManagedIdItemFilter : IActionFilter
{
    private readonly ILogger<RequiresManagedIdItemFilter> _logger;
    private readonly ContextUser _contextUser;
    private readonly IAuthorizationCustomWeb _webAuthorizationCustom;

    public RequiresManagedIdItemFilter(
        ILogger<RequiresManagedIdItemFilter> logger
        , ContextUser contextUser
        , IAuthorizationCustomWeb webAuthorizationCustom
        )
    {
        _logger = logger;
        _contextUser = contextUser;
        _webAuthorizationCustom = webAuthorizationCustom;
    }



    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogAppDebug("CALL");

        if (_contextUser.IdItemCurrentlyManagedByUser.Invalid())
        {
            string controllerName = context.GetController();
            string actionName = context.GetAction();
            _logger.LogAppWarning($"controller: '{controllerName}', action '{actionName}' : missing {nameof(_contextUser.IdItemCurrentlyManagedByUser)}, redirecting to homepage for selection");


            context.Result =
                new RedirectToRouteResult(_webAuthorizationCustom.GetLandingPageByRole(null, null));

            return;
        }
    }


    public void OnActionExecuted(ActionExecutedContext context)
    {
        //nothing
    }
}
