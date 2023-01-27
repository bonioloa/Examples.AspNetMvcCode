namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.UserIsAdmin)]
[ServiceFilter(typeof(RedirectIfAccessWithLoginCodeFilter), Order = 1)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 2)]
public class AmministrazioneUtentiPostController : BaseContextController
{
    private readonly ContextTenant _contextTenant;
    private readonly IUrlHelper _urlHelper;

    private readonly ISupervisorAdministrationLogic _logicSupervisorAdministration;
    private readonly ISupervisorSaveLogic _logicSupervisorSave;
    private readonly IEmailSendSystemLogic _logicEmailSendSystem;

    private readonly IMainLocalizer _localizer;

    public AmministrazioneUtentiPostController(
        ContextTenant contextTenant
        , IUrlHelper urlHelper
        , ISupervisorAdministrationLogic logicSupervisorAdministration
        , ISupervisorSaveLogic logicSupervisorSave
        , IEmailSendSystemLogic logicEmailSendSystem
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IMainLocalizer localizer
        ) : base(webHttpContextAccessor)
    {
        _contextTenant = contextTenant;
        _urlHelper = urlHelper;
        _logicSupervisorAdministration = logicSupervisorAdministration;
        _logicSupervisorSave = logicSupervisorSave;
        _logicEmailSendSystem = logicEmailSendSystem;
        _localizer = localizer;
    }


    [HttpPost]
    public IActionResult NuovoResponsabilePost(
        UserNewSupervisorSaveViewModel newSupervisor
        )
    {
        Guard.Against.Null(newSupervisor, nameof(newSupervisor));

        //clean string properties and override
        UserNewSupervisorSaveViewModel newSupervisorCleaned =
            newSupervisor with
            {
                Login = newSupervisor.Login.Clean(),
                Nome = newSupervisor.Nome.Clean(),
                Cognome = newSupervisor.Cognome.Clean(),
                Email = newSupervisor.Email.Clean()
            };

        UserNewSupervisorResultLgc userNewSupervisorResult =
            _logicSupervisorSave.SaveNewSupervisor(
                newSupervisorCleaned.MapFromWebToLogicCustomWithNullCheck()
                );

        if (userNewSupervisorResult.ErrorMessage.StringHasValue())
        {
            return SaveNewSupervisorFail(newSupervisorCleaned, userNewSupervisorResult);
        }

        UserSupervisorDataLgc userSupervisorData =
            new(
                UserId: userNewSupervisorResult.NewUserIdSupervisor
                , UserMustBeIncludedInManagedBeforeEdits: default //useless in this context
                , IsActive: default  //useless in this context
                , Login: newSupervisorCleaned.Login
                , Name: newSupervisorCleaned.Nome
                , Surname: newSupervisorCleaned.Cognome
                , Email: newSupervisorCleaned.Email
                , RolesSelection: //USELESS HERE FOR MAIL SEND
                    new RolesSelectionLgc(
                        ExclusiveRolesFound: Enumerable.Empty<OptionLgc>()
                        , SupervisorRolesFound: Enumerable.Empty<OptionLgc>()
                    )
                );

        _logicEmailSendSystem.SendCredentialsForSupervisor(
            SupervisorCredentialsEmail.NewSupervisor
            , userSupervisorData: userSupervisorData
            , password: userNewSupervisorResult.Password
            , loginPageWithToken: _urlHelper.AbsoluteActionAccessPage(_contextTenant.Token)
            );

        SaveNewSupervisorSetSuccessMessage();

        return
            RedirectToAction(
                MvcComponents.ActNewSupervisor
                , MvcComponents.CtrlAdministrationUsers
                );
    }


    [NonAction]
    private IActionResult SaveNewSupervisorFail(
        UserNewSupervisorSaveViewModel newSupervisor
        , UserNewSupervisorResultLgc userNewSupervisorResult
        )
    {
        SaveNewSupervisorSetErrorMessages(userNewSupervisorResult);

        _webHttpContextAccessor.TempDataOnceUserNewSupervisorSave = newSupervisor;

        return RedirectToAction(
           MvcComponents.ActNewSupervisor
           , MvcComponents.CtrlAdministrationUsers
           );
    }

    [NonAction]
    private void SaveNewSupervisorSetErrorMessages(UserNewSupervisorResultLgc userNewSupervisorResult)
    {
        OperationResultViewModel modelMessage =
            new()
            {
                Success = false,
                LocalizedTitle = _localizer[nameof(LocalizedStr.UserNewSupervisorErrorMessage)],
                LocalizedMessage = userNewSupervisorResult.ErrorMessage
            };

        _webHttpContextAccessor.SessionOperationResult = modelMessage;
    }

    [NonAction]
    private void SaveNewSupervisorSetSuccessMessage()
    {
        OperationResultViewModel modelMessage =
            new()
            {
                Success = true,
                LocalizedMessage = _localizer[nameof(LocalizedStr.UserNewSupervisorSuccessMessage)],
            };

        _webHttpContextAccessor.SessionOperationResult = modelMessage;
    }




    [HttpPost]
    public IActionResult IncludiUtenteInGestionePost(long idutente)
    {
        string error = _logicSupervisorSave.IncludeUserInManaged(idutente);

        if (error.StringHasValue())
        {
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel()
                {
                    Success = false,
                    LocalizedMessage = error,
                };
            //when error happens we are not sure if userId is valid, so it's better to return to search users
            return
                RedirectToAction(
                    MvcComponents.ActSupervisorManagement
                    , MvcComponents.CtrlAdministrationUsers
                    );
        }
        else
        {
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel()
                {
                    Success = true,
                    LocalizedMessage = "Utente incluso con successo fra gli utenti gestiti",
                };

            return
                RedirectToAction(
                    MvcComponents.ActSupervisorEdit
                    , MvcComponents.CtrlAdministrationUsers
                    , new Dictionary<string, string>() { { ParamsNames.UserId, idutente.ToString() } }
                    );
        }
    }



    //fare reset password utenza e inviare email credenziali
    [HttpPost]
    public IActionResult AttivaUtentePost(long idutente)
    {
        UserResetResultLgc userResetResult = _logicSupervisorSave.EnableUser(userId: idutente);

        Guard.Against.Null(userResetResult, nameof(userResetResult));

        if (userResetResult.ErrorResult.StringHasValue())
        {
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel()
                {
                    Success = false,
                    LocalizedMessage = userResetResult.ErrorResult,
                };

            //when error happens we are not sure if userId is valid, so it's better to return to search users
            return
                RedirectToAction(
                    MvcComponents.ActSupervisorManagement
                    , MvcComponents.CtrlAdministrationUsers
                    );
        }

        UserSupervisorDataLgc userSupervisorData = _logicSupervisorAdministration.GetUserSupervisorData(userId: idutente);

        _logicEmailSendSystem.SendCredentialsForSupervisor(
            SupervisorCredentialsEmail.EnabledSupervisor
            , userSupervisorData: userSupervisorData
            , password: userResetResult.NewPassword
            , loginPageWithToken: _urlHelper.AbsoluteActionAccessPage(_contextTenant.Token)
            );


        _webHttpContextAccessor.SessionOperationResult =
               new OperationResultViewModel()
               {
                   Success = true,
                   LocalizedMessage = "Utente attivato con successo. E' stata inviata una email con la nuova password assegnata",
               };

        //we can return to user edit page because we are sure that userId is valid
        return
            RedirectToAction(
                MvcComponents.ActSupervisorEdit
                , MvcComponents.CtrlAdministrationUsers
                , new Dictionary<string, string>() { { ParamsNames.UserId, idutente.ToString() } }
                );
    }



    [HttpPost]
    public IActionResult DisattivaUtentePost(long idutente)
    {
        //deve anche rimuovere ruoli collegati oltre che disattivare utente

        string errorMessage = _logicSupervisorSave.DisableUser(userId: idutente);

        if (errorMessage.StringHasValue())
        {
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel()
                {
                    Success = false,
                    LocalizedMessage = errorMessage,
                };

            //when error happens we are not sure if userId is valid, so it's better to return to search users
            return
                RedirectToAction(
                    MvcComponents.ActSupervisorManagement
                    , MvcComponents.CtrlAdministrationUsers
                    );
        }


        _webHttpContextAccessor.SessionOperationResult =
            new OperationResultViewModel()
            {
                Success = true,
                LocalizedMessage = "Completato! Utente disattivato e i relativi ruoli assegnati sono stati rimossi",
            };

        //we can return to user edit page because we are sure that userId is valid
        return
            RedirectToAction(
                MvcComponents.ActSupervisorEdit
                , MvcComponents.CtrlAdministrationUsers
                , new Dictionary<string, string>() { { ParamsNames.UserId, idutente.ToString() } }
                );
    }


    //fare reset password utenza e inviare email credenziali
    [HttpPost]
    public IActionResult ResetPasswordPost(long idutente)
    {
        UserResetResultLgc userResetResult = _logicSupervisorSave.ResetPasswordFromAdmin(userId: idutente);

        Guard.Against.Null(userResetResult, nameof(userResetResult));

        if (userResetResult.ErrorResult.StringHasValue())
        {
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel()
                {
                    Success = false,
                    LocalizedMessage = userResetResult.ErrorResult,
                };

            //when error happens we are not sure if userId is valid, so it's better to return to search users
            return
                RedirectToAction(
                    MvcComponents.ActSupervisorManagement
                    , MvcComponents.CtrlAdministrationUsers
                    );
        }

        UserSupervisorDataLgc userSupervisorData = _logicSupervisorAdministration.GetUserSupervisorData(userId: idutente);

        _logicEmailSendSystem.SendCredentialsForSupervisor(
            SupervisorCredentialsEmail.ResetPassword
            , userSupervisorData: userSupervisorData
            , password: userResetResult.NewPassword
            , loginPageWithToken: _urlHelper.AbsoluteActionAccessPage(_contextTenant.Token)
            );


        _webHttpContextAccessor.SessionOperationResult =
               new OperationResultViewModel()
               {
                   Success = true,
                   LocalizedMessage = "Password resettata per l'utente selezionato. E' stata inviata una email con la nuova password assegnata",
               };

        //we can return to user edit page because we are sure that userId is valid
        return
            RedirectToAction(
                MvcComponents.ActSupervisorEdit
                , MvcComponents.CtrlAdministrationUsers
                , new Dictionary<string, string>() { { ParamsNames.UserId, idutente.ToString() } }
                );
    }


    [HttpPost]
    public IActionResult ModificaDatiUtente(UserEditSupervisorSaveViewModel input)
    {
        Guard.Against.Null(input, nameof(UserEditSupervisorSaveViewModel));

        UserEditFromAdminResultLgc userEditFromAdminResult =
            _logicSupervisorSave.ModifyUserDataFromAdmin(
                new UserEditFromAdminLgc(
                    UserId: input.IdUtente
                    , Name: input.Nome
                    , Surname: input.Cognome
                    , Email: input.Email
                    , ExclusiveRole: input.ProfiloEsclusivo
                    , SupervisorRoles: input.Profili
                    )
                );

        if (userEditFromAdminResult.ErrorMessage.StringHasValue())
        {
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel()
                {
                    Success = false,
                    LocalizedMessage = userEditFromAdminResult.ErrorMessage,
                };

            if (userEditFromAdminResult.UserId.Valid())
            {
                return
                    RedirectToAction(
                        MvcComponents.ActSupervisorEdit
                        , MvcComponents.CtrlAdministrationUsers
                        , new Dictionary<string, string>() { { ParamsNames.UserId, input.IdUtente.ToString() } }
                        );
            }

            //when error happens and userId is invalid, return to search users
            return
                RedirectToAction(
                    MvcComponents.ActSupervisorManagement
                    , MvcComponents.CtrlAdministrationUsers
                    );
        }

        if (userEditFromAdminResult.EmailChanged)
        {
            _logicEmailSendSystem.SendNotificationsUserForEmailChange(
                userDataBeforeUpdate: userEditFromAdminResult.UserDataBefore
                , userDataCurrent: userEditFromAdminResult.UserDataUpdated
                , loginPageWithToken: _urlHelper.AbsoluteActionAccessPage(_contextTenant.Token)
              );
        }


        _webHttpContextAccessor.SessionOperationResult =
            new OperationResultViewModel()
            {
                Success = true,
                LocalizedMessage =
                    userEditFromAdminResult.EmailChanged
                    ? "Dati Utente modificati con successo. Sono state inviate notifiche all'indirizzo email precedente e sul nuovo"
                    : "Dati Utente modificati con successo.",
            };

        //we can return to user edit page because we are sure that userId is valid
        return
            RedirectToAction(
                MvcComponents.ActSupervisorEdit
                , MvcComponents.CtrlAdministrationUsers
                , new Dictionary<string, string>() { { ParamsNames.UserId, input.IdUtente.ToString() } }
                );
    }
}