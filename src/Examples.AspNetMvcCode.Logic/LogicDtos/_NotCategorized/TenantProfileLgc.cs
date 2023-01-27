namespace Examples.AspNetMvcCode.Logic;

public class TenantProfileLgc : IOperationResultLgc
{
    public string Token { get; set; }
    public ConfigurationType Type { get; set; }
    public string LogoFileName { get; set; }
    public bool TwoFactorAuthenticationEnabled { get; set; }
    public long CompanyGroupId { get; set; }
    public bool DisableRegistrationForUsers { get; set; }

    //following properties are not used in context
    public SsoLoginMode SsoLoginMode { get; set; }
    public long SsoSpConfigId { get; set; }
    public IDictionary<long, string> SsoIdpConfigDict { get; set; } = new Dictionary<long, string>();



    [JsonIgnore]
    public IList<MessageField> FieldToWarnList { get; set; } = new List<MessageField>();

    [JsonIgnore]
    public bool Success { get; set; }

    [JsonIgnore]
    public string ValuesAllowed { get; set; }

    [JsonIgnore]
    public WarningType WarningType { get; set; }
}