namespace Examples.AspNetMvcCode.Common;

public static class FieldTypeExtensions
{
    private static readonly List<FieldType> TitleTypes =
        new()
        {
            FieldType.LabelTitle
            , FieldType.LabelTitleSmaller
        };

    public static bool IsTitle(this FieldType fieldType)
    {
        return
            TitleTypes.Contains(fieldType);
    }
    public static bool IsTitleSection(this FieldType fieldType)
    {
        return fieldType == FieldType.LabelTitle;
    }



    private static readonly IList<FieldType> OptionsTypes =
        new List<FieldType>
        {
            FieldType.OptionsCheckBox
            , FieldType.OptionsRadio
            , FieldType.OptionsSelect
            , FieldType.OptionsSelectMultiple
        };

    public static bool IsOption(this FieldType fieldType)
    {
        return OptionsTypes.Contains(fieldType);
    }


    private static readonly IList<FieldType> OptionsMultipleChoicesTypes =
        new List<FieldType>
        {
            FieldType.OptionsCheckBox
            , FieldType.OptionsSelectMultiple
        };
    public static bool IsOptionMultipleChoices(this FieldType fieldType)
    {
        return OptionsMultipleChoicesTypes.Contains(fieldType);
    }
    public static bool IsOptionSingleChoice(this FieldType fieldType)
    {
        return !OptionsMultipleChoicesTypes.Contains(fieldType);
    }



    private static readonly IList<FieldType> TextInputTypes =
        new List<FieldType>
        {
            FieldType.InputTextSimple
            , FieldType.InputTextArea
            //for now we handle this not-strictly-text types here
            , FieldType.InputDate
            , FieldType.InputNumeric
        };

    public static bool IsTextInput(this FieldType fieldType)
    {
        return TextInputTypes.Contains(fieldType);
    }


    private static readonly IList<FieldType> FileInputTypes =
       new List<FieldType>
       {
            FieldType.InputMultipleFile
            , FieldType.InputOptionalMultipleFileUpload
            , FieldType.InputSingleFileOnly
       };
    public static bool IsFileInput(this FieldType fieldType)
    {
        return FileInputTypes.Contains(fieldType);
    }

    public static bool IsEditable(this FieldType fieldType)
    {
        return
            fieldType.IsOption()
            || fieldType.IsTextInput()
            || fieldType.IsFileInput()
            ;
    }
    public static bool NotEditable(this FieldType fieldType)
    {
        return !fieldType.IsEditable();
    }


    public static bool IsTypeUsableForCalculation(this FieldType fieldType)
    {
        return
            new List<FieldType>()
            {
                FieldType.InputTextArea,
                FieldType.InputTextSimple,
                FieldType.InputDate,
                FieldType.InputNumeric
            }
            .Contains(fieldType);
    }
}