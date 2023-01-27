namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// add here properties for section handling architecture settings
/// </summary>
public class WebArchitectureSettings
{
    /// <summary>
    /// Prevent host spoofing in links. 
    /// List of host/domain to validate against. 
    /// </summary>
    public List<string> AllowedHosts { get; set; }

    /// <summary>
    /// Add here domains and url to allow cross-domain redirects
    /// </summary>
    public List<string> RedirectAllowedDestinations { get; set; }

    /// <summary>
    /// Necessary to differentiate cookies between products and environments, 
    /// in this way it's possible to test all sites on same browser at the same time.
    /// </summary>
    public string CookiesPrefix { get; set; }

    public int CookieAuthenticationTimeoutInMinutes { get; set; }

    public int SessionTimeoutInMinutes { get; set; }

    /// <summary>
    /// Following microsoft recommendation for each environment, but we leave this setting editable anyway
    /// </summary>
    public int HstsMaxAgeInDays { get; set; }

    /// <summary>
    /// quota for single post upload, different by product and to be changed in case it's not enough.
    /// Must be changed in all products appsettings.json and web.config
    /// </summary>
    public long FormMultipartBodyMaxInBytes { get; set; }
}