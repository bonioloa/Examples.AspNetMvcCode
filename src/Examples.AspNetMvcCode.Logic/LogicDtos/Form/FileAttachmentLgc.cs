namespace Examples.AspNetMvcCode.Logic;

public class FileAttachmentLgc : IOperationResultLgc
{
    public long ItemId { get; set; }
    public string Phase { get; set; }
    public string State { get; set; }
    public string FieldNameAssociated { get; set; }

    public long Id { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }
    public DateTime UploadDate { get; set; }

    /// <summary>
    /// file content not always present for performance reason.<br/>
    /// Do a specific inquiry to get it ONLY when necessary<br/>
    /// (ie: when a user requests a file download)<br/>
    /// </summary>
    public byte[] ByteContent { get; set; } = null;


    //inherited properties
    [JsonIgnore]
    public IList<MessageField> FieldToWarnList { get; set; } = new List<MessageField>();

    [JsonIgnore]
    public bool Success { get; set; }

    [JsonIgnore]
    public string ValuesAllowed { get; set; }

    [JsonIgnore]
    public WarningType WarningType { get; set; }
}