namespace Examples.AspNetMvcCode.Web.Models;

public class ItemUserMessageViewModel
{
    public long Id { get; set; }
    public bool SenderIsBaseUser { get; set; }
    public string SenderUserSurname { get; set; }
    public string SenderUserName { get; set; }
    public DateTime DateAndTime { get; set; }
    public IList<FileAttachmentViewModel> Attachments { get; set; }
    public string Subject { get; set; }
    public string Text { get; set; }
}