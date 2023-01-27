namespace Comunica.ProcessManager.Web.Code;

public interface IReportingDocumentWeb
{
    FileDownloadInfoModel BuildExcelDumpForStep(ItemFormViewModel itemFormSubmitViewModel, string formStepCode);
    ReportAdvancedModel BuildReportAdvanced(ReportExportDataModel reportDataDumpModel, bool hasItemEndEditableField);
}