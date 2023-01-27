namespace Examples.AspNetMvcCode.Web.Models;

public class ProcessLinkedRuleViewModel
{
    public IHtmlContent OpenNewLinkedItemDescription { get; set; }
    public long ProcessMasterId { get; set; }
    public string PhaseMaster { get; set; }
    public string StateMaster { get; set; }
}