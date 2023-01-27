namespace Comunica.ProcessManager.Web.Models;

public class ReportExportDataModel
{
    public IList<ReportItemPrimaryInfoModel> ItemsBaseDataList { get; set; }
    //public IList<ReportItemPrimaryInfoModel> ReportLinkedItemPrimaryInfoList { get; set; }
    public IList<ItemsFormsByProcessModel> ItemsFormsByProcess { get; set; }
    //public IList<ItemsFormsByProcessModel> ItemLinkedFormDataByProcess { get; set; }
}