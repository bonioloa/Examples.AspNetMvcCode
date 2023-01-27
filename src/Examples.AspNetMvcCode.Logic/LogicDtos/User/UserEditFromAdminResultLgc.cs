namespace Examples.AspNetMvcCode.Logic;

public record UserEditFromAdminResultLgc(
    string ErrorMessage
    , long UserId
    , bool EmailChanged
    , UserSupervisorDataLgc UserDataBefore
    , UserSupervisorDataLgc UserDataUpdated
    );