namespace Comunica.ProcessManager.Web.Controllers;

[AllowAnonymous]
public class AccessoPrincipaleController : BaseContextController
{
    public AccessoPrincipaleController(
        IHttpContextAccessorCustom httpContextAccessorCustomWeb
        ) : base(httpContextAccessorCustomWeb)
    {
    }


    //ignore code recommendation to use Url type here, it can't be used as querystring parameter
    [HttpGet]
    public IActionResult IdentificazioneGruppo(
        [ValidateAsStringSimpleFromQuery] string token
        , string returnUrl
        )
    {
        return this.CommonMainPage(token, hideLogo: false, returnUrl);
    }



    //we are keeping returnUrl as string; framework handles this action parameter like string
    //ignore code recommendation to use Url type here, but leave it enabled
    [HttpGet]
    public IActionResult IdentificazioneGruppoRidotto(
        [ValidateAsStringSimpleFromQuery] string token
        , string returnUrl
        )
    {
        return this.CommonMainPage(token, hideLogo: true, returnUrl);
    }



    /// <summary>
    /// Displays warning popup if model is in session.
    /// If warning not found but session/authentication found
    /// cleans everything and reload
    /// Page built with application default supported languages,
    /// After tenant token validation, we will use the 
    /// configured language/s for tenant database
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public IActionResult CommonMainPage(
        string token
        , bool hideLogo
        , string returnUrl
        )
    {
        if (_httpContextAccessorCustomWeb.SessionOperationResult is null //
            && (_httpContextAccessorCustomWeb.IsLoggedOrHasSession()
                //when returnUrl is present we need to reload page for cleanup 
                //(in future maybe we will use it to bring back user to requested page)
                || returnUrl.StringHasValue())
                )
        {
            IDictionary<string, string> routeVal =
                _httpContextAccessorCustomWeb.ContextRequestQuerystringToDictionary(); //returnUrl will be discarded

            _httpContextAccessorCustomWeb.LogoutClearSessionAndPersonalCookies();

            if (hideLogo)
            {
                return RedirectToAction(
                    MvcComponents.ActLoginTenantNoLogo
                    , MvcComponents.CtrlAccessMain
                    , routeVal
                );
            }
            else
            {
                return RedirectToAction(
                   MvcComponents.ActLoginTenant
                   , MvcComponents.CtrlAccessMain
                   , routeVal
               );
            }
        }

        //save is here to allow a first pass where browser
        //is cleaned up for a new access
        _httpContextAccessorCustomWeb.SaveRouteForBackTenantLogin();

        return View(MvcComponents.ViewTenantTokenLogin, token);
    }



    [HttpPost]
    public IActionResult IdentificazioneGruppoPost(
        string token
        , [FromServices] ITenantConfiguratorLogic _logicTenantConfigurator
        , [FromServices] MainLocalizer _localizer
        )
    {
        TenantProfileLgc tenantProfile =
            _logicTenantConfigurator.ValidateAndSetTenantContext(
                token.Clean()
                , _httpContextAccessorCustomWeb.HttpContext.Connection.RemoteIpAddress
                );

        if (!tenantProfile.Success)
        {
            OperationResultViewModel modelMessage = tenantProfile.MapTenantProfileResult();
            modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.SharedErrorAccessDenied)];
            _httpContextAccessorCustomWeb.SessionOperationResult = modelMessage;

            //this method must remove token parameter from querystring to prevent a redirection loop
            return this.BaseRedirectToDefaultLoginPage();
        }

        if (_httpContextAccessorCustomWeb.SignInTenant(tenantProfile.MapFromLogicToWeb()))
        {
            return RedirectToAction(
                MvcComponents.ActLoginUser
                , MvcComponents.CtrlAccessUser
                );
        }

        //if signin fails..
        return this.BaseRedirectToInitialLoginPage();
    }
}