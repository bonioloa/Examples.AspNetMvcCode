namespace Comunica.ProcessManager.Web.Controllers;

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
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        ) : base(httpContextAccessorCustomWeb)
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
        _logicAudit.LogLogout();

        _logger.LogAppInformation($"userId '{_contextUser.UserIdLoggedIn}' logged out");

        return this.BaseRedirectToInitialLoginPage();
    }
}
