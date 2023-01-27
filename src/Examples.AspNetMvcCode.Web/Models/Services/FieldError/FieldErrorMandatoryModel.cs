namespace Examples.AspNetMvcCode.Web.Models;

public record FieldErrorMandatoryModel : IFieldErrorModel
{
    public FieldErrorMandatoryModel(
        IHtmlContent fieldDescription
        )
    {
        FieldDescription = fieldDescription;
    }

    public IHtmlContent FieldDescription { get; init; }
}