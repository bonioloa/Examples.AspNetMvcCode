namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// Config section for email client settings
/// </summary>
public class EmailClientSettings
{
    /// <summary>
    /// Smtp server user for mail sending. 
    /// Can be overridden by tenant configuration, maybe when a tenant will require a installation on their server
    /// </summary>
    public string DefaultSmtp { get; set; }

    /// <summary>
    /// Smtp port. No override by tenant configuration
    /// </summary>
    public int SmtpPort { get; set; }

    /// <summary>
    /// Type of credentials. At the moment no use case apart default.
    /// </summary>
    public string Credentials { get; set; }
}