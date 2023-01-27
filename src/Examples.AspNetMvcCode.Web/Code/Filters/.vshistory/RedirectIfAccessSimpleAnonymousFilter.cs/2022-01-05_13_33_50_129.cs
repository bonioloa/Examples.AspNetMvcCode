namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// prevent access to marked pages for user that has made login with item code (logincode)
/// </summary>
public class RedirectIfAccessSimpleAnonymousFilter : IActionFilter
{
    private readonly ILogger<RedirectIfAccessSimpleAnonymousFilter> _logger;

    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;

    public RedirectIfAccessSimpleAnonymousFilter(
       ILogger<RedirectIfAccessSimpleAnonymousFilter> logger
       , IHttpContextAccessorCustom httpContextAccessorCustomWeb
       )
    {
        _logger = logger;
        _httpContextAccessorCustomWeb = httpContextAccessorCustomWeb;
    }



    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogAppDebug("CALL");

        IEnumerable<string> existsQuery =
            from c in _httpContextAccessorCustomWeb?.HttpContext?.User?.Claims
            where c.Type == ClaimsKeys.UserProfile
            select c.Value;

        if (existsQuery.HasValues()
            && existsQuery.First().StringHasValue())
        {
            UserProfileLgc userProfile = JsonSerializer.Deserialize<UserProfileLgc>(existsQuery.First());

            if (userProfile.AccessType == AccessType.BasicRoleUserAnonymousForInsert)
            {
                context.Result =
                 new RedirectToRouteResult(
                     new RouteValueDictionary
                     {
                            {RouteParams.Controller, MvcComponents.CtrlProcesses},
                            {RouteParams.Action, MvcComponents.ActWelcome},
                     }
                 );
            }
        }
    }


    public void OnActionExecuted(ActionExecutedContext context)
    {
        //nothing
    }
}
