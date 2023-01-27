namespace Examples.AspNetMvcCode.Web.Models;

public record PoliciesViewModel(
    IList<CompanyDocumentViewModel> PrivacyPolicies
    );