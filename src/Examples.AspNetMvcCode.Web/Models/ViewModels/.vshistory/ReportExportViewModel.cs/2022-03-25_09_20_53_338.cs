namespace Comunica.ProcessManager.Web.Models;

public class ReportExportViewModel : IOperationResultViewModel
{
    public bool ShowMessageNoResultsFound { get; set; }
    public bool FilterPreloadCall { get; set; }

    public bool AllProcesses { get; set; }
    public IList<OptionViewModel> ProcessSelect { get; set; }
    public bool AllLinkedProcesses { get; set; }
    public IList<OptionViewModel> ProcessLinkedSelect { get; set; }
    public DateTime? DateSubmitFrom { get; set; }
    public DateTime? DateSubmitTo { get; set; }
    public bool EnableDateExpirationFilter { get; set; }
    public DateTime? DateExpirationFrom { get; set; }
    public DateTime? DateExpirationTo { get; set; }
    public bool LinkedItemsToResults { get; set; }

    public string DataSourceGrid { get; set; }
    //public long? ProcessId { get; set; }
    public HashSet<ReportingColumnFeaturesModel> ColumnsOptionsList { get; set; }
    public string TenantImage { get; set; }


    //inherited properties
    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldsToWarn { get; set; }
    public string ValuesAllowed { get; set; }
}
