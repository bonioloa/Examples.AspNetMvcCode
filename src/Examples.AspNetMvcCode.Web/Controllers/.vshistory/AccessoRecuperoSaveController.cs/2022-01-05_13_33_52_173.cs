namespace Comunica.ProcessManager.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveTenantProfile)]
[ServiceFilter(typeof(RedirectIfHasCompleteLoginFilter), Order = 1)]
public class AccessoRecuperoSaveController : BaseContextController
{
    private readonly ICredentialPartialLogic _logicCredentialPartial;

    private readonly MainLocalizer _localizer;
    private readonly IRecaptchaWeb _webRecaptcha;
    private readonly IEmailWeb _webEmail;

    public AccessoRecuperoSaveController(
        ICredentialPartialLogic logicCredentialPartial
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , MainLocalizer localizer
        , IRecaptchaWeb webRecaptcha
        , IEmailWeb webEmail
        ) : base(httpContextAccessorCustomWeb)
    {
        _logicCredentialPartial = logicCredentialPartial;
        _localizer = localizer;
        _webRecaptcha = webRecaptcha;
        _webEmail = webEmail;
    }



    [HttpPost]
    public IActionResult Recover(UserRecoverCredentialsViewModel model)
    {
        if (!_webRecaptcha.ValidateReCaptchaRequestIfNeeded())
        {
            //service already saved error message in temp data
            return RedirectToAction(
               MvcComponents.ActRecoverUserData
               , MvcComponents.CtrlAccessRecover
               );
        }

        UserRecoverCredentialsResultLgc result =
            _logicCredentialPartial.DoRecover(model.Login.Clean(), model.Email.Clean());

        if (result.Success)
        {
            _webEmail.SendEmailCredentialRecover(
                result.RecoverType
                , result.Password
                , result.UserLogin
                , result.Email
                );

            _httpContextAccessorCustomWeb.TempDataOnceRecoverSuccess = true;

            return RedirectToAction(
                MvcComponents.ActResultRecoverUserData
                , MvcComponents.CtrlAccessRecover
                );
        }
        else
        {
            OperationResultViewModel modelResult = result.MapUserRecoverCredentialsResult();
            modelResult.LocalizedMessage =
                _localizer[nameof(LocalizedStr.UserRecoverCredentialsErrorMessage)];
            _httpContextAccessorCustomWeb.SessionOperationResult = modelResult;

            return RedirectToAction(
               MvcComponents.ActRecoverUserData
               , MvcComponents.CtrlAccessRecover
               );
        }
    }
}
