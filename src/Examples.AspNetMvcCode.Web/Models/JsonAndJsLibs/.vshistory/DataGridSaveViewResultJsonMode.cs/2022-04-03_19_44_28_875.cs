namespace Comunica.ProcessManager.Web.Models;

public class DataGridSaveViewResultJsonMode : ISubmitResultJsonModel
{

    //interface implementation
    public string ResultCode { get; set; }
    public string ErrorMessage { get; set; }
}