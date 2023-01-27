namespace Comunica.ProcessManager.Web.Code;

public interface IReportingDocumentWeb
{
    ReportAdvancedModel BuildReportAdvanced(ReportExportDataModel reportDataDumpModel, bool hasItemEndEditableField);
}