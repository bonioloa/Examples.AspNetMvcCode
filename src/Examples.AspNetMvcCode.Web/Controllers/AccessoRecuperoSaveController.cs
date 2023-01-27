namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveTenantProfile)]
[ServiceFilter(typeof(RedirectIfHasCompleteLoginFilter), Order = 1)]
public class AccessoRecuperoSaveController : BaseContextController
{
    private readonly ICredentialPartialLogic _logicCredentialPartial;
    private readonly IEmailSendSystemLogic _logicEmailSendBase;

    private readonly IMainLocalizer _localizer;
    private readonly ICaptchaWeb _webCaptcha;


    public AccessoRecuperoSaveController(
        ICredentialPartialLogic logicCredentialPartial
        , IEmailSendSystemLogic logicEmailSendBase
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IMainLocalizer localizer
        , ICaptchaWeb webCaptcha
        ) : base(webHttpContextAccessor)
    {
        _logicCredentialPartial = logicCredentialPartial;
        _logicEmailSendBase = logicEmailSendBase;
        _localizer = localizer;
        _webCaptcha = webCaptcha;
    }



    [HttpPost]
    public IActionResult Recover(UserRecoverCredentialsViewModel model)
    {
        if (!_webCaptcha.ValidateCaptchaRequestIfNeeded())
        {
            //service already saved error message in temp data
            return
                RedirectToAction(
                   MvcComponents.ActRecoverUserData
                   , MvcComponents.CtrlAccessRecover
                   );
        }


        Guard.Against.Null(model, nameof(model));

        UserRecoverCredentialsResultLgc result =
            _logicCredentialPartial.DoRecover(
                model.Login.Clean()
                , model.Email.Clean()
                );

        if (result.Success)
        {
            _logicEmailSendBase.SendEmailCredentialRecover(
                result.RecoverType
                , result.Password
                , result.UserLogin
                , result.Email
                );

            _webHttpContextAccessor.TempDataOnceRecoverSuccess = true;

            return
                RedirectToAction(
                    MvcComponents.ActResultRecoverUserData
                    , MvcComponents.CtrlAccessRecover
                    );
        }


        OperationResultViewModel modelResult = result.MapUserRecoverCredentialsResult();

        modelResult.LocalizedMessage =
            _localizer[nameof(LocalizedStr.UserRecoverCredentialsErrorMessage)];

        _webHttpContextAccessor.SessionOperationResult = modelResult;


        return
            RedirectToAction(
                MvcComponents.ActRecoverUserData
                , MvcComponents.CtrlAccessRecover
                );
    }
}