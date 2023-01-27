namespace Comunica.ProcessManager.Web.Models;

public class TenantProfileModel
{
    public string Token { get; set; }
    public ConfigurationType Type { get; set; }
    public string LogoFileName { get; set; }
    public bool TwoFactorAuthenticationEnabled { get; set; }
    public long CompanyGroupId { get; set; }
    public bool DisableRegistrationForUsers { get; set; }
    public SsoLoginMode SsoLoginMode { get; set; }
    public IDictionary<long, string> SsoIdentifiers { get; set; }
}
