namespace Examples.AspNetMvcCode.Logic.LogicServices.User;

public interface IRoleAdminManagedLogic
{
    RolesSelectionLgc CheckAndSetRolesSelectionForModifyUser(long userId);
    RolesSelectionLgc CheckAndSetRolesSelectionForNewUser(string selectedExclusiveRole, IEnumerable<long> selectedSupervisorRoles);
    IEnumerable<OptionLgc> GetRolesFilterForSearch(IEnumerable<long> selectedFilterRoles);
    IEnumerable<HtmlString> GetRolesOrphansOfUsers();
    RolesSelectionResultLgc ValidateAndParseSubmittedRolesSelectionForModifyUser(long userId, string selectedExclusiveRole, IEnumerable<long> selectedSupervisorRoles);
    RolesSelectionResultLgc ValidateAndParseSubmittedRolesSelectionForNewUser(string selectedExclusiveRole, IEnumerable<long> selectedSupervisorRoles);
}