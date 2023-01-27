namespace Comunica.ProcessManager.Web.Models;

public class ItemSubmitViewModel : IOperationResultViewModel
{
    //probably answer could be converted in a dictionary, 
    //but we need more testing
    public IList<SubmittedInputViewModel> SubmittedInputList { get; set; }


    //the following fields are used only for item change
    public ItemChangeType ChangeType { get; set; }
    public string Phase { get; set; }
    public string State { get; set; }



    //inherited properties
    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldsToWarn { get; set; }
    public string ValuesAllowed { get; set; }
}
