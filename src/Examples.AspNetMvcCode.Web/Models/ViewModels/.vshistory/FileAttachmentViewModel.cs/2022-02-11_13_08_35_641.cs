namespace Comunica.ProcessManager.Web.Models;

public class FileAttachmentViewModel : IOperationResultViewModel
{
    public string Name { get; set; }
    public string MimeType { get; set; }
    public long Id { get; set; }
    public string Phase { get; set; }
    public string State { get; set; }
    public byte[] ByteContent { get; set; }
    public DateTime UploadDate { get; set; }



    //inherited properties
    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldsToWarn { get; set; }
    public string ValuesAllowed { get; set; }
}