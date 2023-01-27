namespace Examples.AspNetMvcCode.Web.Models;

public class ItemManagementViewModel
{
    public long Id { get; set; }
    public long IdMaster { get; set; }
    public long ProcessId { get; set; }
    public IHtmlContent ProcessDescription { get; set; }
    public long StepProgressive { get; set; }
    public IHtmlContent CurrentStepDescription { get; set; }
    public ExpirationViewModel Expiration { get; set; }
    public StepStateGroupType StepStateGroup { get; set; }
    public string ItemDescriptiveCode { get; set; }
    public DateTime? SubmitDateTime { get; set; }
    public StepFormViewModel StepForm { get; set; }
    public string Phase { get; set; }
    public string State { get; set; }
    public bool IsPastStep { get; set; }
    public bool IsFirstStep { get; set; }
    public bool UserHasModifyPermissions { get; set; }
    public HashSet<ItemCalculatedFieldViewModel> CalculatedFields { get; set; }
}