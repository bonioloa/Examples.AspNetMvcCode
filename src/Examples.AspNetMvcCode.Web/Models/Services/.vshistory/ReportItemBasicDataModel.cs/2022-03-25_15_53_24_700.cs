namespace Comunica.ProcessManager.Web.Models;

public class ReportItemBasicDataModel
{
    public DateTime SubmitDateTime { get; set; }
    public string SubmitUserName { get; set; }
    public string SubmitUserSurname { get; set; }
    public IHtmlContent ProcessDescription { get; set; }
    public long Id { get; set; }
    public long IdMaster { get; set; }
    public long ProcessId { get; set; }
    public string ItemDescriptiveCode { get; set; }
    public IHtmlContent CurrentStepDescription { get; set; }
    public StepStateGroupType StepStateGroup { get; set; }
    public DateTime ExpirationDate { get; set; }
    public HashSet<ItemCalculatedFieldViewModel> CalculatedFields { get; set; }
}