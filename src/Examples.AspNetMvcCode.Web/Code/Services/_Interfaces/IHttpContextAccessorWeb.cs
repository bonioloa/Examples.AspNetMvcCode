namespace Examples.AspNetMvcCode.Web.Code;

public interface IHttpContextAccessorWeb : IHttpContextAccessor
{
    IDictionary<string, string> ContextRequestQuerystringToDictionary();
    string ContextLanguage { get; }
    string ContextController { get; }
    string ContextAction { get; }
    Guid SessionIdentifier { get; set; }
    bool IsLoggedOrHasSession();
    bool SignInTenant(TenantProfileModel tenantProfile);
    bool SignInUser(UserProfileModel userProfile);
    void LogoutClearSessionAndPersonalCookies();
    RedirectToRouteResult ClearSessionAndCookieAndGetRedirectForLogin();
    void SaveRouteForBackTenantLogin();
    bool IsTenantLoginWithoutLeftPanel();
    void SaveRouteForBackItemViewAndManage();
    long SessionProcessId { get; set; }
    long SessionItemIdCurrentlyManagedByUser { get; set; }
    UserLoginViewModel TempDataOnceLoginUser { get; set; }
    UserProfileModel TempDataOnceUserProfile { get; set; }
    UserChangePasswordResultViewModel TempDataOnceChangePasswordResult { get; set; }
    bool TempDataOnceRecoverSuccess { get; set; }
    UserRegistrationViewModel TempDataOnceRegister { get; set; }
    OperationResultViewModel SessionOperationResult { get; set; }
    bool SessionHasSingleProcessConfiguration { get; set; }
    string SessionProcessLogoFileName { get; set; }
    ProblemReportViewModel TempDataOnceProblemReport { get; set; }
    UserNewSupervisorSaveViewModel TempDataOnceUserNewSupervisorSave { get; set; }

    void SessionRemoveOperationResult();
    bool IsBannerCookiePolicyDismissed();
    IDictionary<string, string> GetContextRequestQuerystringWithLanguage(string cultureIsoCode);
    RouteViewModel GetRouteForBackTenantLogin(bool removeTenantTokenFromQuerystring);
    RouteViewModel GetRouteForBackItemViewAndManage();
    bool SignInSso(IEnumerable<Claim> ssoClaims, TenantProfileModel tenantProfile, UserProfileModel userProfile);
    void SaveRouteForBackUserViewAndManage();
    RouteViewModel GetRouteForBackUserViewAndManage();
}