namespace Examples.AspNetMvcCode.Web.Models;

public class UserProfileModel
{
    public long UserIdLoggedIn { get; set; }
    public IEnumerable<long> AssignedSupervisorRolesList { get; set; } = Enumerable.Empty<long>();
    public long ItemIdFromLoginCode { get; set; }
    public AccessType AccessType { get; set; }
    public bool IsAlsoAdminTenant { get; set; }
    public ExclusiveRole ExclusiveRoleType { get; set; }
    public long SsoConfigId { get; set; }
}