namespace Comunica.ProcessManager.Web.Models;

public class ReportingAreaSimpleModel : IEquatable<ReportingAreaSimpleModel>
{
    //public DataTable Data { get; set; }
    public long ProcessId { get; set; }
    public string ColumnsSuffixPrimary { get; set; }
    public string ColumnsSuffixSecondary { get; set; }
    public List<DataColumn> DataColumns { get; set; }
    public HashSet<ReportingColumnFeaturesModel> ColumnFeatureSet { get; set; }


    /// <summary>
    /// equality
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ReportingAreaSimpleModel other)
    {
        return (this is null && other is null)
                || (this != null && other != null && ProcessId == other.ProcessId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return ProcessId.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ReportingAreaSimpleModel);
    }
}