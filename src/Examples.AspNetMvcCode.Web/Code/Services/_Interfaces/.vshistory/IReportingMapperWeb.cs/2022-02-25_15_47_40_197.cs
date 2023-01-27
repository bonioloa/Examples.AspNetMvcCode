namespace Comunica.ProcessManager.Web.Code;

public interface IReportingMapperWeb
{
    FileDownloadInfoModel MapFile(IList<ReportingAreaModel> dataToLoad, ReportConfigFileLgc reportConfigFileDto, DateTime timeStamp);
    string GetImagePath(ReportConfigAreaImageLgc areaImage);
}
