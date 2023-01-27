namespace Comunica.ProcessManager.Web.Models;

/// <summary>
/// this object represents a group of areas that should be writed in report one beside each other
/// </summary>
public class ReportPositionGridForMasterProcessModel
{
    /// <summary>
    /// will be 0 if steps data have an invariant structure for each process
    /// </summary>
    public long MasterProcessId { get; set; }
    public IList<ReportPositionRowItemModel> ItemsPositioningGrid { get; set; }
}
