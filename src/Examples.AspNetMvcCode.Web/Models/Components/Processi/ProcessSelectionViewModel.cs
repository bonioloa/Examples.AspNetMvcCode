namespace Examples.AspNetMvcCode.Web.Models;

public class ProcessSelectionViewModel
{
    public long SingleProcessDirect { get; set; }
    public IList<FieldViewModel> FieldList { get; set; } = new List<FieldViewModel>();


    //these properties are mapped in viewcomponent code
    public string InsertButtonDescription { get; set; }
}