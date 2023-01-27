namespace Comunica.ProcessManager.Web.Controllers;

/// <summary>
/// tenant is a user input, so this can be accessed without login from tenant token page
/// there is no need to reset session or login because this page does not use 
/// data from session or identity.
/// Do not use session/identity reset because it can cause problems with consent
/// back url handling
/// </summary>
[AllowAnonymous]
public class AccessoValidazioneRegistrazioneController : BaseContextController
{
    public AccessoValidazioneRegistrazioneController(
        IHttpContextAccessorCustom httpContextAccessorCustomWeb
        ) : base(httpContextAccessorCustomWeb)
    {
    }



    //this link is available without passing from tenant login (link in email)
    [HttpGet]
    public IActionResult ValidaRegistrazione(
        [ValidateAsStringSimpleFromQuery] string token
        , [ValidateAsStringSimpleFromQuery] string codiceconferma
        )
    {
        UserValidateRegistrationViewModel model = new();

        if (token.StringHasValue())
        {
            model.TenantToken = token.Clean();
        }

        if (codiceconferma.StringHasValue())
        {
            model.ValidationCode = codiceconferma;//do not clean
        }


        return View(MvcComponents.ViewUserValidateRegistration, model);
    }




    //this is available without tenant login (link in email)
    [HttpPost]
    public IActionResult ValidaRegistrazionePost(
        UserValidateRegistrationViewModel model
        , [FromServices] ICredentialExternalLogic _logicCredentialExternal
        , [FromServices] IRecaptchaWeb _webRecaptcha
        , [FromServices] IEmailWeb _webEmail
        , [FromServices] IResultMessageMapperWeb _webResultMessageMapper
        )
    {
        if (!_webRecaptcha.ValidateReCaptchaRequestIfNeeded())
        {
            //service already saved error message in temp data
            return RedirectToValidateRegistration(model);
        }
        UserValidateRegistrationResultLgc result =
            _logicCredentialExternal.ActivateNewUser(
                model.TenantToken.Clean()
                , model.ValidationCode //do not clean
                , _httpContextAccessorCustomWeb.HttpContext.Connection.RemoteIpAddress
                );

        //we have to show a message regardless result
        OperationResultViewModel modelMessage = result.MapUserValidateRegistrationResult();


        _httpContextAccessorCustomWeb.SessionOperationResult =
            _webResultMessageMapper.SetValidateRegistrationResultMessage(modelMessage);

        if (result.Success)
        {
            _webEmail.SendEmailRegistrationValidated(result.Email);
            return BaseRedirectToInitialLoginPage();
        }
        else
        {
            return RedirectToValidateRegistration(model);
        }
    }



    [NonAction]
    private IActionResult RedirectToValidateRegistration(UserValidateRegistrationViewModel model)
    {
        if (model != null)
        {
            return base.RedirectToValidateRegistration(
                language: null //request language will be used
                , model.TenantToken.Clean()
                , model.ValidationCode//do not clean
                );
        }
        else
        {
            return base.RedirectToValidateRegistration();
        }
    }
}
