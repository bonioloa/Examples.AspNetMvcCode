namespace Comunica.ProcessManager.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 1)]
public class ElementiSalvaController : BaseContextController
{
    private readonly ILogger<ElementiSalvaController> _logger;
    private readonly ContextUser _contextUser;

    private readonly IItemManagementSaveLogic _logicItemManagementSave;

    private readonly IHtmlFormToModelMapperWeb _webHtmlFormToModelMapper;
    private readonly IEmailWeb _webEmail;
    private readonly IResultMessageMapperWeb _webResultMessageMapper;
    private readonly IExcelTemplateWeb _webExcelTemplate;

    public ElementiSalvaController(
        ILogger<ElementiSalvaController> logger
        , ContextUser contextUser
        , IItemManagementSaveLogic logicItemManagementSave
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , IHtmlFormToModelMapperWeb webHtmlFormToModelMapper
        , IEmailWeb webEmail
        , IResultMessageMapperWeb webResultMessageMapper
        , IExcelTemplateWeb webExcelTemplate
        ) : base(httpContextAccessorCustomWeb)
    {
        _logger = logger;
        _contextUser = contextUser;
        _logicItemManagementSave = logicItemManagementSave;
        _webHtmlFormToModelMapper = webHtmlFormToModelMapper;
        _webEmail = webEmail;
        _webResultMessageMapper = webResultMessageMapper;
        _webExcelTemplate = webExcelTemplate;
    }




    /// <summary>
    /// This action is used only for new item creation
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    [ServiceFilter(typeof(RedirectIfAccessWithLoginCodeFilter), Order = 1)]
    [ServiceFilter(typeof(RequiresProcessIdFilter), Order = 4)]
    [HttpPost]
    public IActionResult SalvaNuovo(IFormCollection form)
    {
        return Json(
            Save(form, isNewItemSave: true)
            );
    }



    [ServiceFilter(typeof(RedirectIfAccessWithLoginCodeFilter), Order = 1)]
    [ServiceFilter(typeof(RequiresProcessIdFilter), Order = 4)]
    [HttpPost]
    public IActionResult CaricamentoMassivoPost(IFormCollection form)
    {
        FileAttachmentViewModel fileAttachmentViewModel = _webHtmlFormToModelMapper.MapItemInsertFromFile(form);

        List<List<SubmittedInputViewModel>> submittedItems =
            _webExcelTemplate.ExcelParse(
                fileAttachmentViewModel
                , _httpContextAccessorCustomWeb.SessionProcessId
                );

        List<List<SubmittedInputLgc>> itemsToSaveRawList = submittedItems.MapListListFromViewModelToLogic();

        ItemSaveResultLgc itemSaveResultLgc = _logicItemManagementSave.SaveNewItems(itemsToSaveRawList);

        OperationResultViewModel modelResult = new();
        if (itemSaveResultLgc.Success)
        {
            modelResult.LocalizedTitle = "Completato";
            modelResult.LocalizedMessage =
                "Importazione conclusa con successo. Vai alla ricerca per caricare eventuali allegati";
            _httpContextAccessorCustomWeb.SessionOperationResult = modelResult;
        }
        else
        {
            modelResult.LocalizedTitle = "Attenzione";
            modelResult.LocalizedMessage =
                "Importazione fallita";
            _httpContextAccessorCustomWeb.SessionOperationResult = modelResult;
        }

        return RedirectToAction(
                MvcComponents.ActStartNewItemsFromFile
                , MvcComponents.CtrlItemManagement
                );
    }


    /// <summary>
    /// save step form data updating item
    /// </summary>
    /// <remarks></remarks>
    /// <param name="form"></param>
    /// <returns></returns>
    [Authorize(Policy = PoliciesKeys.UserIsSupervisor)]
    [ServiceFilter(typeof(RedirectIfAccessSimpleAnonymousFilter), Order = 3)]
    [ServiceFilter(typeof(RequiresManagedIdItemFilter), Order = 4)]
    [HttpPost]
    public IActionResult SalvaPassaggio(IFormCollection form)
    {
        return Json(
            Save(form, isNewItemSave: false)
            );
    }


    /// <summary>
    /// validate and save submitted data. 
    /// Logic here is mainly to handle the numerous cases for result/error display
    /// </summary>
    /// <param name="form"></param>
    /// <param name="isNewItemSave"></param>
    /// <returns></returns>
    [NonAction]
    private SubmitResultJsonModel Save(
        IFormCollection form
        , bool isNewItemSave
        )
    {
        ItemSubmitViewModel itemSubmitViewModel =
            _webHtmlFormToModelMapper.MapItemSubmitFromPostedForm(form);

        ItemSubmitLgc itemSubmit = itemSubmitViewModel.MapFromWebToLogic();

        ItemSaveResultLgc itemSaveResult =
            isNewItemSave
            ? _logicItemManagementSave.SaveNewItem(itemSubmit)
            : _logicItemManagementSave.UpdateCurrentItem(itemSubmit);



        SubmitResultJsonModel outputErrors = new();
        //handle saving errors
        if (!itemSaveResult.Success)
        {
            outputErrors.LoginCodeForAnonymousInsert = string.Empty;

            if (itemSaveResult.FieldsToWarn.HasValues())
            {
                //we must remain in the same page to allow user to edit values in fields
                outputErrors.ResultCode = ResultCode.KoNoReload.ToString();
                outputErrors.ErrorMessage =
                    _webResultMessageMapper.GetMessageForSubmitFormErrors(itemSaveResult.FieldsToWarn);
                return outputErrors;
            }
            else
            {
                //other error messages must be handled by architecture after reload 
                //but if this happens it means that something in logic is wrong or weird attachment was uploaded
                _httpContextAccessorCustomWeb.SessionOperationResult = itemSaveResult.MapItemSaveResult();
                outputErrors.ResultCode = ResultCode.KoWithReload.ToString();
                return outputErrors;
            }
        }

        if (itemSaveResult.IsFirstStep
            && !_httpContextAccessorCustomWeb.SessionHasSingleProcessConfiguration)
        {
            _httpContextAccessorCustomWeb.SessionProcessId = long.MinValue;
            _httpContextAccessorCustomWeb.SessionProcessLogoFileName = string.Empty;
            _contextUser.ProcessId = long.MinValue;
        }

        OperationResultViewModel modelMessage = new();
        ResultCode rcode;
        //handle success
        //different message depending on change type
        //logincode is set in return if returned by save method
        bool error = false;
        switch (itemSubmit.ChangeType)
        {
            case ItemChangeType.SaveNewAndAdvance:
                if (itemSaveResult.IsFirstStep
                    && itemSaveResult.HasMultipleSections)
                {
                    rcode = ResultCode.OkNoReload;
                }
                else
                {
                    rcode = ResultCode.OkToItem;
                    _httpContextAccessorCustomWeb.SessionOperationResult =
                        _webResultMessageMapper.SetSuccessMessageForAdvance(modelMessage);
                }

                _webEmail.SendEmailNotificationChange();
                break;

            case ItemChangeType.UpdateAndPhaseAdvance:
            case ItemChangeType.UpdateAndAlternativeAdvance:

                //advance for first step must stay in same page and show save confirmation from a hidden confirmation
                //this is needed because in anonymous mode, logincode must be shown and a popup can be dismissed by error
                if (itemSaveResult.IsFirstStep
                    && itemSaveResult.HasMultipleSections)
                {
                    rcode = ResultCode.OkNoReload;
                }
                else
                {
                    rcode = ResultCode.OkToItem;
                    _httpContextAccessorCustomWeb.SessionOperationResult =
                        _webResultMessageMapper.SetSuccessMessageForAdvance(modelMessage);
                }
                _webEmail.SendEmailNotificationChange();
                break;


            case ItemChangeType.SaveNewWithoutAdvance:

                //item page will load the correct view
                _httpContextAccessorCustomWeb.SessionIdItemCurrentlyManagedByUser = itemSaveResult.IdItem;
                rcode = ResultCode.OkToItem;
                _httpContextAccessorCustomWeb.SessionOperationResult =
                    _webResultMessageMapper.SetSuccessMessageForSave(modelMessage);
                break;


            case ItemChangeType.UpdateWithoutAdvance:
                rcode = ResultCode.OkWithReload;
                _httpContextAccessorCustomWeb.SessionOperationResult =
                    _webResultMessageMapper.SetSuccessMessageForSave(modelMessage);
                break;

            case ItemChangeType.UpdatePastStep:
                rcode = ResultCode.OkToItem;
                _httpContextAccessorCustomWeb.SessionOperationResult =
                    _webResultMessageMapper.SetSuccessMessageForSave(modelMessage);
                break;

            case ItemChangeType.Abort:
                rcode = ResultCode.OkToMainpage;
                _httpContextAccessorCustomWeb.SessionIdItemCurrentlyManagedByUser = long.MinValue;//reset aborted id item
                _httpContextAccessorCustomWeb.SessionOperationResult =
                    _webResultMessageMapper.SetSuccessMessageForAbort(modelMessage);
                break;

            case ItemChangeType.RollbackToPrevious:
                rcode = ResultCode.OkToItem;
                _httpContextAccessorCustomWeb.SessionOperationResult =
                    _webResultMessageMapper.SetSuccessMessageForRollback(modelMessage);
                break;

            default:
                error = true;
                rcode = ResultCode.Missing;
                break;
        }

        if (error)
        {
            _logger.LogAppError($"unrecognized {nameof(itemSubmit.ChangeType)} '{itemSubmit.ChangeType}' ");
            throw new WebAppException();
        }

        return new SubmitResultJsonModel()
        {
            ResultCode = rcode.ToString(),
            ErrorMessage = string.Empty,
            LoginCodeForAnonymousInsert = itemSaveResult.LoginCodeForAnonymousInsert,
        };
    }
}