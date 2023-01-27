namespace Examples.AspNetMvcCode.Logic;

public record FieldErrorMandatoryLgc : IFieldErrorLgc
{
    public FieldErrorMandatoryLgc(
        IHtmlContent fieldDescription
        )
    {
        FieldDescription = fieldDescription;
    }

    public IHtmlContent FieldDescription { get; init; }
}