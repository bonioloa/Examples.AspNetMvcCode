namespace Examples.AspNetMvcCode.Web.Code;

public class ItemFormSubmitErrorsBuilderWeb : IItemFormSubmitErrorsBuilderWeb
{
    private readonly ILogger<ItemFormSubmitErrorsBuilderWeb> _logger;

    //private readonly MainLocalizer _localizer;


    public ItemFormSubmitErrorsBuilderWeb(
        ILogger<ItemFormSubmitErrorsBuilderWeb> logger
        //, MainLocalizer localizer
        )
    {
        _logger = logger;
        //_localizer = localizer;
    }



    public string BuildErrorMessage(HashSet<ItemFormSubmitErrorModel> itemFormSubmitErrorSet)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(BuildErrorMessage) }
                });



        Guard.Against.Null(itemFormSubmitErrorSet, nameof(itemFormSubmitErrorSet));


        _logger.LogWarning("Building error message...");


        StringBuilder errorMessageBuilder = new();


        foreach (ItemFormSubmitErrorModel itemFormImportError
            in itemFormSubmitErrorSet.OrderBy(itemFormImportError => itemFormImportError.RowLine))
        {
            Guard.Against.Null(itemFormImportError, nameof(itemFormImportError));


            if (itemFormImportError.RowLine >= 0)
            {
                errorMessageBuilder.AppendFormat(
                    "Errori per riga {1}{0}{0}"
                    , Environment.NewLine
                    , itemFormImportError.RowLine
                    );
            }


            errorMessageBuilder.AppendLine("I seguenti campi risultano errati:");
            errorMessageBuilder.AppendLine();


            BuildFieldErrorMessages(errorMessageBuilder, itemFormImportError.FieldErrorList);
        }

        string errorMessageBuilt = errorMessageBuilder.ToString();

        _logger.LogWarning("Error message built {ErrorMessage}", errorMessageBuilt);

        return errorMessageBuilt;
    }


    private static void BuildFieldErrorMessages(StringBuilder errorMessageBuilder, List<IFieldErrorModel> fieldErrorList)
    {
        Guard.Against.Null(fieldErrorList, nameof(fieldErrorList));


        foreach (IFieldErrorModel fieldErrorModel in fieldErrorList)
        {
            Guard.Against.Null(fieldErrorModel, nameof(fieldErrorModel));


            if (BuildMandatoryViolationMessage(errorMessageBuilder, fieldErrorModel))
            {
                continue;
            }


            if (BuildDateErrorMessage(errorMessageBuilder, fieldErrorModel))
            {
                continue;
            }


            if (BuildNumberErrorMessage(errorMessageBuilder, fieldErrorModel))
            {
                continue;
            }


            if (BuildOptionSingleErrorMessage(errorMessageBuilder, fieldErrorModel))
            {
                continue;
            }


            if (BuildOptionMultipleErrorMessage(errorMessageBuilder, fieldErrorModel))
            {
                continue;
            }


            if (BuildAttachmentErrorMessage(errorMessageBuilder, fieldErrorModel))
            {
                continue;
            }


            throw new PmWebException($"unhandled {nameof(IFieldErrorModel)} type: {fieldErrorModel.GetType()} ");
        }
    }






    private static bool BuildMandatoryViolationMessage(StringBuilder errorMessageBuilder, IFieldErrorModel fieldErrorModel)
    {
        if (fieldErrorModel is not FieldErrorMandatoryModel)
        {
            return false;
        }


        FieldErrorMandatoryModel mandatoryError = fieldErrorModel as FieldErrorMandatoryModel;

        errorMessageBuilder.AppendFormat(
            "Il campo{0}{1}{0}non è valorizzato{0}{0}"
            , Environment.NewLine
            , mandatoryError.FieldDescription.GetStringContent()
            );

        return true;
    }


    private static bool BuildDateErrorMessage(StringBuilder errorMessageBuilder, IFieldErrorModel fieldErrorModel)
    {
        if (fieldErrorModel is not FieldErrorDateModel)
        {
            return false;
        }


        FieldErrorDateModel dateError = fieldErrorModel as FieldErrorDateModel;

        errorMessageBuilder.AppendFormat(
            "Il campo{0}{1}{0}ha valore errato{0}{2}{0}Formato richiesto: {3}{0}{0}"
            , Environment.NewLine
            , dateError.FieldDescription.GetStringContent()
            , dateError.WrongValue
            , dateError.RequiredFormat
            );

        return true;
    }


    private static bool BuildNumberErrorMessage(StringBuilder errorMessageBuilder, IFieldErrorModel fieldErrorModel)
    {
        if (fieldErrorModel is not FieldErrorNumberModel)
        {
            return false;
        }


        FieldErrorNumberModel numberError = fieldErrorModel as FieldErrorNumberModel;

        errorMessageBuilder.AppendFormat(
            "Il campo{0}{1}{0}ha valore {2}{0}che non è un numero o non è formattato correttamente{0}Di seguito un esempio di un numero formattato correttamente: {3}{0}{0}"
            , Environment.NewLine
            , numberError.FieldDescription.GetStringContent()
            , numberError.WrongValue
            , numberError.ExampleCorrect
            );

        return true;
    }


    private static bool BuildOptionSingleErrorMessage(StringBuilder errorMessageBuilder, IFieldErrorModel fieldErrorModel)
    {
        if (fieldErrorModel is not FieldErrorOptionSingleModel)
        {
            return false;
        }

        FieldErrorOptionSingleModel optionError = fieldErrorModel as FieldErrorOptionSingleModel;


        errorMessageBuilder.AppendFormat(
            "Il campo{0}{1}{0}prevede una sola scelta{0}Valore inviato{0}{2}{0}{0}"
            , Environment.NewLine
            , optionError.FieldDescription.GetStringContent()
            , string.Join(CodeConstants.Comma, optionError.InvalidOptionList)
            );

        return true;
    }


    private static bool BuildOptionMultipleErrorMessage(StringBuilder errorMessageBuilder, IFieldErrorModel fieldErrorModel)
    {
        if (fieldErrorModel is not FieldErrorOptionMultipleModel)
        {
            return false;
        }


        FieldErrorOptionMultipleModel optionError = fieldErrorModel as FieldErrorOptionMultipleModel;

        errorMessageBuilder.AppendFormat(
            "Il campo{0}{1}{0}contiene opzioni inesistenti fra le disponibili{0}{2}{0}{0}"
            , Environment.NewLine
            , optionError.FieldDescription.GetStringContent()
            , string.Join(CodeConstants.Comma, optionError.InvalidOptionList)
            );

        return true;
    }


    private static bool BuildAttachmentErrorMessage(StringBuilder errorMessageBuilder, IFieldErrorModel fieldErrorModel)
    {
        if (fieldErrorModel is not FieldErrorAttachmentModel)
        {
            return false;
        }


        FieldErrorAttachmentModel attachmentError = fieldErrorModel as FieldErrorAttachmentModel;

        errorMessageBuilder.AppendFormat(
            "Il campo{0}{1}{0}non ha rilevato allegati con i seguenti nomi{0}{2}{0}{0}"
            , Environment.NewLine
            , attachmentError.FieldDescription.GetStringContent()
            , string.Join(AppSeparators.AttachmentNames, attachmentError.MissingDeclaredAttachment)
            );

        return true;
    }

}