namespace Examples.AspNetMvcCode.Logic.LogicServices.User;

public interface ISupervisorSaveChecksLogic
{
    string CheckIfEmailExists(string email);
    string CheckIfEmailExistsForOtherActiveUsers(string email, long userToExclude);
    string CheckIfLoginExists(string login);
    string CheckIfLoginExistsForOtherActiveUsers(string login, long userToExclude);
    string ChecksEnableDisable(long userId, bool isEnableCommand);
}