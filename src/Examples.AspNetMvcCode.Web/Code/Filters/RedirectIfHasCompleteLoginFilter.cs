namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// we are using a filter instead of a policy because we
/// need a redirect, not an authorization fail, because that will generate an error message
/// we just need to silently prevent access to the page and redirect to login
/// </summary>
public class RedirectIfHasCompleteLoginFilter : IActionFilter
{
    private readonly ILogger<RedirectIfHasCompleteLoginFilter> _logger;

    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;

    public RedirectIfHasCompleteLoginFilter(
       ILogger<RedirectIfHasCompleteLoginFilter> logger
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


        if (existsQuery.HasValues() && existsQuery.First().StringHasValue())
        {
            string controller = context.GetController();
            string action = context.GetAction();
            string userProfileClaimName = ClaimsKeys.UserProfile;
            _logger.LogWarning(
                "prevented access to controller: '{Controller}', action '{Action}'.  Claim {UserProfileClaimName} present, redirecting to tenant login"
                , controller
                , action
                , userProfileClaimName
                );

            context.Result = _webHttpContextAccessor.ClearSessionAndCookieAndGetRedirectForLogin();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        //nothing
    }
}