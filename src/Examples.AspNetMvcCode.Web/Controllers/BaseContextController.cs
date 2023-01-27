namespace Examples.AspNetMvcCode.Web.Controllers;

/// <summary>
/// encapsulates common contextaccessor redirection methods with necessary code to be always executed.
/// NOTE, they are methods, NOT callable actions
/// Inherit this class for controllers that require contextaccessor
/// </summary>
public abstract class BaseContextController : Controller
{
    internal readonly IHttpContextAccessorWeb _webHttpContextAccessor;

    protected BaseContextController(
        IHttpContextAccessorWeb webHttpContextAccessor
        )
    {
        _webHttpContextAccessor = webHttpContextAccessor;
    }



    [NonAction]
    internal IActionResult BaseRedirectToDefault()
    {
        return
            RedirectToAction(
                 MvcComponents.ActLoginTenant
                , MvcComponents.CtrlAccessMain
                , new Dictionary<string, string>()
                    {
                        { RouteParams.Language, SupportedCulturesConstants.IsoCodeDefault }
                    }
                );
    }



    [NonAction]
    internal IActionResult BaseRedirectToTenantLogin()
    {
        return BaseRedirectToTenantLoginWithClear(null, null);
    }


    [NonAction]
    internal IActionResult BaseRedirectToTenantLoginSimple(string language, string token)
    {
        return
            RedirectToAction(
                 MvcComponents.ActLoginTenant
                , MvcComponents.CtrlAccessMain
                , GetQuerystringRoute(
                    language.Clean()
                    , token.Clean()
                    , idConfirm: null
                    , itemId: null
                    )
                );
    }


    [NonAction]
    internal IActionResult BaseRedirectToTenantLoginWithClear(string language, string token)
    {
        _webHttpContextAccessor.LogoutClearSessionAndPersonalCookies();

        return
            BaseRedirectToTenantLoginSimple(language, token);
    }


    [NonAction]
    internal IActionResult BaseRedirectToLoginNoLogo()
    {
        return BaseRedirectToLoginNoLeftPanel(null, null);
    }


    [NonAction]
    internal IActionResult BaseRedirectToLoginNoLeftPanel(string language, string token)
    {
        _webHttpContextAccessor.LogoutClearSessionAndPersonalCookies();

        return
            RedirectToAction(
                 MvcComponents.ActLoginTenantNoLogo
                , MvcComponents.CtrlAccessMain
                , GetQuerystringRoute(
                    language.Clean()
                    , token.Clean()
                    , idConfirm: null
                    , itemId: null
                    )
                );
    }


    [NonAction]
    internal IActionResult BaseRedirectToDefaultLoginPage()
    {
        RouteViewModel routeModel =
            _webHttpContextAccessor.GetRouteForBackTenantLogin(
                 removeTenantTokenFromQuerystring: true
                 );

        _webHttpContextAccessor.LogoutClearSessionAndPersonalCookies();

        return
            RedirectToAction(
                routeModel.Action
                , routeModel.Controller
                , routeModel.QueryStringValues
                );
    }


    /// <summary>
    /// This action returns to initial login page, preserving token in provided in initial call
    /// It also logs out eventual logged in identities and drops session
    /// </summary>
    /// <returns></returns>
    [NonAction]
    internal IActionResult BaseRedirectToInitialLoginPage()
    {
        //retrieve from session what page has been used for access
        RouteViewModel routeModel =
            _webHttpContextAccessor.GetRouteForBackTenantLogin(
                removeTenantTokenFromQuerystring: false
                );

        _webHttpContextAccessor.LogoutClearSessionAndPersonalCookies();

        return
            RedirectToAction(
                routeModel.Action
                , routeModel.Controller
                , routeModel.QueryStringValues
                );
    }



    [NonAction]
    internal IActionResult RedirectToValidateRegistration()
    {
        return RedirectToValidateRegistration(null, null, null);
    }



    [NonAction]
    internal IActionResult RedirectToValidateRegistration(string language, string token, string idConfirm)
    {
        //reset session and identity NOT needed for this feature

        return
            RedirectToAction(
                 MvcComponents.ActValidateRegistrationGet
                , MvcComponents.CtrlAccessValidationRegistration
                , GetQuerystringRoute(
                    language
                    , token
                    , idConfirm
                    , null
                    )
                );
    }



    [NonAction]
    internal IActionResult RedirectToItem(string language, long? itemId)
    {
        return
            RedirectToAction(
                 MvcComponents.ActViewAndManage
                , MvcComponents.CtrlItemManagement
                , GetQuerystringRoute(
                    language
                    , null
                    , null
                    , itemId
                    )
                );
    }

    [NonAction]
    internal IActionResult RedirectToSendErrorReport(string language)
    {
        return
            RedirectToAction(
                 MvcComponents.ActSendErrorReport
                , MvcComponents.CtrlFeedback
                , GetQuerystringRoute(
                    language
                    , null
                    , null
                    , null
                    )
                );
    }


    [NonAction]
    private static IDictionary<string, string> GetQuerystringRoute(
        string language
        , string token
        , string idConfirm
        , long? itemId
        )
    {
        IDictionary<string, string> route = new Dictionary<string, string>();

        AddTokenToRoute(ref route, token);
        AddLanguageToRoute(ref route, language);
        AddIdConfirmToRoute(ref route, idConfirm);
        AddItemIdToRoute(ref route, itemId);

        return route;
    }


    [NonAction]
    internal IActionResult BuildFileResult(FileDownloadInfoLgc fileDownloadInfo)
    {
        return
            File(
               fileContents: fileDownloadInfo.FileContents
               , contentType: fileDownloadInfo.ContentType
               , fileDownloadName: fileDownloadInfo.FileName
               );
    }


    [NonAction]
    private static void AddLanguageToRoute(ref IDictionary<string, string> route, string language)
    {
        //this is ok. only old links issued with language querystring contains "ENG", not worth to create a constant
        if (language.StringHasValue())
        {
            if (language.StartsWithInvariant(
                    SupportedCulturesConstants.IsoCodeEnglish
                    ))
            {
                route.Add(RouteParams.Language, SupportedCulturesConstants.IsoCodeEnglish);
            }

            if (language.StartsWithInvariant(
                    SupportedCulturesConstants.IsoCodeSpanish
                    ))
            {
                route.Add(RouteParams.Language, SupportedCulturesConstants.IsoCodeSpanish);
            }
        }
    }


    [NonAction]
    private static void AddTokenToRoute(ref IDictionary<string, string> route, string token)
    {
        if (token.StringHasValue())
        {
            route.Add(ParamsNames.TenantToken, token);
        }
    }


    [NonAction]
    private static void AddIdConfirmToRoute(ref IDictionary<string, string> route, string idConfirm)
    {
        if (idConfirm.StringHasValue())
        {
            route.Add(ParamsNames.ValidationCode, idConfirm);
        }
    }


    [NonAction]
    private static void AddItemIdToRoute(ref IDictionary<string, string> route, long? itemId)
    {
        if (itemId.Valid())
        {
            route.Add(ParamsNames.ItemId, itemId.ToString());
        }
    }
}