namespace Comunica.ProcessManager.Web.Models;

public class OperationResultViewModel : IOperationResultViewModel
{
    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }


    //fields in common with Lgc
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldsToWarn { get; set; }
    public string ValuesAllowed { get; set; }
}
