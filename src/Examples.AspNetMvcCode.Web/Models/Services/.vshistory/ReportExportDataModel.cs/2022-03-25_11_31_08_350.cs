namespace Comunica.ProcessManager.Web.Models;

public class ReportExportDataModel
{
    public IList<ReportItemPrimaryInfoModel> ReportItemPrimaryInfoList { get; set; }
    //public IList<ReportItemPrimaryInfoModel> ReportLinkedItemPrimaryInfoList { get; set; }
    public IList<ItemsFormsByProcessModel> ItemFormDataByProcess { get; set; }
    //public IList<ItemsFormsByProcessModel> ItemLinkedFormDataByProcess { get; set; }
}