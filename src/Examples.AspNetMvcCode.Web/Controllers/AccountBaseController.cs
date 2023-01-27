namespace Examples.AspNetMvcCode.Web.Controllers;

//there is no explicit logout method for anonymous sessions
[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.UserAccessedWithLoginAndPassword)]
public class AccountBaseController : BaseContextController
{
    private readonly ILogger<AccountBaseController> _logger;
    private readonly ContextUser _contextUser;
    private readonly IAuditLogic _logicAudit;

    public AccountBaseController(
        ILogger<AccountBaseController> logger
        , ContextUser contextUser
        , IAuditLogic logicAudit
        , IHttpContextAccessorWeb webHttpContextAccessor
        ) : base(webHttpContextAccessor)
    {
        _logger = logger;
        _contextUser = contextUser;
        _logicAudit = logicAudit;
    }



    //not implemented for now
    //[HttpGet]
    //public IActionResult AccessDenied()
    //{
    //    base.DoSignOut();
    //    return View();
    //}

    [HttpGet]
    public IActionResult Logout()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(Logout) }
                });
        long contextUserId = _contextUser.UserIdLoggedIn;

        _logicAudit.LogLogout();



        _logger.LogInformation(
            "userId '{UserId}' logged out"
            , contextUserId
            );


        return BaseRedirectToInitialLoginPage();
    }
}