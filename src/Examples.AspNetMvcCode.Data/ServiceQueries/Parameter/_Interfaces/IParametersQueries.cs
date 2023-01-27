namespace Examples.AspNetMvcCode.Data;

public interface IParametersQueries
{
    DateTime CalculateNextExpirationPassword();
    IEnumerable<ExpirationConfigQr> GetExpirationConfigs();
    IList<string> GetEmailDomainRestriction();
    TenantOwnConfigurationQr GetTenantOwnConfiguration();
    string GetEmailBodyWithReplacements(string itemDescriptiveCode, IHtmlContent currentStepDescription, IHtmlContent processDescription);
    bool GetTenantDisplayPreferences();
    SsoLoginMode GetSsoLoginMode();
    bool HasOldOptionsSchemaType();
    bool TenantCanInsertItemsFromFile();
    bool HasReportAdvanced();
    bool AllowUserChatOnItems();
    EmailConfigTenantQr GetEmailSmtpTenantConfig();
    bool ForceNotificationEmailForItemChangeStep();
    string GetEmailSubjectWithReplacements(string itemDescriptiveCode, IHtmlContent currentStepDescription, IHtmlContent processDescription);
}