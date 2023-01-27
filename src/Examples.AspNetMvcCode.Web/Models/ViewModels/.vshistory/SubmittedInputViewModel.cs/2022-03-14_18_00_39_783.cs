namespace Comunica.ProcessManager.Web.Models;

public class SubmittedInputViewModel
{
    public string FieldName { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public bool ShowOnReports { get; set; }
    public IList<FileAttachmentViewModel> Attachments { get; set; }
}
