namespace Comunica.ProcessManager.Web.Code;

public interface IReportingDataTableWeb
{
    DataTable BuildFieldsTable(ItemFormViewModel itemFormSubmitViewModel);
    DataTable BuildFileInfoTable(CultureInfo culture, DateTime creationTimestamp);
    DataTable BuildPrimaryDataTable(CultureInfo culture, ItemFormViewModel itemFormSubmitViewModel);
    HashSet<ReportingAreaSimpleModel> GetBasicDataSchemasByProcess(IList<ReportItemBasicDataModel> reportItemBasicDataList, bool hasItemEndEditableField);
    DataTable GetDataTableFiltersUsed(ReportExportDataModel reportDataDumpModel, bool getForAllAvailableProcesses, string dateSubmitFromStr, string dateSubmitToStr, bool hasItemEndEditableField, string dateExpirationFromStr, string dateExpirationToStr);
    ReportingAreaSimpleModel GetFormSchema(IHtmlContent stepDescription, ItemFormDisplayBasicModel itemForm);
    ReportingAreaSimpleModel GetFormsDataTableSchema(ItemsFormsByFormCodeModel stepFormDataModel);
    ReportingAreaSimpleModel GetPrimaryDataTableSchema(ReportAreaDataToLoad reportAreaDataToLoad, IList<ReportItemBasicDataModel> reportItemPrimaryInfo, bool hasItemEndEditableField);
    DataRow MapRowFormStep(DataTable formModelTable, IEnumerable<ItemFormDisplayBasicModel> itemFormDataModelList);
    DataRow MapRowItemBasicData(DataRow row, ReportItemBasicDataModel reportItemInfo, bool hasItemEndEditableField);
    DataRow MapRowItemPrimaryData(DataTable table, ReportItemBasicDataModel reportItemInfo, bool hasItemEndEditableField);
}