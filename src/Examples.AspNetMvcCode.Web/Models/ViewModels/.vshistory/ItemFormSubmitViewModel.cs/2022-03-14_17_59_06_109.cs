namespace Comunica.ProcessManager.Web.Models;

public class ItemFormSubmitViewModel : IOperationResultViewModel, IItemFormBasicModel
{
    public string ItemDescriptiveCode { get; set; }
    public IHtmlContent ProcessDescription { get; set; }
    public IHtmlContent StepDescription { get; set; }
    public IList<SubmittedInputViewModel> SubmittedInputList { get; set; }


    //inherited properties
    public long IdItem { get; set; }
    public long FormId { get; set; }
    public DateTime SubmitDate { get; set; }
    public string SubmitUserName { get; set; }
    public string SubmitUserSurname { get; set; }


    //inherited properties
    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldsToWarn { get; set; }
    public string ValuesAllowed { get; set; }
}
