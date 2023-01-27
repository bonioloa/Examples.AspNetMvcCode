namespace Comunica.ProcessManager.Web.Models;

public class ItemSaveResultJsonModel : ISubmitResultJsonModel
{
    public string ResultCode { get; set; }
    public string ErrorMessage { get; set; }
    public string LoginCodeForAnonymousInsert { get; set; }
}