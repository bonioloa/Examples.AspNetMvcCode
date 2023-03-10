namespace Examples.AspNetMvcCode.Web.Models;

public class ReportStatisticsViewModel : IOperationResultViewModel
{
    public IList<OptionViewModel> ProcessSelect { get; set; }
    public DateTime? DateSubmitFrom { get; set; }
    public DateTime? DateSubmitTo { get; set; }
    public bool EnableDateExpirationFilter { get; set; }
    public DateTime? DateExpirationFrom { get; set; }
    public DateTime? DateExpirationTo { get; set; }

    public bool ShowStatisticsChart { get; set; }

    public ChartJsJsonModel JsonStatisticsChart { get; set; }



    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldToWarnList { get; set; }
    public string ValuesAllowed { get; set; }
}