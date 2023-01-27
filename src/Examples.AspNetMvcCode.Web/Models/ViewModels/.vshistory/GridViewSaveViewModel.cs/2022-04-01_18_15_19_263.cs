namespace Comunica.ProcessManager.Web.Models;

public class GridViewSaveViewModel
{
    public GridViewUsage Type { get; set; }
    public string Description { get; set; }
    public bool SaveAlsoForAllProfiles { get; set; }
    public string StateSerialized { get; set; }
}