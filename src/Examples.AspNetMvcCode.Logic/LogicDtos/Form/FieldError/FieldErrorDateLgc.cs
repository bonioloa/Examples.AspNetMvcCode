namespace Examples.AspNetMvcCode.Logic;

public record FieldErrorDateLgc : IFieldErrorLgc
{
    public FieldErrorDateLgc(
        IHtmlContent fieldDescription
        , string wrongValue
        , string requiredFormat
        )
    {
        FieldDescription = fieldDescription;
        WrongValue = wrongValue;
        RequiredFormat = requiredFormat;
    }

    public IHtmlContent FieldDescription { get; init; }
    public string WrongValue { get; init; }
    public string RequiredFormat { get; init; }
}