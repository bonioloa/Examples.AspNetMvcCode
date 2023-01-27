namespace Examples.AspNetMvcCode.Logic;

public class SubmittedInputLgc
{
    public string FieldName { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public bool ShowOnReports { get; set; }
    public List<FileAttachmentLgc> Attachments { get; set; } = new();


    public string SelectedCode { get; set; } //not used in mapper
}