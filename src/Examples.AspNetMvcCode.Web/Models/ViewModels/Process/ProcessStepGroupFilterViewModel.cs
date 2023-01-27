namespace Examples.AspNetMvcCode.Web.Models;

public class ProcessStepGroupFilterViewModel
{
    public long ProcessId { get; set; }
    public IHtmlContent ProcessDescription { get; set; }
    public IList<ProcessStepFilterViewModel> Steps { get; set; }
}