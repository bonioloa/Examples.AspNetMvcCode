namespace Comunica.ProcessManager.Web.Code;

public interface IExpirationWeb
{
    string BuildMessageForItemManagement(DateTimeSpan expirationCompareResult);
    string BuildMessageForItemSearch(DateTimeSpan expirationCompareResult);
}
