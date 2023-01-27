namespace Comunica.ProcessManager.Web.Models;

public interface IOperationResultViewModel
{
    IList<MessageField> FieldsToWarn { get; set; }
    string LocalizedMessage { get; set; }
    string LocalizedTitle { get; set; }
    bool Success { get; set; }
    string ValuesAllowed { get; set; }
    WarningType WarningType { get; set; }
}
