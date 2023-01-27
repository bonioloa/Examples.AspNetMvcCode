namespace Comunica.ProcessManager.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveTenantProfile)]
[Authorize(Policy = PoliciesKeys.TenantHasRegisteredConfig)]
[Authorize(Policy = PoliciesKeys.EnableRegistrationForUsers)]
[ServiceFilter(typeof(RedirectIfHasCompleteLoginFilter), Order = 1)]
public class AccessoRegistrazioneController : BaseContextController
{
    public AccessoRegistrazioneController(
       IHttpContextAccessorCustom httpContextAccessorCustomWeb
        ) : base(httpContextAccessorCustomWeb)
    {
    }


    [HttpGet]
    public IActionResult Registrazione()
    {
        //restore data if fail
        UserRegistrationViewModel model =
            _httpContextAccessorCustomWeb.TempDataOnceRegister ?? new UserRegistrationViewModel();

        return View(MvcComponents.ViewUserRegistration, model);
    }


    [HttpPost]
    public IActionResult RegistrazionePost(
        UserRegistrationViewModel model
        , [FromServices] ContextTenant _contextTenant
        , [FromServices] ICredentialPartialLogic _logicCredentialPartial
        , [FromServices] IRecaptchaWeb _webRecaptcha
        , [FromServices] IEmailWeb _webEmail
        , [FromServices] IResultMessageMapperWeb _webResultMessageMapper
        )
    {
        if (!_webRecaptcha.ValidateReCaptchaRequestIfNeeded())
        {
            _httpContextAccessorCustomWeb.TempDataOnceRegister = model;

            //service already saved error message in temp data
            return RedirectToAction(
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

        _httpContextAccessorCustomWeb.SessionOperationResult =
            _webResultMessageMapper.SetRegistrationResultMessage(modelMessage);

        if (result.Success)
        {
            _webEmail.SendEmailRegistration(
                userName: result.UserName
                , userLogin: result.UserLogin
                , password: result.Password
                , email: result.Email
                , validationCode: result.ValidationCode
                , base.Url.AbsoluteAction(
                    MvcComponents.ActValidateRegistrationGet
                    , MvcComponents.CtrlAccessValidationRegistration
                    , new Dictionary<string, string>()
                        {
                                { ParamsNames.TenantToken, _contextTenant.Token.ToUpper()},
                                { ParamsNames.ValidationCode, WebUtility.HtmlEncode(result.ValidationCode)}
                        })
                , base.Url.AbsoluteAction(
                    MvcComponents.ActValidateRegistrationGet
                    , MvcComponents.CtrlAccessValidationRegistration
                    )
                );

            return this.BaseRedirectToInitialLoginPage();
        }
        else
        {
            //aggiungere a temp data il modello e ricaricare pagina mostrando errori
            _httpContextAccessorCustomWeb.TempDataOnceRegister = model;

            return RedirectToAction(
                MvcComponents.ActRegistrationGet
                , MvcComponents.CtrlAccessRegistration
                );
        }
    }
}
