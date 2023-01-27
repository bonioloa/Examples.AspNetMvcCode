namespace Comunica.ProcessManager.Web.Code;

public interface IReportingDataTableWeb
{
    DataTable BuildFieldsTable(ItemFormViewModel itemFormSubmitViewModel);
    DataTable BuildFileInfoTable(CultureInfo culture, DateTime creationTimestamp);
    DataTable BuildPrimaryDataTable(CultureInfo culture, ItemFormViewModel itemFormSubmitViewModel);
    DataTable GetDataTableFiltersUsed(ReportExportDataModel reportDataDumpModel, bool getForAllAvailableProcesses, string dateSubmitFromStr, string dateSubmitToStr, bool hasItemEndEditableField, string dateExpirationFromStr, string dateExpirationToStr);
    ReportingAreaSimpleModel GetFormSchema(IHtmlContent stepDescription, ItemFormDisplayBasicModel itemForm);
    ReportingAreaSimpleModel GetFormsDataTableSchema(ItemsFormsByFormCodeModel stepFormDataModel);
    ReportingAreaSimpleModel GetPrimaryDataTableSchema(ReportAreaDataToLoad reportAreaDataToLoad, IList<ReportItemPrimaryInfoModel> reportItemPrimaryInfo, bool hasItemEndEditableField);
    ReportingAreaSimpleModel GetPrimaryDataTableSchema(bool isRootGroup, IList<ReportItemPrimaryInfoModel> reportItemPrimaryInfo, bool hasItemEndEditableField);
    DataRow MapRowFormStep(DataTable formModelTable, IEnumerable<ItemFormDisplayBasicModel> itemFormDataModelList);
    DataRow MapRowItemPrimaryData(DataTable table, ReportItemPrimaryInfoModel reportItemInfo, bool hasItemEndEditableField);
}