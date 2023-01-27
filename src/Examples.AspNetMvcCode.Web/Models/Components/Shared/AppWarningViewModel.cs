namespace Examples.AspNetMvcCode.Web.Models;

public record AppWarningViewModel(
    string Title
    , IList<string> MessageLines
    );