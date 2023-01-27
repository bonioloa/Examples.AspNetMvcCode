namespace Comunica.ProcessManager.Web.Models;

public class DataGridSaveViewResultJsonMode : ISubmitResultJsonModel
{
    public List<OptionViewModel> SavedViewList { get; set; }
    //interface implementation
    public string ResultCode { get; set; }
    public string ErrorMessage { get; set; }
}