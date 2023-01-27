namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// available after tenant login, mapped in cookie validation
/// </summary>
public class ContextTenant
{
    public string Token { get; set; }
    public ConfigurationType Type { get; set; }
    public string LogoFileName { get; set; }
    public bool TwoFactorAuthenticationEnabled { get; set; }
    public long CompanyGroupId { get; set; }
    public bool DisableRegistrationForUsers { get; set; }


    public string SsoSpCertificatePath { get; set; }

    [JsonIgnore]
    public string SsoSpCertificatePassword { get; set; }

    public string SsoSpDomain { get; set; }


    public SsoLoginMode SsoLoginMode { get; set; }
    //attention the string part is buttons description, it's localized 
    //and every page load must be loaded with correct languages
    public IDictionary<long, string> SsoIdpConfigDict { get; set; }


    //logic Dto is missing this property
    public IList<string> ValidatedDbCulturesIsoCodes { get; set; }
}