namespace Examples.AspNetMvcCode.Web.Models;

public record UserNewSupervisorStartViewModel(
    RolesSelectionViewModel RolesForSelectionWithRestored
    , UserNewSupervisorSaveViewModel UserNewSupervisorSaveInputRestore
    );