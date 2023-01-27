namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 1)]
public class ElementiSalvaController : BaseContextController
{
    private readonly ContextUser _contextUser;
    private readonly IUrlHelper _urlHelper;

    private readonly IItemSavingNewLogic _logicItemSavingNew;
    private readonly IItemSavingEditLogic _logicItemSavingEdit;
    private readonly IEmailSendAppNotificationLogic _logicEmailSendAppNotification;
    private readonly IExcelTemplateParseForItemsInsertMassiveLogic _logicExcelTemplateParseForItemsInsertMassive;

    private readonly IHtmlFormToModelMapperWeb _webHtmlFormToModelMapper;
    private readonly IResultMessageMapperWeb _webResultMessageMapper;
    private readonly IItemFormSubmitErrorsBuilderWeb _webItemFormSubmitErrorsBuilder;

    public ElementiSalvaController(
        ContextUser contextUser
        , IUrlHelper urlHelper
        , IItemSavingNewLogic logicItemSavingNew
        , IItemSavingEditLogic logicItemSavingEdit
        , IEmailSendAppNotificationLogic logicEmailSendAppNotification
        , IExcelTemplateParseForItemsInsertMassiveLogic logicExcelTemplateParseForItemsInsertMassive
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IHtmlFormToModelMapperWeb webHtmlFormToModelMapper
        , IResultMessageMapperWeb webResultMessageMapper
        , IItemFormSubmitErrorsBuilderWeb webItemFormSubmitErrorsBuilder
        ) : base(webHttpContextAccessor)
    {
        _contextUser = contextUser;
        _urlHelper = urlHelper;
        _logicItemSavingNew = logicItemSavingNew;
        _logicItemSavingEdit = logicItemSavingEdit;
        _logicEmailSendAppNotification = logicEmailSendAppNotification;
        _logicExcelTemplateParseForItemsInsertMassive = logicExcelTemplateParseForItemsInsertMassive;
        _webHtmlFormToModelMapper = webHtmlFormToModelMapper;
        _webResultMessageMapper = webResultMessageMapper;
        _webItemFormSubmitErrorsBuilder = webItemFormSubmitErrorsBuilder;
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
        return
            Json(
                Save(form, isNewItemSave: true)
                );
    }



    [ServiceFilter(typeof(RedirectIfAccessWithLoginCodeFilter), Order = 1)]
    [ServiceFilter(typeof(RequiresProcessIdFilter), Order = 4)]
    [HttpPost]
    public IActionResult CaricamentoMassivoPost(IFormCollection form)
    {
        ItemDataInsertFromFileModel itemsInsertFromFileModel =
            _webHtmlFormToModelMapper.MapItemDataInsertFromFile(form);


        //for this action we will always redirect to the same page, eventually showing a message modal
        IActionResult returnToPageAction =
            RedirectToAction(
                   MvcComponents.ActStartNewItemsFromFile
                   , MvcComponents.CtrlItemManagement
                   );



        IEnumerable<FileAttachmentViewModel> itemInsertDataFileFound =
            itemsInsertFromFileModel.UploadedFilesFound
                .Where(file => file.Name.StartsWithInvariant(LocFilenames.InsertMassiveTemplatePrefix));

        if (itemInsertDataFileFound.IsNullOrEmpty())
        {
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel
                {
                    LocalizedMessage =
                        $@"Nei file caricati non è stato trovato nessun documento con prefisso '{LocFilenames.InsertMassiveTemplatePrefix}'; 
                            il documento con i dati da caricare DEVE avere questo prefisso. 
                            Tieni presente che le maiuscole e le minuscole sono considerati caratteri differenti per questo prefisso"
                };

            return returnToPageAction;
        }

        if (itemInsertDataFileFound.Count() > 1)
        {
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel
                {
                    LocalizedMessage =
                        $@"Nei file caricati sono stati trovati documenti multipli con prefisso '{LocFilenames.InsertMassiveTemplatePrefix}'; 
                            il documento con i dati da caricare DEVE essere uno e uno solo. 
                            Tieni presente che le maiuscole e le minuscole sono considerati caratteri differenti per questo prefisso"
                };

            return returnToPageAction;
        }

        List<List<SubmittedInputLgc>> submittedItems =
            _logicExcelTemplateParseForItemsInsertMassive.ExcelParse(
                itemInsertDataFileFound.Single()
                                       .MapFromWebToLogic()
                , _webHttpContextAccessor.SessionProcessId
                );

        IEnumerable<FileAttachmentLgc> attachmentsWithoutInsertDocumentFound =
            itemsInsertFromFileModel.UploadedFilesFound
                    .Where(file => !file.Name.StartsWithInvariant(LocFilenames.InsertMassiveTemplatePrefix))
                    .MapIEnumerableFromWebToLogic();


        ItemSaveResultLgc itemSaveResult =
            _logicItemSavingNew.SaveNewItems(
                submittedItems
                , attachmentsWithoutInsertDocumentFound
                , itemsInsertFromFileModel.SaveInDraftState
                );



        if (itemSaveResult.ItemFormSubmitErrorSet.HasValues())
        {
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel
                {
                    LocalizedMessage =
                        _webItemFormSubmitErrorsBuilder.BuildErrorMessage(
                            itemSaveResult.ItemFormSubmitErrorSet.MapHashSetFromLogicToWeb()
                            )
                };

            return returnToPageAction;
        }


        if (itemSaveResult.WarningType != WarningType.None)
        {
            _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel
                {
                    WarningType = itemSaveResult.WarningType,
                };

            return returnToPageAction;
        }


        _webHttpContextAccessor.SessionOperationResult =
                new OperationResultViewModel
                {
                    LocalizedTitle = "Completato",
                    LocalizedMessage = "Importazione conclusa con successo",
                };

        return returnToPageAction;
    }


    /// <summary>
    /// save step form data updating item
    /// </summary>
    /// <remarks></remarks>
    /// <param name="form"></param>
    /// <returns></returns>
    [Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
    [ServiceFilter(typeof(RedirectIfAccessSimpleAnonymousFilter), Order = 3)]
    [ServiceFilter(typeof(RequiresManagedItemIdFilter), Order = 4)]
    [HttpPost]
    public IActionResult SalvaPassaggio(IFormCollection form)
    {
        return
            Json(
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
    private ItemSaveResultJsonModel Save(
        IFormCollection form
        , bool isNewItemSave
        )
    {
        ItemSubmitViewModel itemSubmitViewModel =
            _webHtmlFormToModelMapper.MapItemSubmitFromPostedForm(form);

        ItemSubmitLgc itemSubmit = itemSubmitViewModel.MapFromWebToLogic();

        ItemSaveResultLgc itemSaveResult;
        if (isNewItemSave)
        {
            itemSaveResult = _logicItemSavingNew.SaveNewItem(itemSubmit);
        }
        else
        {
            ItemSaveDataForUpdateLgc itemSaveDataForUpdate =
                _logicItemSavingEdit.GetDataForUpdateCurrentItem(
                    itemSubmit
                    , updateDateTime: DateTime.Now
                    );
            itemSaveResult = _logicItemSavingEdit.UpdateItem(itemSaveDataForUpdate);
        }


        if (itemSaveResult.ItemFormSubmitErrorSet.HasValues())
        {
            //we must remain in the same page to allow user to
            //know which fields are invalid and make the necessary corrections

            return
                new ItemSaveResultJsonModel
                {
                    LoginCodeForAnonymousInsert = string.Empty,
                    ResultCode = ItemSaveResultCode.KoNoReload.ToString(),
                    ErrorMessage =
                        _webItemFormSubmitErrorsBuilder.BuildErrorMessage(
                            itemSaveResult.ItemFormSubmitErrorSet.MapHashSetFromLogicToWeb()
                            ),
                };

        }


        if (itemSaveResult.WarningType != WarningType.None)
        {
            AppWarningViewModel appWarningViewModel =
                _webResultMessageMapper.GetLocalized(
                    new OperationResultViewModel
                    {
                        WarningType = itemSaveResult.WarningType,
                    });

            return
                new ItemSaveResultJsonModel
                {
                    LoginCodeForAnonymousInsert = string.Empty,
                    ResultCode = ItemSaveResultCode.KoNoReload.ToString(),
                    ErrorMessage = string.Join(Environment.NewLine, appWarningViewModel.MessageLines),
                };
        }


        if (itemSaveResult.IsFirstStep
            && !_webHttpContextAccessor.SessionHasSingleProcessConfiguration)
        {
            _webHttpContextAccessor.SessionProcessId = long.MinValue;
            _webHttpContextAccessor.SessionProcessLogoFileName = string.Empty;
            _contextUser.ProcessId = long.MinValue;
        }


        Uri itemUrl = _urlHelper.AbsoluteActionItemManagement(_contextUser.ItemIdCurrentlyManagedByUser);

        ItemSaveResultCode rcode;
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
                    rcode = ItemSaveResultCode.OkNoReload;
                }
                else
                {
                    if (_webHttpContextAccessor.SessionItemIdCurrentlyManagedByUser.Invalid())
                    {
                        //necessary for new item added with a form using page template without stepper,
                        //because this template does not have a html section to show insert confirmation
                        _webHttpContextAccessor.SessionItemIdCurrentlyManagedByUser = itemSaveResult.ItemId;

                        _webHttpContextAccessor.SessionOperationResult =
                            _webResultMessageMapper.SetSuccessMessageForSave();
                    }
                    else
                    {
                        _webHttpContextAccessor.SessionOperationResult =
                            _webResultMessageMapper.SetSuccessMessageForAdvance();
                    }

                    rcode = ItemSaveResultCode.OkToItem;
                }

                _logicEmailSendAppNotification.SendEmailNotificationChange(itemUrl);
                break;

            case ItemChangeType.UpdateAndPhaseAdvance:
            case ItemChangeType.UpdateAndAlternativeAdvance:

                //advance for first step must stay in same page and show save confirmation from a hidden confirmation
                //this is needed because in anonymous mode, logincode must be shown and a popup can be dismissed by error
                if (itemSaveResult.IsFirstStep
                    && itemSaveResult.HasMultipleSections)
                {
                    rcode = ItemSaveResultCode.OkNoReload;
                }
                else
                {
                    rcode = ItemSaveResultCode.OkToItem;

                    _webHttpContextAccessor.SessionOperationResult =
                        _webResultMessageMapper.SetSuccessMessageForAdvance();
                }

                _logicEmailSendAppNotification.SendEmailNotificationChange(itemUrl);

                break;


            case ItemChangeType.SaveNewWithoutAdvance:

                //item page will load the correct view
                _webHttpContextAccessor.SessionItemIdCurrentlyManagedByUser = itemSaveResult.ItemId;

                rcode = ItemSaveResultCode.OkToItem;

                _webHttpContextAccessor.SessionOperationResult =
                    _webResultMessageMapper.SetSuccessMessageForSave();
                break;


            case ItemChangeType.UpdateWithoutAdvance:

                rcode = ItemSaveResultCode.OkWithReload;

                _webHttpContextAccessor.SessionOperationResult =
                    _webResultMessageMapper.SetSuccessMessageForSave();
                break;


            case ItemChangeType.UpdatePastStep:

                rcode = ItemSaveResultCode.OkToItem;

                _webHttpContextAccessor.SessionOperationResult =
                    _webResultMessageMapper.SetSuccessMessageForSave();
                break;


            case ItemChangeType.Abort:

                rcode = ItemSaveResultCode.OkToMainpage;

                _webHttpContextAccessor.SessionItemIdCurrentlyManagedByUser = long.MinValue;//reset aborted id item

                _webHttpContextAccessor.SessionOperationResult =
                    _webResultMessageMapper.SetSuccessMessageForAbort();

                break;


            case ItemChangeType.RollbackToPrevious:

                rcode = ItemSaveResultCode.OkToItem;

                _webHttpContextAccessor.SessionOperationResult =
                    _webResultMessageMapper.SetSuccessMessageForRollback();

                _logicEmailSendAppNotification.SendEmailNotificationChange(itemUrl);
                break;


            default:
                error = true;
                rcode = ItemSaveResultCode.Missing;
                break;
        }


        if (error)
        {
            throw new PmWebException($"unrecognized {nameof(itemSubmit.ChangeType)} '{itemSubmit.ChangeType}' ");
        }


        return
            new ItemSaveResultJsonModel()
            {
                ResultCode = rcode.ToString(),
                ErrorMessage = string.Empty,
                LoginCodeForAnonymousInsert = itemSaveResult.LoginCodeForAnonymousInsert,
            };
    }
}