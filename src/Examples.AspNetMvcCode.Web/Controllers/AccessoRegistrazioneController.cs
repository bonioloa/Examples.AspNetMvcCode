namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveTenantProfile)]
[Authorize(Policy = PoliciesKeys.TenantHasRegisteredConfig)]
[Authorize(Policy = PoliciesKeys.EnableRegistrationForUsers)]
[ServiceFilter(typeof(RedirectIfHasCompleteLoginFilter), Order = 1)]
public class AccessoRegistrazioneController : BaseContextController
{
    private readonly ICryptingLogic _logicCrypting;
    public AccessoRegistrazioneController(
        IHttpContextAccessorWeb webHttpContextAccessor
        , ICryptingLogic logicCrypting
        ) : base(webHttpContextAccessor)
    {
        _logicCrypting = logicCrypting;
    }


    [HttpGet]
    public IActionResult Registrazione()
    {
        //restore data if fail
        UserRegistrationViewModel model =
            _webHttpContextAccessor.TempDataOnceRegister ?? new UserRegistrationViewModel();

        return View(MvcComponents.ViewUserRegistration, model);
    }


    [HttpPost]
    public IActionResult RegistrazionePost(
        UserRegistrationViewModel model
        , [FromServices] ContextTenant _contextTenant
        , [FromServices] ICredentialPartialLogic _logicCredentialPartial
        , [FromServices] IEmailSendSystemLogic _logicEmailSendBase
        , [FromServices] ICaptchaWeb _webCaptcha
        , [FromServices] IResultMessageMapperWeb _webResultMessageMapper
        )
    {
        if (!_webCaptcha.ValidateCaptchaRequestIfNeeded())
        {
            _webHttpContextAccessor.TempDataOnceRegister = model;

            //service already saved error message in temp data
            return
                RedirectToAction(
                    MvcComponents.ActRegistrationGet
                    , MvcComponents.CtrlAccessRegistration
                    );
        }

        UserRegistrationResultLgc result =
            _logicCredentialPartial.CreateNewUser(
                userLogin: model.Login.Clean()
                , password: model.Password.Clean()
                , confirmationPassword: model.ConfirmPassword.Clean()
                , email: model.Email.Clean()
                , name: model.Nome.Clean()
                , surname: model.Cognome.Clean()
                );

        //we have to show a message in both cases
        OperationResultViewModel modelMessage = result.MapUserRegistrationResult();

        _webHttpContextAccessor.SessionOperationResult =
            _webResultMessageMapper.SetRegistrationResultMessage(modelMessage);


        if (!result.Success)
        {
            //aggiungere a temp data il modello e ricaricare pagina mostrando errori
            _webHttpContextAccessor.TempDataOnceRegister = model;

            return
                RedirectToAction(
                    MvcComponents.ActRegistrationGet
                    , MvcComponents.CtrlAccessRegistration
                    );

        }


        string validationCodeWrapped = _logicCrypting.WrapStringToHex(result.ValidationCode);

        _logicEmailSendBase.SendEmailRegistration(
            new EmailRegistrationLgc(
                UserName: result.UserName
                , UserLogin: result.UserLogin
                , Password: result.Password
                , Email: result.Email
                , ValidationCode: result.ValidationCode

                , CompleteUrlValidationWithParamenter:
                    base.Url.AbsoluteAction(
                        MvcComponents.ActValidateRegistrationGet
                        , MvcComponents.CtrlAccessValidationRegistration
                        , new Dictionary<string, object>()
                            {
                                { ParamsNames.TenantToken, _contextTenant.Token.ToUpperInvariant()},
                                { ParamsNames.ValidationCode, validationCodeWrapped}
                            })

                , CompleteUrlValidationSimple:
                    base.Url.AbsoluteAction(
                        MvcComponents.ActValidateRegistrationGet
                        , MvcComponents.CtrlAccessValidationRegistration
                        )

                , Token: _contextTenant.Token
                )
            );

        return
            RedirectToAction(
                MvcComponents.ActLoginUser
                , MvcComponents.CtrlAccessUser
                );
    }
}