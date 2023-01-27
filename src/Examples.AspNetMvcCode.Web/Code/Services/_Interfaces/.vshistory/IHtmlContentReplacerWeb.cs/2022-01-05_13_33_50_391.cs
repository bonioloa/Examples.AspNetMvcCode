namespace Comunica.ProcessManager.Web.Code;

public interface IHtmlContentReplacerWeb
{
    IHtmlContent ContentStandardizePathAndRewriteUrlsForHtmlStrings(IHtmlContent contentWithUrls);
}
