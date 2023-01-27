namespace Examples.AspNetMvcCode.Web.Models;

public record FieldErrorDateModel : IFieldErrorModel
{
    public FieldErrorDateModel(
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