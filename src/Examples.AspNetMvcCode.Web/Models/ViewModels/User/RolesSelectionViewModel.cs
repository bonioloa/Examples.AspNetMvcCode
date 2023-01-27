namespace Examples.AspNetMvcCode.Web.Models;

public record RolesSelectionViewModel(
    IEnumerable<OptionViewModel> ExclusiveRolesFound
    , IEnumerable<OptionViewModel> SupervisorRolesFound
    );