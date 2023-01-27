namespace Examples.AspNetMvcCode.Web.Models;

public class OperationResultViewModel : IOperationResultViewModel
{
    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }


    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldToWarnList { get; set; } = new List<MessageField>();
    public string ValuesAllowed { get; set; }
}