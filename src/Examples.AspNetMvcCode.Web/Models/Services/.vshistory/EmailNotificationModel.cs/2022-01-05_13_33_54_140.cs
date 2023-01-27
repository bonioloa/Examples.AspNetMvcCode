namespace Comunica.ProcessManager.Web.Models;

public class EmailNotificationModel
{
    public string EmailFromAddress { get; set; }
    public string EmailFromDisplayName { get; set; }
    public IList<string> EmailToList { get; set; }
    public IList<string> EmailCCList { get; set; }
    public IList<string> EmailBCCList { get; set; }
    public string Subject { get; set; }
    public IHtmlContent Body { get; set; }
    public bool SendMailBodyAsText { get; set; }
}
