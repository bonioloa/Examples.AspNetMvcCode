namespace Examples.AspNetMvcCode.Web.Models;

public class DataTablesNetViewModel
{
    public IHtmlContent ColumnsJsonObjectSerialized { get; set; }
    public IHtmlContent RowsJsonObjectSerialized { get; set; }
    public IDictionary<string, int> AvailableColumnsIndexes { get; set; } = new Dictionary<string, int>();
    public IHtmlContent ColumnDefSimpleTargetsSerialized { get; set; }
    public IHtmlContent ColumnDefaultOrderingModelSerialized { get; set; }
}