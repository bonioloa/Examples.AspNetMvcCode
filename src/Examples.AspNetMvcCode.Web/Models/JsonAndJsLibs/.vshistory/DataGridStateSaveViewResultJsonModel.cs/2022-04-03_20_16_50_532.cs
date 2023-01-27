namespace Comunica.ProcessManager.Web.Models;

public class DataGridStateSaveViewResultJsonModel : ISubmitResultJsonModel
{
    public List<OptionViewModel> SavedDataGridViewList { get; set; }
    //interface implementation
    public string ResultCode { get; set; }
    public string ErrorMessage { get; set; }
}