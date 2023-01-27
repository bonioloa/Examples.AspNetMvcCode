namespace Examples.AspNetMvcCode.Web.Models;

public class ItemSubmitViewModel : IOperationResultViewModel
{
    //probably answer could be converted in a dictionary, 
    //but we need more testing
    public List<SubmittedInputViewModel> SubmittedInputList { get; set; }



    public ItemChangeType ChangeType { get; set; }
    public string Phase { get; set; }
    public string State { get; set; }




    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldToWarnList { get; set; }
    public string ValuesAllowed { get; set; }
}