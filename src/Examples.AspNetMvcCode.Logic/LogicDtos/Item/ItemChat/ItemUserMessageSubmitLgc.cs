namespace Examples.AspNetMvcCode.Logic;

public class ItemUserMessageSubmitLgc
{
    public string Phase { get; set; }
    public string State { get; set; }
    public string Subject { get; set; }
    public string Text { get; set; }
    public IList<FileAttachmentLgc> AttachmentList { get; set; } = new List<FileAttachmentLgc>();
}