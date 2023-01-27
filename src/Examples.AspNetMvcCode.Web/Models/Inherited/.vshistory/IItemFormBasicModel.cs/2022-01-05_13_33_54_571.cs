namespace Comunica.ProcessManager.Web.Models;

public interface IItemFormBasicModel
{
    public long FormId { get; set; }
    public long IdItem { get; set; }
    public DateTime SubmitDate { get; set; }
    public string SubmitUserName { get; set; }
    public string SubmitUserSurname { get; set; }
}
