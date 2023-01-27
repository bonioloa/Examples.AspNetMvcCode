namespace Examples.AspNetMvcCode.Logic;

public record FieldErrorOptionMultipleLgc : IFieldErrorLgc
{
    public FieldErrorOptionMultipleLgc(
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