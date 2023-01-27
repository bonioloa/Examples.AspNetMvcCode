namespace Comunica.ProcessManager.Web.Models;

public class UserProfileModel
{
    public long UserIdLoggedIn { get; set; }
    public IList<OptionViewModel> SupervisorRolesList { get; set; }
    public long IdItemFromLoginCode { get; set; }
    public AccessType AccessType { get; set; }
    public bool IsAdmin { get; set; }
    public long SsoConfigId { get; set; }
}
