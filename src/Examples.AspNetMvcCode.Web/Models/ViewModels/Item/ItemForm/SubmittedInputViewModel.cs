namespace Examples.AspNetMvcCode.Web.Models;

public class SubmittedInputViewModel
{
    public string FieldName { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public bool ShowOnReports { get; set; }
    public List<FileAttachmentViewModel> Attachments { get; set; }
}