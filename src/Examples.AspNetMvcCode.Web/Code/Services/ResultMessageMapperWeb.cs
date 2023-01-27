namespace Examples.AspNetMvcCode.Web.Code;

public class ResultMessageMapperWeb : IResultMessageMapperWeb
{
    private readonly ILogger<ResultMessageMapperWeb> _logger;

    private readonly IMainLocalizer _localizer;


    public ResultMessageMapperWeb(
        ILogger<ResultMessageMapperWeb> logger
        , IMainLocalizer localizer
        )
    {
        _logger = logger;
        _localizer = localizer;
    }



    //NOTE: message must be set only if title or message are DIFFERENT from standard one

    public OperationResultViewModel SetMessageDataForIdentityDisclosureResult(OperationResultViewModel modelMessage)
    {
        Guard.Against.Null(modelMessage, nameof(modelMessage));

        modelMessage.LocalizedMessage =
            modelMessage.Success
            ? _localizer[nameof(LocalizedStr.IdentityDisclosureSubmitSuccess)]
            : _localizer[nameof(LocalizedStr.IdentityDisclosureSubmitError)];

        return modelMessage;
    }


    public OperationResultViewModel SetResultForItemUserMessageSubmit(OperationResultViewModel modelMessage)
    {
        Guard.Against.Null(modelMessage, nameof(modelMessage));

        modelMessage.LocalizedMessage =
            modelMessage.Success
                ? _localizer[nameof(LocalizedStr.ItemUserMessageSubmitSuccess)]
                : _localizer[nameof(LocalizedStr.ItemUserMessageSubmitError)];

        return modelMessage;
    }





    public OperationResultViewModel SetSuccessMessageForAdvance()
    {
        return
            new OperationResultViewModel
            {
                LocalizedTitle =
                   _localizer[nameof(LocalizedStr.ItemManagementSubmitResultSuccessTitleAdvance)],

                LocalizedMessage =
                _localizer[nameof(LocalizedStr.ItemManagementSubmitResultSuccessMessage)]
            };
    }



    public OperationResultViewModel SetSuccessMessageForSave()
    {
        return
            new OperationResultViewModel
            {
                LocalizedTitle =
                    _localizer[nameof(LocalizedStr.ItemManagementSubmitResultSuccessTitleSave)],

                LocalizedMessage =
                    _localizer[nameof(LocalizedStr.ItemManagementSubmitResultSuccessMessage)],
            };
    }



    public OperationResultViewModel SetSuccessMessageForAbort()
    {
        return
            new OperationResultViewModel
            {
                LocalizedTitle =
                    _localizer[nameof(LocalizedStr.ItemManagementSubmitResultSuccessTitleAbort)],

                LocalizedMessage =
                    _localizer[nameof(LocalizedStr.ItemManagementSubmitResultSuccessMessage)],
            };
    }


    public OperationResultViewModel SetSuccessMessageForRollback()
    {
        return
            new OperationResultViewModel
            {
                LocalizedTitle =
                    _localizer[nameof(LocalizedStr.ItemManagementSubmitResultSuccessTitleRollback)],

                LocalizedMessage =
                    _localizer[nameof(LocalizedStr.ItemManagementSubmitResultSuccessMessage)],
            };
    }



    public OperationResultViewModel SetRoleInclusionMessage(
        OperationResultViewModel modelMessage
        , IList<OptionViewModel> includedRoles
        )
    {
        Guard.Against.Null(modelMessage, nameof(modelMessage));

        //message here is set only on success, for failure object has all the needed data for display
        if (modelMessage.Success)
        {
            Guard.Against.Null(includedRoles, nameof(includedRoles));

            StringBuilder builder = new();
            builder.AppendLine(_localizer[nameof(LocalizedStr.AssignPermissionsSuccessMessage)]);
            builder.AppendLine();

            foreach (OptionViewModel role in includedRoles)
            {
                builder.Append(CodeConstants.Dash);
                builder.Append(CodeConstants.Space);
                builder.Append(role.Description.GetStringContent());
                builder.AppendLine();
            }

            modelMessage.LocalizedMessage = builder.ToString();
        }

        return modelMessage;
    }



    public OperationResultViewModel SetRegistrationResultMessage(OperationResultViewModel modelMessage)
    {
        Guard.Against.Null(modelMessage, nameof(modelMessage));

        modelMessage.LocalizedMessage =
            modelMessage.Success
            ? _localizer[nameof(LocalizedStr.UserRegistrationResultSuccessMessage)]
            : _localizer[nameof(LocalizedStr.UserRegistrationResultErrorMessage)];

        return modelMessage;
    }


    public OperationResultViewModel SetValidateRegistrationResultMessage(OperationResultViewModel modelMessage)
    {
        Guard.Against.Null(modelMessage, nameof(modelMessage));

        modelMessage.LocalizedMessage =
           modelMessage.Success
           ? _localizer[nameof(LocalizedStr.UserValidateRegistrationSuccessMessage)]
           : _localizer[nameof(LocalizedStr.UserValidateRegistrationErrorMessage)];

        return modelMessage;
    }


    public AppWarningViewModel GetLocalized(OperationResultViewModel inputOperation)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetLocalized) }
                });
        string valuesAllowedName = nameof(inputOperation.ValuesAllowed);
        string fieldToWarnListName = nameof(inputOperation.FieldToWarnList);

        _logger.LogDebug("CALL");



        Guard.Against.Null(inputOperation, nameof(inputOperation));

        if (inputOperation.LocalizedTitle.Empty())
        {
            inputOperation.LocalizedTitle =
                inputOperation.Success
                ? GetStandardSuccessTitle()
                : _localizer[nameof(LocalizedStr.SharedWarningMessageGenericTitle)];
        }


        if (inputOperation.LocalizedMessage.Empty())
        {
            inputOperation.LocalizedMessage =
                _localizer[nameof(LocalizedStr.SharedWarningMessageGenericText)];
        }


        StringBuilder messageBuilder = new();

        messageBuilder.Append(inputOperation.LocalizedMessage);
        messageBuilder.AppendLine();

        switch (inputOperation.WarningType)
        {
            case WarningType.InvalidOrEmpty:

                if (inputOperation.FieldToWarnList.IsNullOrEmpty())//in this case this should not be a problem, but better to log it anyway
                {
                    _logger.LogInformation(
                        "list {FieldToWarnListName)} is empty"
                        , fieldToWarnListName
                        );
                }
                else
                {
                    /*i seguenti campi obbligatori sono vuoti o non validi*/
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageInvalid)]);
                    messageBuilder.AppendLine();

                    IList<string> fieldsList =
                        inputOperation.FieldToWarnList
                            .Select(f => GetLocalizedName(f))
                            .ToList();

                    messageBuilder.Append(string.Join(AppSeparators.Statement, fieldsList));
                }
                break;


            case WarningType.InsufficientPermissions:

                messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageInsufficientPermissions)]);
                break;


            case WarningType.TenantAccessDenied:

                messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageAccessUnauthorized)]);
                messageBuilder.Append(CodeConstants.Colon);
                messageBuilder.Append(CodeConstants.Space);

                if (inputOperation.FieldToWarnList.HasValues())
                {
                    switch (inputOperation.FieldToWarnList.First())
                    {
                        case MessageField.ClientIp:
                            messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageIPUnauthorized)]);
                            break;
                    }
                }
                break;


            case WarningType.PageNeedsRefresh:

                messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageRefresh)]);
                break;


            case WarningType.ValueAllowed:

                //we must prevent eventual wrongdoing in error composition
                if (inputOperation.ValuesAllowed.Empty()
                    || inputOperation.FieldToWarnList.IsNullOrEmpty()
                    || inputOperation.FieldToWarnList.Count != 1)
                {
                    if (inputOperation.ValuesAllowed.Empty())
                    {
                        _logger.LogError(
                            "{ValuesAllowedName} is mandatory"
                            , valuesAllowedName
                            );
                    }
                    else
                    {
                        _logger.LogError(
                            "{FieldToWarnListName} is mandatory and must contain only one element"
                            , fieldToWarnListName
                            );
                    }
                }
                else
                {
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageAllowedValues)]);
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageField)]);
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[0]));
                    messageBuilder.Append(CodeConstants.Colon);
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(inputOperation.ValuesAllowed);
                }
                break;


            case WarningType.AlreadyInUse:

                if (inputOperation.FieldToWarnList.IsNullOrEmpty()
                   || inputOperation.FieldToWarnList.Count != 1)
                {
                    _logger.LogError(
                        "{FieldToWarnListName} is mandatory and must contain only one element"
                        , fieldToWarnListName
                        );
                }
                else
                {
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageField)]);
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[0]));
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageAlreadyInUse)]);
                }
                break;


            case WarningType.AlreadyActivated:

                if (inputOperation.FieldToWarnList.IsNullOrEmpty()
                   || inputOperation.FieldToWarnList.Count != 1)
                {
                    _logger.LogError(
                        "{FieldToWarnListName} is mandatory and must contain 1 element"
                        , fieldToWarnListName
                        );
                }
                else
                {
                    messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[0]));
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageAlreadyActivated)]);
                }
                break;


            case WarningType.NotFound:

                if (inputOperation.FieldToWarnList.IsNullOrEmpty()
                   || inputOperation.FieldToWarnList.Count != 1)
                {
                    _logger.LogError(
                        "{FieldToWarnListName} is mandatory and must contain 1 element"
                        , fieldToWarnListName
                        );
                }
                else
                {
                    messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[0]));
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageNotFound)]);
                }
                break;


            case WarningType.IsEqual:

                if (inputOperation.FieldToWarnList.IsNullOrEmpty()
                   || inputOperation.FieldToWarnList.Count != 2)
                {
                    _logger.LogError(
                        "{FieldToWarnListName} is mandatory and must contain 2 element"
                        , fieldToWarnListName
                        );
                }
                else
                {
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageField)]);
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[0]));
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageNotEqual)]);//must not be equal to
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageField)]);
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[1]));
                }
                break;


            case WarningType.Mismatch:

                if (inputOperation.FieldToWarnList.IsNullOrEmpty()
                   || inputOperation.FieldToWarnList.Count != 2)
                {
                    _logger.LogError(
                        "{FieldToWarnListName} is mandatory and must contain 2 element"
                        , fieldToWarnListName
                        );
                }
                else
                {
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageField)]);
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[0]));
                    messageBuilder.Append(CodeConstants.Space);
                    messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageDifferent)]);//is different
                    messageBuilder.Append(CodeConstants.Space);
                    //messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageField)]); //don't prefix with "field", could be misleading
                    //messageBuilder.Append(AppConstants.Space);
                    messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[1]));
                }
                break;


            case WarningType.Expired:

                messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[0]));
                messageBuilder.Append(CodeConstants.Space);
                messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageExpired)]);
                break;


            case WarningType.Incompatibility:

                messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageFilter)]);
                messageBuilder.Append(CodeConstants.Space);
                messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[0]));
                messageBuilder.Append(CodeConstants.Space);
                messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageIncompatibleWith)]);
                messageBuilder.Append(CodeConstants.Space);
                messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageFilter)]);
                messageBuilder.Append(CodeConstants.Space);
                messageBuilder.Append(GetLocalizedName(inputOperation.FieldToWarnList[1]));
                break;


            case WarningType.MissingRolesInclusion:

                messageBuilder.Append(_localizer[nameof(LocalizedStr.SharedWarningMessageMustIncludeRole)]);
                break;


            //do nothing cases
            case WarningType.None:
            case WarningType.WrongSchedulingIp:
            case WarningType.FileNameEmpy:
            default:
                break;
        }

        string newLinePlaceholder = "#newline#";

        string formattedMessage =
            messageBuilder.ToString()
                          .CleanReplaceHtmlNewLines(newLinePlaceholder)
                          .CleanReplaceTextNewLines(newLinePlaceholder)
                          .CleanReplaceTextTabs(CodeConstants.SpaceStr)
                          .CleanReplaceHtmlNonBreakableSpaces(CodeConstants.SpaceStr);

        return
            new AppWarningViewModel(
                Title: inputOperation.LocalizedTitle

                , MessageLines:
                        formattedMessage
                            .Split(
                                newLinePlaceholder
                                , StringSplitOptions.None
                                )//don't remove empty spaces from messages
                            .ToList()
                );
    }

    private string GetLocalizedName(MessageField field)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetLocalizedName) }
                });



        string fieldMessage;

        switch (field)
        {
            case MessageField.TenantToken:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldTenantToken)];
                break;

            case MessageField.UserLogin:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldUserLogin)];
                break;

            case MessageField.Password:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldPassword)];
                break;

            case MessageField.OldPassword:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldOldPassword)];
                break;

            case MessageField.NewPassword:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldNewPassword)];
                break;

            case MessageField.ConfirmPassword:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldConfirmPassword)];
                break;

            case MessageField.CurrentPassword:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldCurrentPassword)];
                break;

            case MessageField.LoginCode:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldLoginCode)];
                break;

            case MessageField.Credentials:
                fieldMessage =
                    _localizer[nameof(LocalizedStr.SharedFieldUserLogin)]
                    + CodeConstants.Comma + CodeConstants.Space
                    + _localizer[nameof(LocalizedStr.SharedFieldPassword)];
                break;

            case MessageField.Email:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldEmail)];
                break;

            case MessageField.EmailDomain:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldEmailDomain)];
                break;

            case MessageField.UserName:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldName)];
                break;

            case MessageField.UserSurname:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldSurname)];
                break;

            case MessageField.Roles:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedRoleProfiles)];
                break;

            case MessageField.ValidationCode:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldValidationCode)];
                break;

            case MessageField.RecoverInputs:
                fieldMessage =
                    _localizer[nameof(LocalizedStr.SharedFieldUserLogin)]
                    + CodeConstants.Comma + CodeConstants.Space
                    + _localizer[nameof(LocalizedStr.SharedFieldEmail)]
                    ;
                break;

            case MessageField.MessageSubject:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldMessageSubject)];
                break;

            case MessageField.MessageText:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldMessageText)];
                break;

            case MessageField.StrongAuthenticationCode:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldUserLogin2faCode)];
                break;

            case MessageField.SearchItemProcessFilter:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldLabelProcess)];
                break;

            case MessageField.SearchItemStepFilter:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldSearchFilterStep)];
                break;

            case MessageField.DateSubmitFromItemFilter:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldLabelDateSubmitFrom)];
                break;

            case MessageField.DateSubmitToItemFilter:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldLabelDateSubmitTo)];
                break;

            case MessageField.ItemDateExpirationFromFilter:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldLabelDateExpirationFrom)];
                break;

            case MessageField.ItemDateExpirationToItemFilter:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldLabelDateExpirationTo)];
                break;

            case MessageField.PermissionRoles:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldAssignPermissionRole)];
                break;

            case MessageField.Attachment:
                fieldMessage = _localizer[nameof(LocalizedStr.SharedFieldAttachment)];
                break;

            case MessageField.ItemCode:
                fieldMessage = _localizer[nameof(LocalizedStr.ProblemReportItemCode)];
                break;

            case MessageField.ProblemDescription:
                fieldMessage = _localizer[nameof(LocalizedStr.ProblemReportDescription)];
                break;
            //case MessageField.:
            //    fieldMessage = _localizer[nameof(LocalizedStr.)];
            //    break;


            case MessageField.GenericField:
            case MessageField.GenericFields:

                string genericFieldName = nameof(MessageField.GenericField);
                string genericFieldsName = nameof(MessageField.GenericFields);

                _logger.LogError(
                    "{GenericFieldName} and {GenericFieldsName} should not be handled here"
                    , genericFieldName
                    , genericFieldsName
                    );

                fieldMessage = string.Empty;
                break;


            default:
                _logger.LogError(
                    "{Field} not handled by localization"
                    , field
                    );

                fieldMessage = string.Empty;
                break;
        }

        return fieldMessage;
    }

    private string GetStandardSuccessTitle()
    {
        return _localizer[nameof(LocalizedStr.SharedWarningTitleSuccessGeneric)];
    }
}