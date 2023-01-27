namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// prevent access to marked pages for user that has made login with item code (logincode)
/// </summary>
public class RedirectIfAccessSimpleAnonymousFilter : IActionFilter
{
    private readonly ILogger<RedirectIfAccessSimpleAnonymousFilter> _logger;

    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;

    public RedirectIfAccessSimpleAnonymousFilter(
       ILogger<RedirectIfAccessSimpleAnonymousFilter> logger
       , IHttpContextAccessorWeb webHttpContextAccessor
       )
    {
        _logger = logger;
        _webHttpContextAccessor = webHttpContextAccessor;
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



        IEnumerable<string> existsQuery =
            from c in _webHttpContextAccessor?.HttpContext?.User?.Claims
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