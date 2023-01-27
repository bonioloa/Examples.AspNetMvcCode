namespace Comunica.ProcessManager.Web.Models;

public class ReportingAreaSimpleModel
{
    //public DataTable Data { get; set; }
    public long ProcessId { get; set; }
    public string ColumnGroupName { get; set; }
    public List<DataColumn>  DataColumns { get; set; }
    public HashSet<ReportingColumnFeaturesModel> ColumnFeatureSet { get; set; }
}