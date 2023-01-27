namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// COnfiguration for email notifications to send in case of app errors or problem.
/// Useful to know exactly when something went wrong and do the fix independently from user support requests.
/// </summary>
public class EmailUfSupportSettings
{
    /// <summary>
    /// necessary in case we need to temporarily disable (I.E. a penetration test)
    /// </summary>
    public bool EnableForGenericError { get; set; }
    /// <summary>
    /// necessary in case we need to temporarily disable (I.E. a penetration test)
    /// </summary>
    public bool EnableForErrorNotSupportedBrowser { get; set; }
    /// <summary>
    /// necessary in case we need to temporarily disable (I.E. a penetration test)
    /// </summary>
    public bool EnableForCodePages { get; set; }
    /// <summary>
    /// necessary in case we need to temporarily disable (I.E. a penetration test)
    /// </summary>
    public bool EnableForWrongSchedulingIp { get; set; }

    /// <summary>
    /// notification address list
    /// </summary>
    public List<string> ToEmailAddress { get; set; }

    /// <summary>
    /// Allows identification of environment and deploy type where the error occurred 
    /// </summary>
    public string PrefixSubject { get; set; }

    /// <summary>
    /// list of html error codes to ignore
    /// </summary>
    public List<int> IgnoreEmailForCodes { get; set; }

    /// <summary>
    /// list of string to check against user agent to prevent sending support emails 
    /// when browser used is not supported. This is intended to separate user errors from crawler bots
    /// </summary>
    public List<string> IgnoreEmailErrorCrawlerBots { get; set; }
}