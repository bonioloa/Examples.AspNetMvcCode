namespace Examples.AspNetMvcCode.Logic;

public record RolesSelectionLgc(
    IEnumerable<OptionLgc> ExclusiveRolesFound
    , IEnumerable<OptionLgc> SupervisorRolesFound
    );