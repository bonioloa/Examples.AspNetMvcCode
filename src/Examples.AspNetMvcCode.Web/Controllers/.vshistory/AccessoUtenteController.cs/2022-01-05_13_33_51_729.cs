namespace Comunica.ProcessManager.Web.Controllers;

//back in this page after user access must be prevented and forced to token login
[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveTenantProfile)]
[ServiceFilter(typeof(RedirectIfHasCompleteLoginFilter), Order = 1)]
public class AccessoUtenteController : BaseContextController
{
    private readonly ContextTenant _contextTenant;

    public AccessoUtenteController(
        ContextTenant contextTenant
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        ) : base(httpContextAccessorCustomWeb)
    {
        _contextTenant = contextTenant;
    }


    [HttpGet]
    public IActionResult LoginUtente()
    {
        //restore to show the form that was posted
        UserLoginViewModel model =
            _httpContextAccessorCustomWeb.TempDataOnceLoginUser ?? new UserLoginViewModel();

        model.SsoConfigDict = _contextTenant.SsoConfigDict;

        string viewName = string.Empty;
        switch (_contextTenant.Type)
        {
            case ConfigurationType.Anonymous:
                viewName = MvcComponents.ViewUserLoginAnonymous;
                break;
            case ConfigurationType.Registered:
                viewName = MvcComponents.ViewUserLoginRegistered;
                break;
        }//wrong configuration type already handled by logic validation
        return View(viewName, model);
    }
}
