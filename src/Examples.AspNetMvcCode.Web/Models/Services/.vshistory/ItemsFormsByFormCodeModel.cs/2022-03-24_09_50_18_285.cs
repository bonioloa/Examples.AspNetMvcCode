namespace Comunica.ProcessManager.Web.Models;

public class ItemsFormsByFormCodeModel
{
    public string FormCode { get; set; }
    public long StepProgressive { get; set; }
    public IHtmlContent StepDescription { get; set; }
    public IList<ItemFormDisplayBasicModel> ItemFormDisplayBasicList { get; set; }
}