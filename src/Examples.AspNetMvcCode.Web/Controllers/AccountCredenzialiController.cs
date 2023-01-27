namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.UserAccessedWithLoginAndPassword)]
public class AccountCredenzialiController : BaseContextController
{
    public AccountCredenzialiController(
        IHttpContextAccessorWeb webHttpContextAccessor
        ) : base(webHttpContextAccessor)
    {

    }



    [HttpGet]
    public IActionResult CambioPassword()
    {
        //no model
        return View(MvcComponents.ViewUserChangePassword);
    }


    [HttpPost]
    public IActionResult CambioPassword(
        UserChangePasswordViewModel changePassword
        , [FromServices] ICredentialInternalLogic _logicCredentialInternal
        , [FromServices] IMainLocalizer _localizer
        )
    {
        UserChangePasswordResultLgc result =
            _logicCredentialInternal.ValidateAndChange(
                changePassword.OldPassword.Clean()
                , changePassword.NewPassword.Clean()
                , changePassword.ConfirmPassword.Clean()
                );

        if (result.Success)
        {
            UserChangePasswordResultViewModel resultModel = result.MapFromLogicToWeb();

            _webHttpContextAccessor.TempDataOnceChangePasswordResult = resultModel;

            return
                RedirectToAction(
                    MvcComponents.ActResultChangePassword
                    , MvcComponents.CtrlAccountCredentials
                    );
        }


        OperationResultViewModel modelMessage = result.MapUserChangePasswordResult();

        modelMessage.LocalizedMessage =
            _localizer[nameof(LocalizedStr.UserChangePasswordResultErrorPageTitle)];

        _webHttpContextAccessor.SessionOperationResult = modelMessage;

        //don't restore sent model, they are password and they are unreadable for user
        //reload page to show error
        return
            RedirectToAction(
                MvcComponents.ActChangePassword
                , MvcComponents.CtrlAccountCredentials
                );
    }


    [HttpGet]
    public IActionResult EsitoCambioPassword(
        [FromServices] IHttpContextAccessorWeb _webHttpContextAccessor
        )
    {
        UserChangePasswordResultViewModel result =
            _webHttpContextAccessor.TempDataOnceChangePasswordResult;

        return
            result is null
            ? RedirectToAction(
                MvcComponents.ActChangePassword
                , MvcComponents.CtrlAccountCredentials
                )
            : View(MvcComponents.ViewUserChangePasswordResult, result);
    }
}