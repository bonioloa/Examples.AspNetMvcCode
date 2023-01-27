namespace Examples.AspNetMvcCode.Web.Models;

public record FieldErrorOptionSingleModel : IFieldErrorModel
{
    public FieldErrorOptionSingleModel(
        IHtmlContent fieldDescription
        , IList<string> invalidOptionList
        )
    {
        FieldDescription = fieldDescription;
        InvalidOptionList = invalidOptionList;
    }

    public IHtmlContent FieldDescription { get; init; }
    public IList<string> InvalidOptionList { get; init; }
}