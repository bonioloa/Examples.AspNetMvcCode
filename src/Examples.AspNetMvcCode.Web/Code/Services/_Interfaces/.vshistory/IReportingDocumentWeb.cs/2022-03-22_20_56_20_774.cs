namespace Comunica.ProcessManager.Web.Code;

public interface IReportingDocumentWeb
{
    FileDownloadInfoModel BuildExcelDumpForStep(ItemFormViewModel itemFormSubmitViewModel, string formStepCode);
    FileDownloadInfoModel BuildExcelReport(ReportExportDataModel reportDataDumpModel, bool getForAllAvailableProcesses, string dateSubmitFrom, string dateSubmitTo, bool hasItemEndEditableField, string dateExpirationFromFilter, string dateExpirationToFilter);
    ReportAdvancedModel BuildReportAdvanced(ReportExportDataModel reportDataDumpModel, bool hasItemEndEditableField);
}