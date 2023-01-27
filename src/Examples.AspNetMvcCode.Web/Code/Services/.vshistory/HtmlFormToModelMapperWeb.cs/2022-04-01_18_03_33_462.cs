namespace Comunica.ProcessManager.Web.Code;

public class HtmlFormToModelMapperWeb : IHtmlFormToModelMapperWeb
{
    //https://stackoverflow.com/questions/41367602/upload-files-and-json-in-asp-net-core-web-api/41383852#41383852
    //https://thomaslevesque.com/2018/09/04/handling-multipart-requests-with-json-and-file-uploads-in-asp-net-core/
    //https://blog.bitscry.com/2018/03/12/uploading-files-in-asp-net-core-from-an-mvc-view/
    //https://stackoverflow.com/questions/51021182/httppostedfilebase-in-asp-net-core-2-0/51021836

    private readonly ILogger<HtmlFormToModelMapperWeb> _logger;
    private readonly IFileUploadValidationLogic _logicFileUploadValidation;

    public HtmlFormToModelMapperWeb(
        ILogger<HtmlFormToModelMapperWeb> logger
        , IFileUploadValidationLogic logicFileUploadValidation
        )
    {
        _logger = logger;
        _logicFileUploadValidation = logicFileUploadValidation;
    }


    public ProcessSelectionResultModel MapProcessSelectionFromPostedForm(IFormCollection processSelectionForm)
    {
        _logger.LogAppDebug("CALL");
        _logger.LogAppDebug(processSelectionForm.Serialize());

        ProcessSelectionResultModel model = new();
        if (processSelectionForm is null || processSelectionForm.Keys.IsNullOrEmpty())
        {
            return model;
        }

        bool processFound = false;
        string tmpKey;
        string tmpValue;
        foreach (string key in processSelectionForm.Keys)
        {
            tmpKey = key.Clean();
            //process key must not be handled by switch 
            //because names in configuration vary to allow jquery-validation to work 
            //for separated process radiobuttons groups
            //also: only one key is allowed
            if (tmpKey.StartsWithInvariant(AppConstants.HtmlNameProcess))
            {
                if (processFound)
                {
                    _logger.LogAppError($"found multiple form keys with '{AppConstants.HtmlNameProcess}', current key '{tmpKey}', posted form '{processSelectionForm.Serialize()}' ");
                    throw new WebAppException();
                }
                tmpValue = processSelectionForm[tmpKey];
                tmpValue = tmpValue.Clean();
                processFound = long.TryParse(tmpValue, out long tmpProcessId);
                if (processFound)
                {
                    model.MainId = tmpProcessId;
                }
            }

            tmpValue = processSelectionForm[tmpKey];
            tmpValue = tmpValue.Clean();
            switch (tmpKey)
            {
                case AppConstants.HtmlNameAuto:

                    model.ReportAutoCode = tmpValue;
                    break;

                case AppConstants.HtmlNameChannel:

                    model.ChannelCode = tmpValue;
                    break;

                case AppConstants.HtmlNameGroups:

                    if (string.IsNullOrWhiteSpace(tmpValue)
                        || !int.TryParse(tmpValue, out int logicGroupId))
                    {
                        _logger.LogAppWarning($"value '{tmpValue}' for key '{AppConstants.HtmlNameGroups}' is not numeric");
                        model.LogicGroupId = 0;
                    }
                    else
                    {
                        model.LogicGroupId = logicGroupId;
                    }
                    break;
            }
        }
        return model;
    }

    public ItemSubmitViewModel MapItemSubmitFromPostedForm(IFormCollection htmlForm)
    {
        _logger.LogAppDebug("CALL");

        if (htmlForm is null || htmlForm.Keys.IsNullOrEmpty())
        {
            return null;
        }

        ItemSubmitViewModel itemSubmitViewModel = new();
        IList<SubmittedInputViewModel> fieldsListToSave = new List<SubmittedInputViewModel>();
        bool success;
        string tmpValue;
        string tmpKey;
        //build list of required keys and validate
        //we don't validate values here, this will be delegated to logic methods
        //ATTENTION: any new input field apart from item initial form 
        //must be considered in a specific case in the following switch
        foreach (string key in htmlForm.Keys)
        {
            tmpKey = key.Clean();
            tmpValue = htmlForm[tmpKey];
            tmpValue = tmpValue.Clean();
            switch (tmpKey)
            {
                case ParamsNames.Phase:
                    itemSubmitViewModel.Phase = tmpValue;
                    break;

                case ParamsNames.State:
                    itemSubmitViewModel.State = tmpValue;
                    break;

                case ParamsNames.ItemFormUpdateType:
                    success =
                        Enum.TryParse(tmpValue, out ItemChangeType submittedUpdateType);

                    itemSubmitViewModel.ChangeType =
                        success ? submittedUpdateType : ItemChangeType.Missing;
                    break;

                //do nothing for this case, 
                //these are only technical key/values to be discarded, already handled and validated by architecture
                case ParamsNames.AntiforgeryToken:
                    break;

                //all remaining keys are form dynamic fields
                default:
                    fieldsListToSave.Add(new SubmittedInputViewModel
                    {
                        FieldName = tmpKey,
                        Value = tmpValue
                    });
                    break;
            }
        }

        AddAttachmentsToSubmittedFields(htmlForm.Files, ref fieldsListToSave);

        itemSubmitViewModel.SubmittedInputList = fieldsListToSave;

        return itemSubmitViewModel;
    }

    public ItemUserMessageSubmitViewModel MapItemUserMessageSubmit(IFormCollection htmlForm)
    {
        _logger.LogAppDebug("CALL");

        if (htmlForm is null || htmlForm.Keys.IsNullOrEmpty())
        {
            return null;
        }

        ItemUserMessageSubmitViewModel model = new();
        string tmpValue;
        string tmpKey;
        //build list of required keys and validate
        //we don't validate values here, this will be delegated to logic methods
        //ATTENTION: any new input field apart from item initial form 
        //must be considered in a specific case in the following switch
        foreach (string key in htmlForm.Keys)
        {
            tmpKey = key.Clean();
            tmpValue = htmlForm[tmpKey];
            tmpValue = tmpValue.Clean();
            switch (tmpKey)
            {
                case ParamsNames.Phase:
                    model.Phase = tmpValue;
                    break;

                case ParamsNames.State:
                    model.State = tmpValue;
                    break;

                case ParamsNames.MessageSubject:
                    model.Subject = tmpValue.InputSanitize();//necessary to prevent XSS because subject can be used as raw html in other components
                    break;

                case ParamsNames.MessageText:
                    model.Text = tmpValue;
                    break;

                //do nothing for this case, 
                //this is only technical key/values to be discarded, already handled and validated by architecture
                case ParamsNames.AntiforgeryToken:
                    break;

                //IGNORE all remaining keys
                default:
                    break;
            }
        }

        IList<SubmittedInputViewModel> attachmentsFieldsList = new List<SubmittedInputViewModel>();
        AddAttachmentsToSubmittedFields(htmlForm.Files, ref attachmentsFieldsList);

        //message should only have one field for upload
        if (attachmentsFieldsList.Count > 1)
        {
            _logger.LogAppError($"attached files reference multiple fields '{JsonSerializer.Serialize(attachmentsFieldsList.Select(si => new { si.Description, si.FieldName, si.Value }))}' ");
            throw new WebAppException();
        }

        model.FileAttachmentList =
            attachmentsFieldsList.HasValues()
            ? attachmentsFieldsList.First().Attachments
            : new List<FileAttachmentViewModel>();

        return model;
    }


    /// <summary>
    /// organize attachments in list by name and 
    /// </summary>
    /// <param name="filesCollection"></param>
    /// <param name="submittedInputList">field list retrieved from form</param>
    /// <returns></returns>
    private void AddAttachmentsToSubmittedFields(
        IFormFileCollection filesCollection
        , ref IList<SubmittedInputViewModel> submittedInputList
        )
    {
        if (filesCollection.IsNullOrEmpty())
        {
            return;
        }

        IDictionary<string, IList<FileAttachmentViewModel>> attachmentsByFieldName =
            MapAndOrganize(filesCollection);


        IEnumerable<SubmittedInputViewModel> tmpSubmittedInputEnumer;
        foreach (string fieldName in attachmentsByFieldName.Keys)
        {
            tmpSubmittedInputEnumer =
                submittedInputList.Where(fls =>
                    fls.FieldName.Equals(
                        fieldName
                        , StringComparison.InvariantCultureIgnoreCase
                        ));
            if (tmpSubmittedInputEnumer.HasValues())
            {
                //not add field item, just add attachments for this field
                tmpSubmittedInputEnumer.First().Attachments = attachmentsByFieldName[fieldName];
            }
            else
            {
                //field is missing, create and add to list
                submittedInputList.Add(new SubmittedInputViewModel
                {
                    FieldName = fieldName,
                    Value = string.Join(
                        CodeConstants.SemiColonStr + CodeConstants.SpaceStr
                        , attachmentsByFieldName[fieldName].Select(a => a.Name)
                        ),//at the moment it's not used but it's better than leaving the field empty
                    Attachments = attachmentsByFieldName[fieldName],
                });
            }
        }

        return;
    }


    private FileAttachmentViewModel MapUploadedFile(
        IFormFile singleFile
        , bool singleSubmission
        )
    {
        FileAttachmentLgc attachment =
            _logicFileUploadValidation.ValidateAndMap(singleFile, singleSubmission);

        FileAttachmentViewModel attachmentModel = attachment.MapFromLogicToWeb();

        return attachmentModel;
    }

    /// <summary>
    /// map form attachments to our internal objects and separate the lists by field name
    /// </summary>
    /// <param name="filesCollection"></param>
    /// <returns></returns>
    private IDictionary<string, IList<FileAttachmentViewModel>> MapAndOrganize(
        IFormFileCollection filesCollection
        )
    {
        IList<FileAttachmentViewModel> tmpFileList;
        FileAttachmentViewModel tmpAttachment;
        string tmpFieldName;
        IDictionary<string, IList<FileAttachmentViewModel>> attachmentsByFieldName =
            new Dictionary<string, IList<FileAttachmentViewModel>>();

        foreach (IFormFile file in filesCollection)
        {
            tmpAttachment = MapUploadedFile(file, singleSubmission: false);
            if (tmpAttachment is null)
            {
                continue;
            }
            tmpFieldName = tmpAttachment.FieldNameAssociated;
            if (!attachmentsByFieldName.TryAdd(tmpFieldName, new List<FileAttachmentViewModel>() { tmpAttachment }))
            {
                tmpFileList = attachmentsByFieldName[tmpFieldName];
                tmpFileList.Add(tmpAttachment);
                attachmentsByFieldName[tmpFieldName] = tmpFileList;
            }
        }

        return attachmentsByFieldName;
    }


    public RoleInclusionViewModel MapRoleInclusion(IFormCollection inclusionForm)
    {
        _logger.LogAppDebug("CALL");
        _logger.LogAppDebug(inclusionForm.Serialize());

        RoleInclusionViewModel model = new();
        if (inclusionForm is null || inclusionForm.Keys.IsNullOrEmpty())
        {
            return model;
        }

        string tmpKey;
        foreach (string key in inclusionForm.Keys)
        {
            tmpKey = key.Clean();
            StringValues currentKeyValue = inclusionForm[tmpKey];
            if (currentKeyValue.IsNullOrEmpty())
            {
                continue;
            }
            switch (tmpKey)
            {
                case WebAppConstants.HtmlNameRoleGroupIdForPermissionAssignment:

                    if (currentKeyValue.Count == 1
                        && long.TryParse(currentKeyValue.ToString().Clean(), out long permissionCode))
                    {
                        model.RoleGroupIdForAssignment = permissionCode;
                    }
                    break;


                case WebAppConstants.HtmlNameAssignPermission:

                    model.RolesToInclude = new List<string>();
                    foreach (string role in currentKeyValue)
                    {
                        if (role.StringHasValue())
                        {
                            model.RolesToInclude.Add(role.Clean());
                        }
                    }
                    break;
            }
        }
        return model;
    }


    public SsoLoginModel MapPostSsoStateData(IFormCollection postSsoStateForm)
    {
        _logger.LogAppDebug("CALL");

        if (postSsoStateForm.IsNullOrEmpty())
        {
            _logger.LogAppError("no data submitted");
            throw new WebAppException();
        }

        //we assume RelayState never encrypted.
        //we need to pass this information because we need to dynamically retrieve saml configuration from db
        //so we can't use the binding.GetRelayStateQuery 
        string ssoRelayState = postSsoStateForm[ParamsNames.RelayState];

        if (ssoRelayState.Empty())
        {
            _logger.LogAppError($"{ParamsNames.RelayState} empty. Check with Idp why or use POST binding");
            throw new WebAppException();
        }
        _logger.LogAppDebug($"SSO POST received {ParamsNames.RelayState} '{ssoRelayState}' ");


        //write other form keys sent by IdP
        try
        {
            Dictionary<string, string> Dictionary = new();
            StringBuilder s = new();
            s.AppendLine();
            foreach (KeyValuePair<string, StringValues> item in postSsoStateForm)
            {
                if (item.Key == ParamsNames.RelayState)
                {
                    continue;
                }
                s.AppendFormat("key {0} - value {1}", item.Key, item.Value);
                s.AppendLine();
            }
            _logger.LogAppDebug($"remaining SSO form keys-values: '{s}' ");
        }
        catch (Exception ex)
        {
            _logger.LogAppError($"error deserializing SSO form: '{ ex.GetType()}' - '{ex.Message}' ");
        }

        string tmpToken = string.Empty;
        bool implicitSsoConfigId = false;
        long ssoConfigId = long.MinValue;

        NameValueCollection queryStringInRelayState = HttpUtility.ParseQueryString(ssoRelayState);
        if (queryStringInRelayState is null)
        {
            //this should never happen
            _logger.LogAppError($"{nameof(ParamsNames.RelayState)} querystring parse returned null ");
            throw new WebAppException();
        }
        if (!queryStringInRelayState.HasKeys())
        {
            //some customers pass directly token value, not a querystring, so we handle the case here
            //No keys found but value is retrievable (weird behaviour of NameValueCollection)
            if (queryStringInRelayState.Count > 0)
            {
                tmpToken = queryStringInRelayState.ToString();
                implicitSsoConfigId = true;
            }
            else
            {
                _logger.LogAppError($"{nameof(ParamsNames.RelayState)} is empty or does not contain a query string '{ssoRelayState}' ");
                throw new WebAppException();
            }
        }
        if (!implicitSsoConfigId
            && queryStringInRelayState[ParamsNames.TenantToken].Empty())
        {
            _logger.LogAppError($"{nameof(ParamsNames.RelayState)} query string missing {ParamsNames.TenantToken}  ");
            throw new WebAppException();
        }
        if (!implicitSsoConfigId
            && (queryStringInRelayState[ParamsNames.SsoConfigId].Empty()
                || !long.TryParse(queryStringInRelayState[ParamsNames.SsoConfigId], out ssoConfigId)
                ))
        {
            _logger.LogAppError($"{nameof(ParamsNames.RelayState)} query string missing {ParamsNames.SsoConfigId}  ");
            throw new WebAppException();
        }

        //culture is not needed in context we just save value to model and will be set after logging in
        string languageIso = queryStringInRelayState[RouteParams.Language];
        languageIso =
            languageIso.StringHasValue()
            ? languageIso
            : AppConstants.CultureDefaultIsoCode;

        //get token from querystring only if validations above successfully parsed RelayState value as querystring
        string token =
            tmpToken.Empty()
            ? queryStringInRelayState[ParamsNames.TenantToken].Clean()
            : tmpToken;

        //remaining model properties mapped by calling methods
        return new SsoLoginModel
        {
            LanguageIso = languageIso,
            Token = token,
            SsoConfigId = ssoConfigId,
        };
    }

    public FileAttachmentViewModel MapItemInsertFromFile(IFormCollection formFileUpload)
    {
        _logger.LogAppDebug("CALL");

        if (formFileUpload.IsNullOrEmpty())
        {
            _logger.LogAppError("no data submitted");
            throw new WebAppException();
        }

        if (formFileUpload.Files.IsNullOrEmpty())
        {
            _logger.LogAppError("no files submitted");
            throw new WebAppException();
        }

        if (formFileUpload.Files.Count > 1)
        {
            _logger.LogAppError("only single files upload allowed");
            throw new WebAppException();
        }

        return MapUploadedFile(formFileUpload.Files[0], singleSubmission: true);
    }


    public void MapGridViewSave(IFormCollection formGridViewSave)
    {
        _logger.LogAppDebug("CALL");

        if (formGridViewSave.IsNullOrEmpty())
        {
            _logger.LogAppError("no data submitted");
            throw new WebAppException();
        }
    }
}