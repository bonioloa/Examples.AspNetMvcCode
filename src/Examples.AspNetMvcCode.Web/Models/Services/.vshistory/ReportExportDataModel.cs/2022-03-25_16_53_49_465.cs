namespace Comunica.ProcessManager.Web.Models;

public class ReportExportDataModel
{
    public IList<long> RootProcessIsList { get; set; }
    public IList<ReportItemBasicDataModel> ItemsBasicDataList { get; set; }
    //public IList<ReportItemPrimaryInfoModel> ReportLinkedItemPrimaryInfoList { get; set; }
    public IList<ItemsFormsByProcessModel> ItemsFormsByProcessList { get; set; }
    //public IList<ItemsFormsByProcessModel> ItemLinkedFormDataByProcess { get; set; }
}