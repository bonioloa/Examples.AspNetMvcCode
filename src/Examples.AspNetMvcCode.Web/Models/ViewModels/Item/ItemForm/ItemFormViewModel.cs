namespace Examples.AspNetMvcCode.Web.Models;

public class ItemFormViewModel : IOperationResultViewModel, IItemFormBasicModel
{
    public string ItemDescriptiveCode { get; set; }
    public IHtmlContent ProcessDescription { get; set; }
    public IHtmlContent StepDescription { get; set; }
    public IList<FieldViewModel> FieldList { get; set; }



    public long ItemId { get; set; }
    public long FormId { get; set; }
    public DateTime SubmitDate { get; set; }
    public string SubmitUserName { get; set; }
    public string SubmitUserSurname { get; set; }



    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldToWarnList { get; set; }
    public string ValuesAllowed { get; set; }
}