namespace Comunica.ProcessManager.Web.Models;

public class ProcessStepFilterViewModel
{
    public bool Selected { get; set; }
    public string Code { get; set; }
    public IHtmlContent Description { get; set; }
    public StepStateGroupType StepStateGroup { get; set; }
}
