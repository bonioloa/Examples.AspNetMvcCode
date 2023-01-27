namespace Examples.AspNetMvcCode.Web.Controllers;

/// <summary>
/// this controller for now returns only FileResult or redirect
/// so no views associated to actions
/// </summary>
[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.IfUserHasLoginCodeMustMatchCurrentItem)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 1)]
[ServiceFilter(typeof(RedirectIfAccessSimpleAnonymousFilter), Order = 3)]
[ServiceFilter(typeof(RequiresManagedItemIdFilter), Order = 4)]
public class GestioneAllegatiController : BaseContextController
{
    private readonly ContextUser _contextUser;

    private readonly IFileAttachmentLogic _logicFileAttachment;

    private readonly IMainLocalizer _localizer;

    public GestioneAllegatiController(
       ContextUser contextUser
        , IFileAttachmentLogic logicFileAttachment
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IMainLocalizer localizer
        ) : base(webHttpContextAccessor)
    {
        _contextUser = contextUser;
        _logicFileAttachment = logicFileAttachment;
        _localizer = localizer;
    }



    #region azioni allegati schede

    [HttpPost]
    [Authorize(Policy = PoliciesKeys.UserIsSupervisor)]
    public IActionResult CancellaAllegatoScheda(
        long idAllegato
        , string fase
        , string stato
        )
    {
        OperationResultLgc result =
            _logicFileAttachment.Delete(idAllegato, fase.Clean(), stato.Clean());

        OperationResultViewModel modelMessage = result.MapFromLogicToWeb();

        modelMessage.LocalizedMessage =
            result.Success
            ? _localizer[nameof(LocalizedStr.AttachmentDeleteSuccessMessage)]
            : _localizer[nameof(LocalizedStr.AttachmentDeleteErrorMessage)];

        _webHttpContextAccessor.SessionOperationResult = modelMessage;

        //reload current item
        return
            RedirectToAction(
                MvcComponents.ActViewAndManage
                , MvcComponents.CtrlItemManagement
                , new RouteValueDictionary(
                    new Dictionary<string, string>()
                    {
                        { ParamsNames.ItemId, _contextUser.ItemIdCurrentlyManagedByUser.ToString()}
                    })
                );
    }

    /// <summary>
    /// used by an ajax post and returns a json
    /// </summary>
    /// <param name="idAllegato"></param>
    /// <param name="fase"></param>
    /// <param name="stato"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = PoliciesKeys.UserIsSupervisor)]
    public IActionResult CancellaAllegatoSchedaNoReload(
        long idAllegato
        , string fase
        , string stato
        )
    {
        OperationResultLgc result =
            _logicFileAttachment.Delete(idAllegato, fase.Clean(), stato.Clean());

        string message =
            result.Success
            ? _localizer[nameof(LocalizedStr.AttachmentDeleteSuccessMessage)]
            : _localizer[nameof(LocalizedStr.AttachmentDeleteErrorMessage)];

        return
            Json(
                new AttachmentDeleteResultJsonModel()
                {
                    Message = message,

                    IdForDeletedAttachmentLinkRemove =
                        WebAppConstants.HtmlIdElemLinkAttachDelete
                        + WebAppUtility.BuildCodeForAttachmentDelete(
                            attachId: idAllegato
                            , phase: fase
                            , state: stato
                            ),
                });
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="idAllegato"></param>
    /// <param name="fase"></param>
    /// <param name="stato"></param>
    /// <returns>IActionResult because method can redirect in case of error</returns>
    [HttpGet]
    public IActionResult ScaricaAllegatoStepStoricizzato(
        [RequiredFromQuery] long idAllegato
        , [RequiredFromQuery][ValidateAsStringSimpleFromQuery] string fase
        , [RequiredFromQuery][ValidateAsLiteralStringFromQuery] string stato
        )
    {
        return
            DownloadAttachment(
                idAllegato
                , fase.Clean()
                , stato.Clean()
                , PermissionType.View
                );
    }


    /// <summary>
    /// only supervisors can download attachments
    /// of a stepForm not finalized 
    /// </summary>
    /// <param name="idAllegato"></param>
    /// <param name="fase"></param>
    /// <param name="stato"></param>
    /// <returns>IActionResult because method can redirect in case of errore</returns>
    [HttpGet]
    [Authorize(Policy = PoliciesKeys.UserIsSupervisor)]
    public IActionResult ScaricaAllegatoScheda(
        [RequiredFromQuery] long idAllegato
        , [RequiredFromQuery][ValidateAsStringSimpleFromQuery] string fase
        , [RequiredFromQuery][ValidateAsLiteralStringFromQuery] string stato
        )
    {
        return
            DownloadAttachment(
                idAllegato
                , fase.Clean()
                , stato.Clean()
                , PermissionType.Modify //only user with modify permission can download files from a incomplete step
                );
    }



    [NonAction]
    private IActionResult DownloadAttachment(
        long idAllegato
        , string fase
        , string stato
        , PermissionType permissionToCheck
        )
    {
        FileAttachmentLgc result =
            _logicFileAttachment.Download(
                idAllegato
                , phase: fase.Clean()
                , state: stato.Clean()
                , permissionToCheck
                );

        if (result.Success)
        {
            return
                File(
                    result.ByteContent
                    , MimeTypes.GenericContentType
                    , result.Name
                    );
        }


        OperationResultViewModel modelMessage = result.MapFileAttachmentResult();
        modelMessage.LocalizedMessage =
            _localizer[nameof(LocalizedStr.AttachmentDownloadErrorMessage)];


        _webHttpContextAccessor.SessionOperationResult = modelMessage;

        return
            RedirectToAction(
                MvcComponents.ActViewAndManage
                , MvcComponents.CtrlItemManagement
                , new RouteValueDictionary(
                    new Dictionary<string, string>()
                    {
                        { ParamsNames.ItemId, _contextUser.ItemIdCurrentlyManagedByUser.ToString()},
                    })
                );
    }


    #endregion




    /// <summary>
    /// 
    /// </summary>
    /// <param name="idMessaggio"></param>
    /// <param name="idAllegato"></param>
    /// <param name="fase">for validation reasons</param>
    /// <param name="stato">for validation reasons</param>
    /// <returns></returns>
    /// <remarks>this download method is a get, parameters are passed with querystring</remarks>
    [HttpGet]
    public IActionResult ScaricaAllegatoMessaggioStoricizzato(
        [RequiredFromQuery] long idMessaggio
        , [RequiredFromQuery] long idAllegato
        , [RequiredFromQuery][ValidateAsStringSimpleFromQuery] string fase
        , [RequiredFromQuery][ValidateAsLiteralStringFromQuery] string stato
        )
    {
        FileAttachmentLgc result =
             _logicFileAttachment.DownloadMessageAttachment(
                  messageId: idMessaggio
                  , fileAttachmentId: idAllegato
                  , phase: fase.Clean()
                  , state: stato.Clean()
                  );


        if (result.Success)
        {
            return
                File(
                    result.ByteContent
                    , MimeTypes.GenericContentType
                    , result.Name
                    );
        }


        OperationResultViewModel modelMessage = result.MapFileAttachmentResult();
        modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.AttachmentDownloadErrorMessage)];

        _webHttpContextAccessor.SessionOperationResult = modelMessage;


        return RedirectToAction(
            MvcComponents.ActViewAndManage
            , MvcComponents.CtrlItemManagement
            , new Dictionary<string, string>()
                {
                    { ParamsNames.ItemId, _contextUser.ItemIdCurrentlyManagedByUser.ToString() }
                });
    }
}