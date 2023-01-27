namespace Comunica.ProcessManager.Web.Controllers;

/// <summary>
/// add here fallback redirects.
/// 
/// Content:
/// -Invalid language
/// -Old url redirects
/// </summary>
[AllowAnonymous]
public class FallbackController : BaseContextController
{
    public FallbackController(
        IHttpContextAccessorCustom httpContextAccessorCustomWeb
        ) : base(httpContextAccessorCustomWeb)
    {

    }

    /// <summary>
    /// Fallback when route is not valid.
    /// This is needed for default route
    /// </summary>
    /// <returns></returns>
    public IActionResult ToDefaultLanguage()
    {
        return base.BaseRedirectToDefault();
    }

    public IActionResult ToTenantLogin(string lang, string token)
    {
        return base.BaseRedirectToTenantLogin(lang.Clean(), token.Clean());
    }

    public IActionResult ToTenantLoginNoLeftPanel(string lang, string token)
    {
        return base.BaseRedirectToLoginNoLeftPanel(lang.Clean(), token.Clean());
    }

    public IActionResult ToValidateRegistration(string lang, string token, string idConfirm)
    {
        return base.RedirectToValidateRegistration(lang.Clean(), token.Clean(), idConfirm.Clean());
    }

    public IActionResult ToItem(string lang, long? idAdemp)
    {
        return base.RedirectToItem(lang.Clean(), idAdemp);
    }

    public IActionResult ToSendErrorReport(string lang)
    {
        return base.RedirectToSendErrorReport(lang.Clean());
    }
}
