namespace Comunica.ProcessManager.Web.Code;

public interface IReportingDataTableWeb
{
    DataTable BuildFieldsTable(ItemFormViewModel itemFormSubmitViewModel);
    DataTable BuildFileInfoTable(CultureInfo culture, DateTime creationTimestamp);
    DataTable BuildPrimaryDataTable(CultureInfo culture, ItemFormViewModel itemFormSubmitViewModel);
    HashSet<ReportingAreaSimpleModel> GetBasicDataSchemasByProcess(IList<ReportItemBasicDataModel> reportItemBasicDataList, bool hasItemEndEditableField);
    DataTable GetDataTableFiltersUsed(ReportExportDataModel reportDataDumpModel, bool getForAllAvailableProcesses, string dateSubmitFromStr, string dateSubmitToStr, bool hasItemEndEditableField, string dateExpirationFromStr, string dateExpirationToStr);
    ReportingAreaSimpleModel GetFormSchema(IHtmlContent processDescription, IHtmlContent stepDescription, ItemFormDisplayBasicModel itemForm);
    DataRow MapRowForm(DataRow row, IHtmlContent stepDescription, ItemFormDisplayBasicModel itemForm);
    DataRow MapRowItemBasicData(DataRow row, ReportItemBasicDataModel reportItemInfo, bool hasItemEndEditableField);
}