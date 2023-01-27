namespace Comunica.ProcessManager.Web.Models;

public class UserFoundModel
{
    public long UserId { get; set; }
    public string Login { get; set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserProfile UserProfile { get; set; }
}
