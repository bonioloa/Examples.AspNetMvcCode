namespace Comunica.ProcessManager.Web.Models;

public class UserChangePasswordResultViewModel : IOperationResultViewModel
{
    public DateTime Expiration { get; set; }



    //inherited properties
    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldsToWarn { get; set; }
    public string ValuesAllowed { get; set; }
}
