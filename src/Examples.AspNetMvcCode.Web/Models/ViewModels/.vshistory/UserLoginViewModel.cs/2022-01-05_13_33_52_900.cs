namespace Comunica.ProcessManager.Web.Models;

/// <summary>
/// this model has multiple uses. It contains
/// -the data to show in referenced view
/// -sso providers information
/// </summary>
public class UserLoginViewModel
{
    public LoginType FormToShow { get; set; }
    public IDictionary<long, string> SsoConfigDict { get; set; }
}
