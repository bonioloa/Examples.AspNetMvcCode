namespace Comunica.ProcessManager.Web.Models;

public class ButtonsBackSubmitViewModel
{
    public string Id { get; set; }
    public IList<string> CssClasses { get; set; }
    public string SubmitLabel { get; set; }
    public bool SubmitDisabled { get; set; }
    public string SubmitValue { get; set; }
    public bool HideBackButtonComponent { get; set; }
    public BackUrlConfig BackButtonUrlType { get; set; }
}
