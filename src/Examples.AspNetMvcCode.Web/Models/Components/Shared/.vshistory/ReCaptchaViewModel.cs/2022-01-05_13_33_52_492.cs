namespace Comunica.ProcessManager.Web.Models;

public class ReCaptchaViewModel
{
    public RecaptchaSettings RecaptchaSettings { get; set; }
    public string Profile { get; set; }
    public string CallbackFunction { get; set; }
}
