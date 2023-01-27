namespace Comunica.ProcessManager.Web.Models;

public class FileDownloadInfoModel
{
    public byte[] FileContents { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}
