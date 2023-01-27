namespace Comunica.ProcessManager.Web.Models;

public class ReportingAreaModel
{
    public ReportAreaDataToLoad Type { get; set; }
    internal DataTable Data { get; set; }
    public long ProcessId { get; set; }
    /// <summary>
    /// OPTIONAL: used only for data area
    /// </summary>
    public int StepIndex { get; set; }
    public HashSet<ReportingColumnFeaturesModel> ColumnFeatureSet { get; set; }

    //public static implicit operator ReportingAreaModel(ReportingAreaModel v)
    //{
    //    throw new NotImplementedException();
    //}
}