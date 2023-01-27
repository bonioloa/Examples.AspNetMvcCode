namespace Comunica.ProcessManager.Web.Models;

public class ItemUserMessageSubmitViewModel
{
    public string Phase { get; set; }
    public string State { get; set; }
    public string Subject { get; set; }
    public string Text { get; set; }
    public IList<FileAttachmentViewModel> FileAttachmentList { get; set; }
}
