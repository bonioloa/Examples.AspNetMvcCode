namespace Comunica.ProcessManager.Web.Models;

/// <summary>
/// this model is defined by DataTables Net javascript library.
/// </summary>
/// <remarks>Must be serialized in camelcase</remarks>
public class DataTablesNetColumnJsonModel
{
    public string Data { get; set; }
    public string Title { get; set; }
    public string ClassName { get; set; }
    public bool Orderable { get; set; }
    public bool Searchable { get; set; }
    public bool Visible { get; set; }
    public int ResponsivePriority { get; set; }
}
