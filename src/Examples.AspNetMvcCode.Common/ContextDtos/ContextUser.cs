namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// in this object will be mapped some claims data from authentication and session
/// allowing user data and config to be shared between layers.
/// user personal data will be loaded only when needed and not stored here
/// </summary>
public class ContextUser
{
    #region user claim
    //will not be available for simple anonymous access (no logincode)
    public long UserIdLoggedIn { get; set; }

    //always available
    public IEnumerable<long> AssignedSupervisorRolesFound { get; set; } = Enumerable.Empty<long>();


    public AccessType AccessType { get; set; }
    public bool IsAlsoAdminTenant { get; set; }
    public ExclusiveRole ExclusiveRoleType { get; set; }
    public long SsoConfigId { get; set; }
    #endregion

    //sso 
    public bool PasswordIsExpired { get; set; }

    //extra properties dependent from session 
    public long ProcessId { get; set; }
    public long ItemIdCurrentlyManagedByUser { get; set; }
}