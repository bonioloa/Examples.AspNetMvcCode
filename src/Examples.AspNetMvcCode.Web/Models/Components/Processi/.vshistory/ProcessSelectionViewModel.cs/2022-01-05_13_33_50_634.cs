namespace Comunica.ProcessManager.Web.Models;

public class ProcessSelectionViewModel
{
    public long SingleProcessDirect { get; set; }
    public IList<InputControlViewModel> InputControlList { get; set; }


    //these properties are mapped in viewcomponent code
    public string InsertButtonDescription { get; set; }
}
