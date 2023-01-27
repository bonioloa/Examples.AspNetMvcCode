namespace Examples.AspNetMvcCode.Web.Models;

public record FieldErrorNumberModel : IFieldErrorModel
{
    public FieldErrorNumberModel(
        IHtmlContent fieldDescription
        , string wrongValue
        , string exampleCorrect
        )
    {
        FieldDescription = fieldDescription;
        WrongValue = wrongValue;
        ExampleCorrect = exampleCorrect;
    }

    public IHtmlContent FieldDescription { get; init; }
    public string WrongValue { get; init; }
    public string ExampleCorrect { get; init; }
}