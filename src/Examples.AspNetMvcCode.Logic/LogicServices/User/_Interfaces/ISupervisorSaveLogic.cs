namespace Examples.AspNetMvcCode.Logic.LogicServices.User;

public interface ISupervisorSaveLogic
{
    string DisableUser(long userId);
    UserResetResultLgc EnableUser(long userId);
    string IncludeUserInManaged(long userId);
    UserEditFromAdminResultLgc ModifyUserDataFromAdmin(UserEditFromAdminLgc input);
    UserResetResultLgc ResetPasswordFromAdmin(long userId);
    UserNewSupervisorResultLgc SaveNewSupervisor(UserNewSupervisorLgc input);
}