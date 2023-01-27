namespace Examples.AspNetMvcCode.Logic;

public class UserRecoverCredentialsResultLgc : IOperationResultLgc
{
    //for mail send
    public string Email { get; set; } = string.Empty;
    public string UserLogin { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public RecoverType RecoverType { get; set; }



    [JsonIgnore]
    public IList<MessageField> FieldToWarnList { get; set; } = new List<MessageField>();

    [JsonIgnore]
    public bool Success { get; set; }

    [JsonIgnore]
    public string ValuesAllowed { get; set; }

    [JsonIgnore]
    public WarningType WarningType { get; set; }
}