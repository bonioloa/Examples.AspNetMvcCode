namespace Examples.AspNetMvcCode.Web.Models;

//models for chart js json config creation
public class Dataset
{
    public IList<string> Data { get; set; } = new List<string>();
    public IList<string> Labels { get; set; } = new List<string>();
}
public class Data
{
    public IList<Dataset> Datasets { get; set; } = new List<Dataset>();
    public IList<string> Labels { get; set; } = new List<string>();
}