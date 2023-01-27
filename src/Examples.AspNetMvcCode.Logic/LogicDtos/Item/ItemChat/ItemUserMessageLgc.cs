namespace Examples.AspNetMvcCode.Logic;

public class ItemUserMessageLgc
{
    public long Id { get; set; }
    public bool SenderIsBaseUser { get; set; }
    public string SenderUserSurname { get; set; }
    public string SenderUserName { get; set; }
    public DateTime DateAndTime { get; set; }
    public IList<FileAttachmentLgc> Attachments { get; set; } = new List<FileAttachmentLgc>();
    public string Subject { get; set; }
    public string Text { get; set; }
}