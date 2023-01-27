namespace Examples.AspNetMvcCode.Web.Models;

public record SupervisorSearchViewModel(
    string Surname
    , string Name
    , string Email
    , IEnumerable<OptionViewModel> Roles
    , IEnumerable<HtmlString> OrphanedRolesDescriptions
    , bool ShowResults
    , DataTablesNetViewModel SearchResultsModel
    );