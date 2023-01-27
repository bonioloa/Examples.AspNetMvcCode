namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveTenantProfile)]
[ServiceFilter(typeof(RedirectIfHasCompleteLoginFilter), Order = 1)]
public class AccessoUtentePostController : BaseContextController
{
    private readonly ContextTenant _contextTenant;

    private readonly IUserConfiguratorLogic _logicUserConfigurator;
    private readonly IAuditLogic _logicAudit;
    private readonly IEmailSendSystemLogic _logicEmailSendBase;

    private readonly IAuthorizationCustomWeb _webAuthorizationCustom;
    private readonly IMainLocalizer _localizer;
    private readonly ICaptchaWeb _webCaptcha;


    public AccessoUtentePostController(
        ContextTenant contextTenant
        , IUserConfiguratorLogic logicUserConfigurator
        , IAuditLogic logicAudit
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IAuthorizationCustomWeb webAuthorizationCustom
        , IMainLocalizer localizer
        , ICaptchaWeb webCaptcha
        , IEmailSendSystemLogic logicEmailSendBase
        ) : base(webHttpContextAccessor)
    {
        _contextTenant = contextTenant;
        _logicUserConfigurator = logicUserConfigurator;
        _logicAudit = logicAudit;
        _logicEmailSendBase = logicEmailSendBase;
        _webAuthorizationCustom = webAuthorizationCustom;
        _localizer = localizer;
        _webCaptcha = webCaptcha;
    }



    [HttpPost]
    public IActionResult LoginUtenteAnonimo()
    {
        return
            HandleResult(
            _logicUserConfigurator.ValidateAndConfigureLoginAnonymous()
            );
    }

    [HttpPost]
    public IActionResult LoginUtenteCodice(string loginCode)
    {
        if (!_webCaptcha.ValidateCaptchaRequestIfNeeded())
        {
            //service already saved error message in temp data
            _webHttpContextAccessor.TempDataOnceLoginUser =
                new UserLoginViewModel()
                {
                    FormToShow = LoginType.LoginCode,
                };

            return
                RedirectToAction(
                    MvcComponents.ActLoginUser
                    , MvcComponents.CtrlAccessUser
                    );
        }

        return
            HandleResult(
                _logicUserConfigurator.ValidateAndConfigureLoginCode(loginCode.Clean())
                );
    }

    [HttpPost]
    public IActionResult LoginUtenteCredenziali(string userLogin, string password)
    {
        return
            HandleResult(
                _logicUserConfigurator.ValidateAndConfigureLogin(
                    userLogin.Clean()
                    , password.Clean()
                    )
                );
    }



    [NonAction]
    private IActionResult HandleResult(UserProfileLgc profile)
    {
        if (!profile.Success)
        {
            //don't show errors of credentials not found, 
            //simply show access refusal and if some parameter is missing or has invalid characters
            OperationResultViewModel modelMessage = profile.MapUserProfileResult();

            modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.SharedErrorAccessDenied)];


            _webHttpContextAccessor.SessionOperationResult = modelMessage;

            _webHttpContextAccessor.TempDataOnceLoginUser =
                new UserLoginViewModel()
                {
                    FormToShow = profile.FormToShow,
                };

            return
                RedirectToAction(
                    MvcComponents.ActLoginUser
                    , MvcComponents.CtrlAccessUser
                    );
        }

        UserProfileModel profileModel = profile.MapFromLogicToWeb();

        //check for redirect to strong authentication page
        if (_contextTenant.TwoFactorAuthenticationEnabled)
        {
            _logicEmailSendBase.SendEmail2faCode(profile.Email, profile.StrongAuthAccessCode);

            //backup userprofile for final authentication
            _webHttpContextAccessor.TempDataOnceUserProfile = profileModel;

            return
                RedirectToAction(
                    MvcComponents.ActLogin2faGet
                    , MvcComponents.CtrlAccess2fa
                    );
        }


        if (!_webHttpContextAccessor.SignInUser(profileModel))
        {
            return BaseRedirectToInitialLoginPage();
        }

        _logicAudit.LogUserLogin();

        return
            RedirectToRoute(
                _webAuthorizationCustom.GetLandingPageByRole(profile.ItemIdFromLoginCode, null)
                );
    }
}