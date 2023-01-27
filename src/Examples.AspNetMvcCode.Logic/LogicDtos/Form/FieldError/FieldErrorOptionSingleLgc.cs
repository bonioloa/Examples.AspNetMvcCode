namespace Examples.AspNetMvcCode.Logic;

public record FieldErrorOptionSingleLgc : IFieldErrorLgc
{
    public FieldErrorOptionSingleLgc(
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