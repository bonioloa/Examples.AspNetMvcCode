namespace Comunica.ProcessManager.Web.Models;

public class ItemSearchResultViewModel : IOperationResultViewModel
{
    public bool FilterOnProcesses { get; set; }
    public IList<OptionViewModel> ProcessSelect { get; set; }

    public StepStateGroupType SelectedStepStateGroup { get; set; }
    //not mapped from logic dto
    public IList<OptionViewModel> StepStateGroupSelect { get; set; }
    public bool HasAbortedStateGroup { get; set; }

    public bool FilterOnStep { get; set; }
    public IList<ProcessStepGroupFilterViewModel> StepSelect { get; set; }

    public DateTime? DateSubmitFrom { get; set; }
    public DateTime? DateSubmitTo { get; set; }
    public bool EnableDateExpirationFilter { get; set; }
    public DateTime? DateExpirationFrom { get; set; }
    public DateTime? DateExpirationTo { get; set; }

    public bool ShowResults { get; set; }

    public DataTablesNetViewModel SearchResultsModel { get; set; }



    //inherited properties
    public string LocalizedTitle { get; set; }
    public string LocalizedMessage { get; set; }
    public bool Success { get; set; }
    public WarningType WarningType { get; set; }
    public IList<MessageField> FieldsToWarn { get; set; }
    public string ValuesAllowed { get; set; }
}
