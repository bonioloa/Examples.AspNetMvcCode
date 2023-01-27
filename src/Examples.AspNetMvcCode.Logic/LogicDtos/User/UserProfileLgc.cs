namespace Examples.AspNetMvcCode.Logic;

/// <summary>
/// object to use for loading properties useful in all reserved area
/// </summary>
public class UserProfileLgc : IUserProfileValidationLgc
{
    public long UserIdLoggedIn { get; set; }
    public IEnumerable<long> AssignedSupervisorRolesList { get; set; } = Enumerable.Empty<long>();
    public long ItemIdFromLoginCode { get; set; }
    public AccessType AccessType { get; set; }
    public bool IsAlsoAdminTenant { get; set; }
    public ExclusiveRole ExclusiveRoleType { get; set; }
    public long SsoConfigId { get; set; }


    //2factor authentication fields
    [JsonIgnore]
    public string StrongAuthAccessCode { get; set; }

    [JsonIgnore]
    public string Email { get; set; }



    [JsonIgnore]
    public LoginType FormToShow { get; set; }

    [JsonIgnore]
    public bool UserSsoRequiresPermissions { get; set; }



    [JsonIgnore]
    public IList<MessageField> FieldToWarnList { get; set; } = new List<MessageField>();

    [JsonIgnore]
    public bool Success { get; set; }

    [JsonIgnore]
    public string ValuesAllowed { get; set; }

    [JsonIgnore]
    public WarningType WarningType { get; set; }
}