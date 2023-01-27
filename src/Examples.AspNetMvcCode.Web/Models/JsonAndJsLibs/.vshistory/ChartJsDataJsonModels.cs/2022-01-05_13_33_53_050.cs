namespace Comunica.ProcessManager.Web.Models;

//models for chart js json config creation
public class Dataset
{
    public IList<string> Data { get; set; }
    public IList<string> Labels { get; set; }
}
public class Data
{
    public IList<Dataset> Datasets { get; set; }
    public IList<string> Labels { get; set; }
}
