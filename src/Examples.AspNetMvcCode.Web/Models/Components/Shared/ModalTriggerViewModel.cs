namespace Examples.AspNetMvcCode.Web.Models;

public class ModalTriggerViewModel
{
    public string ModalId { get; set; }
    public string Description { get; set; }
    public IList<string> AdditionalClasses { get; set; } = new List<string>();
    public bool ShowAsButton { get; set; }
    public bool IsPrimaryButton { get; set; }
    public IHtmlContent IconCode { get; set; }
    public IList<string> IconClasses { get; set; } = new List<string>();
    public string TooltipDescription { get; set; }
}