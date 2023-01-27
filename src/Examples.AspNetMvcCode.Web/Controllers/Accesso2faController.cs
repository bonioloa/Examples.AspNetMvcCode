namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveTenantProfile)]
[Authorize(Policy = PoliciesKeys.TenantHasTwoFactorAuthenticationEnabled)]
[ServiceFilter(typeof(RedirectIfHasCompleteLoginFilter), Order = 1)]
public class Accesso2faController : BaseContextController
{
    public Accesso2faController(
        IHttpContextAccessorWeb webHttpContextAccessor
        ) : base(webHttpContextAccessor)
    {
    }



    [HttpGet]
    public IActionResult ValidazioneCodice()
    {
        UserProfileModel profile = _webHttpContextAccessor.TempDataOnceUserProfile;

        if (profile is null)
        {
            return BaseRedirectToInitialLoginPage();
        }


        //keep alive value
        _webHttpContextAccessor.TempDataOnceUserProfile = profile;

        return View(MvcComponents.ViewUserLogin2fa);
    }



    [HttpPost]
    public IActionResult ValidazioneCodicePost(
        UserLogin2faViewModel model
        , [FromServices] IStrongAuthenticationLogic _logicStrongAuthentication
        , [FromServices] IAuditLogic _logicAudit
        , [FromServices] IMainLocalizer _localizer
        , [FromServices] ICaptchaWeb _webCaptcha
        )
    {
        UserProfileModel profile = _webHttpContextAccessor.TempDataOnceUserProfile;

        if (profile is null)
        {
            return BaseRedirectToInitialLoginPage();
        }

        //keep alive value, in case captcha fails
        _webHttpContextAccessor.TempDataOnceUserProfile = profile;

        if (!_webCaptcha.ValidateCaptchaRequestIfNeeded())
        {
            return
                RedirectToAction(
                   MvcComponents.ActLogin2faGet
                   , MvcComponents.CtrlAccess2fa
                   );
        }

        OperationResultLgc result = _logicStrongAuthentication.Validate2faCode(model.EmailAuthenticationCode);

        if (!result.Success)
        {
            //don't show errors of credentials not found, 
            //simply show access refusal and if some parameter is missing or has invalid characters
            OperationResultViewModel modelMessage = result.MapFromLogicToWeb();

            modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.SharedErrorAccessDenied)];

            _webHttpContextAccessor.SessionOperationResult = modelMessage;


            return BaseRedirectToInitialLoginPage();
        }

        if (!_webHttpContextAccessor.SignInUser(profile))
        {
            return BaseRedirectToInitialLoginPage();
        }


        _logicAudit.LogUserLogin();


        //Strong authentication for now is only provided for supervisors
        return
            RedirectToAction(
               MvcComponents.ActSearchNew
               , MvcComponents.CtrlSearch
               );
    }
}