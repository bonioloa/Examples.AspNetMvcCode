namespace Comunica.ProcessManager.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveTenantProfile)]
[ServiceFilter(typeof(RedirectIfHasCompleteLoginFilter), Order = 1)]
public class AccessoRecuperoController : BaseContextController
{
    public AccessoRecuperoController(
        IHttpContextAccessorCustom httpContextAccessorCustomWeb
        ) : base(httpContextAccessorCustomWeb)
    {
    }


    [HttpGet]
    public IActionResult RecuperoDati()
    {
        return View(MvcComponents.ViewUserRecoverCredentials);
    }

    /// <summary>
    /// this page must be shown only on success. 
    /// var in temp data prevents user to access this page without the proper flow
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult EsitoRecuperoDati()
    {
        return
            _httpContextAccessorCustomWeb.TempDataOnceRecoverSuccess
                ? View(MvcComponents.ViewUserRecoverCredentialsResult)
                : RedirectToAction(
                    MvcComponents.ActRecoverUserData
                    , MvcComponents.CtrlAccessRecover
                    );
    }
}
