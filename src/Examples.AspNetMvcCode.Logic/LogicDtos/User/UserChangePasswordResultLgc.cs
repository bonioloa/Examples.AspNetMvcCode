namespace Examples.AspNetMvcCode.Logic;

public class UserChangePasswordResultLgc : IOperationResultLgc
{
    public DateTime Expiration { get; set; }



    [JsonIgnore]
    public IList<MessageField> FieldToWarnList { get; set; } = new List<MessageField>();

    [JsonIgnore]
    public bool Success { get; set; }

    [JsonIgnore]
    public string ValuesAllowed { get; set; }

    [JsonIgnore]
    public WarningType WarningType { get; set; }
}