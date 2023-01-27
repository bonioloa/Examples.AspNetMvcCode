namespace Examples.AspNetMvcCode.Web.Models;

public class ItemLinkedViewModel
{
    public long Id { get; set; }
    public long IdMaster { get; set; }
    public string ItemDescriptiveCode { get; set; }
    public string Phase { get; set; }
    public string State { get; set; }
    public DateTime SubmitDateTime { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long ProcessId { get; set; }
    public IHtmlContent ProcessDescription { get; set; }
    //public IDictionary<string, string> CalculationData { get; set; }//da abilitare se servirà più avanti
    public IHtmlContent CurrentStepDescription { get; set; }
}