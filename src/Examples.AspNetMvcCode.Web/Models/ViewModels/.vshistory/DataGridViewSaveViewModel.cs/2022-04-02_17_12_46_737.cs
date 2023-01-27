namespace Comunica.ProcessManager.Web.Models;

public class DataGridViewSaveViewModel
{
    public GridViewUsage Type { get; set; }
    public string UserProvidedDescription { get; set; }
    public bool SaveAlsoForAllProfiles { get; set; }
    public string StateSerialized { get; set; }
}