namespace Examples.AspNetMvcCode.Logic;

public class UserRegistrationResultLgc : IOperationResultLgc
{
    public long NewUserId { get; set; }
    public string ValidationCode { get; set; }
    public string UserName { get; set; }
    public string UserLogin { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }



    [JsonIgnore]
    public IList<MessageField> FieldToWarnList { get; set; } = new List<MessageField>();

    [JsonIgnore]
    public bool Success { get; set; }

    [JsonIgnore]
    public string ValuesAllowed { get; set; }

    [JsonIgnore]
    public WarningType WarningType { get; set; }
}