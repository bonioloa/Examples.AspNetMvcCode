namespace Comunica.ProcessManager.Web.Models;

public class ReportExportDataModel
{
    public IList<long> RootProcessIdList { get; set; }
    public IList<ReportItemBasicDataModel> ItemsBasicDataList { get; set; }
    public IList<ItemsFormsByProcessModel> ItemsFormsByProcessList { get; set; }
}