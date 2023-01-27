namespace Examples.AspNetMvcCode.Web.Models;

public interface IOperationResultViewModel
{
    IList<MessageField> FieldToWarnList { get; set; }
    string LocalizedMessage { get; set; }
    string LocalizedTitle { get; set; }
    bool Success { get; set; }
    string ValuesAllowed { get; set; }
    WarningType WarningType { get; set; }
}