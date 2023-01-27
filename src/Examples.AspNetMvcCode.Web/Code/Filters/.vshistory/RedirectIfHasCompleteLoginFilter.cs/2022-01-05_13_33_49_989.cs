namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// we are using a filter instead of a policy because we
/// need a redirect, not an authorization fail, because that will generate an error message
/// we just need to silently prevent access to the page and redirect to login
/// </summary>
public class RedirectIfHasCompleteLoginFilter : IActionFilter
{
    private readonly ILogger<RedirectIfHasCompleteLoginFilter> _logger;

    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;

    public RedirectIfHasCompleteLoginFilter(
       ILogger<RedirectIfHasCompleteLoginFilter> logger
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

        if (existsQuery.HasValues() && existsQuery.First().StringHasValue())
        {
            _logger.LogAppWarning($"prevented access to controller: '{context.GetController()}', action '{context.GetAction()}'.  Claim {ClaimsKeys.UserProfile} present, redirecting to tenant login");

            context.Result = _httpContextAccessorCustomWeb.ClearSessionAndCookieAndGetRedirectForLogin();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        //nothing
    }
}
