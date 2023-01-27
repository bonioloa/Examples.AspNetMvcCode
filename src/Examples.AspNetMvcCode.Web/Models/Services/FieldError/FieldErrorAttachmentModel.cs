namespace Examples.AspNetMvcCode.Web.Models;

public record FieldErrorAttachmentModel : IFieldErrorModel
{
    public FieldErrorAttachmentModel(
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