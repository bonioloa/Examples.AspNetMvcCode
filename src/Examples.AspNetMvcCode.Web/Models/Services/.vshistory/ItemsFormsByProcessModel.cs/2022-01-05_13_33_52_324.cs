namespace Comunica.ProcessManager.Web.Models;

public class ItemsFormsByProcessModel
{
    public long ProcessId { get; set; }
    public IList<ItemsFormsByFormCodeModel> ItemsFormsByFormCodeList { get; set; }
}
