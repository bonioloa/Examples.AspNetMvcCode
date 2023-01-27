namespace Examples.AspNetMvcCode.Logic;

public interface IUserProfileValidationLgc : IOperationResultLgc
{
    [JsonIgnore]
    LoginType FormToShow { get; set; }

    [JsonIgnore]
    bool UserSsoRequiresPermissions { get; set; }
}