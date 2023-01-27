namespace Examples.AspNetMvcCode.Web.Models;

public record ExpirationViewModel(
    string MessagePrefix
    , DateTimeSpan? CompareResult
    );