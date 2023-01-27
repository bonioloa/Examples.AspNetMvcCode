namespace Comunica.ProcessManager.Web.Models;

public class StepFormViewModel
{
    public string FormTableCode { get; set; }
    public bool SavedDataFound { get; set; }
    public IList<InputControlViewModel> InputControls { get; set; }
    public bool SaveOptionsDescriptionInsteadOfValue { get; set; }
    public bool HasMultipleSections { get; set; }
    public IHtmlContent JumpNextStepDescription { get; set; }
    public IHtmlContent JumpToAlternativeStepDescription { get; set; }
    public bool IsDraftEnabled { get; set; }
    public IHtmlContent AbortItemDescription { get; set; }
    public IHtmlContent RollbackItemDescription { get; set; }
}
