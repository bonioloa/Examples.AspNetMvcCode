namespace Comunica.ProcessManager.Web.Models;

public class RouteViewModel
{
    public string Controller { get; set; }
    public string Action { get; set; }
    public IDictionary<string, string> QueryStringValues { get; set; }
}
