namespace Examples.AspNetMvcCode.Logic;

public record FieldErrorAttachmentLgc : IFieldErrorLgc
{
    public FieldErrorAttachmentLgc(
        IHtmlContent fieldDescription
        , IList<string> missingDeclaredAttachment
        )
    {
        FieldDescription = fieldDescription;
        MissingDeclaredAttachment = missingDeclaredAttachment;
    }

    public IHtmlContent FieldDescription { get; init; }
    public IList<string> MissingDeclaredAttachment { get; init; }
}