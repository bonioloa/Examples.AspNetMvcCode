namespace Examples.AspNetMvcCode.Logic.LogicServices.User;

public interface ISupervisorAdministrationLogic
{
    SupervisorSearchResultLgc ConditionalSearch(bool performSearch, string filterSurname, string filterName, string filterEmail, IEnumerable<long> filterRoles);
    RolesSelectionLgc GetAvailableRolesForNewUser(string selectedExclusiveRole, IEnumerable<long> selectedSupervisorRoles);
    UserSupervisorDataLgc GetUserSupervisorData(long userId);
}