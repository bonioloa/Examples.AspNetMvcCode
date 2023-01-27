namespace Comunica.ProcessManager.Web.Models;

public class ItemStepStatisticModel
{
    public long StepProgressive { get; set; }
    public string StepCode { get; set; }
    public IHtmlContent StepDescription { get; set; }
    public long Count { get; set; }
}
