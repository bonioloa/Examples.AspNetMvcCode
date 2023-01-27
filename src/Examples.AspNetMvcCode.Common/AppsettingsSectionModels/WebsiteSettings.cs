namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// add here properties that belongs to site but are not related to architecture, neither to product type
/// </summary>
public class WebsiteSettings
{
    /// <summary>
    /// in case of maintenance on a tenant db that requires that no user perform any operation, change this setting at true.
    /// </summary>
    public bool ForceMaintenancePage { get; set; }
}