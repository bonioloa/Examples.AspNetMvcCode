namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// Application generic config section
/// </summary>
public class ProductSettings
{
    public Product Product { get; set; }

    /// <summary>
    /// If true the messages component(between basic user and supervisor ) in item management page will not appear 
    /// </summary>
    public bool DisableUsersChatComponent { get; set; }

    /// <summary>
    /// Will use a simple title for component not differentiated from basic user and supervisor. 
    /// This simple title will be used mainly for registers, when component will be enabled by personalization
    /// </summary>
    public bool UseSimpleTitleForUsersChat { get; set; }

    /// <summary>
    /// Show complete user name in chat message, instead of using role type description
    /// </summary>
    public bool ShowChatSenderName { get; set; }

    /// <summary>
    /// If true the cookie privacy document and banner link will not appear
    /// </summary>
    public bool DisablePrivacyComponents { get; set; }

    /// <summary>
    /// If true the notification of items step change will not be sent to users with modify permission
    /// </summary>
    public bool DisableEmailNotificationsChangeStep { get; set; }

    /// <summary>
    /// If true the buttons to export the latest step data will appear on history and item management
    /// </summary>
    public bool EnableStepDataExport { get; set; }

    /// <summary>
    /// If true the buttons to view a step data change history will appear on history and item management
    /// </summary>
    public bool EnableCompleteHistory { get; set; }

    /// <summary>
    /// If true report file will have an additional column for item data to display the name of user that created the item
    /// </summary>
    public bool ShowInReportItemInsertUser { get; set; }

    /// <summary>
    /// If true for this configuration, expiration is defined by the end date saved for item.
    /// Expiration date will also be shown in item search page
    /// </summary>
    public bool UseItemEndDateForExpiration { get; set; }

    /// <summary>
    /// Name of css file with the classes customized for each product
    /// </summary>
    public string ProductDefaultTheme { get; set; }


    //starting page properties

    /// <summary>
    /// A different logo for each environment and product type
    /// </summary>
    public string ProductLogoFileName { get; set; }

    /// <summary>
    /// Text to show in case the log will not be found
    /// </summary>
    public string ProductLogoImageAltText { get; set; }

    /// <summary>
    /// Link to show a link for each product
    /// </summary>
    public string ProductWebsite { get; set; }

    /// <summary>
    /// Display text for website link
    /// </summary>
    public string ProductWebsiteText { get; set; }

    /// <summary>
    /// Contact email
    /// </summary>
    public string ProductSupportEmail { get; set; }

    /// <summary>
    /// Display text for contact email
    /// </summary>
    public string ProductSupportEmailText { get; set; }

    /// <summary>
    /// Image file to use for favorites (will also be shown on site browser tab)
    /// </summary>
    public string ProductFavIcon { get; set; }
}