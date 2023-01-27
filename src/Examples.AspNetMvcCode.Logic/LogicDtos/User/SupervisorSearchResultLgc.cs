namespace Examples.AspNetMvcCode.Logic;

public record SupervisorSearchResultLgc(
    IEnumerable<HtmlString> OrphanedRolesDescriptions
    , IEnumerable<OptionLgc> AvailableRolesWithSelected
    , bool ShowResults
    , IEnumerable<UserFoundLgc> FoundResults
    , IList<MessageField> FieldToWarnList
    , bool Success
    , string ValuesAllowed
    , WarningType WarningType
    );