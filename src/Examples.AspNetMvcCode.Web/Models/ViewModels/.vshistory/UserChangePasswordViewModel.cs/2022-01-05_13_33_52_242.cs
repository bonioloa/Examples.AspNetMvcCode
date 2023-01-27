namespace Comunica.ProcessManager.Web.Models;

public class UserChangePasswordViewModel
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
