namespace Comunica.ProcessManager.Web.Models;

public class ItemLinkedByProcessViewModel : IEquatable<ItemLinkedByProcessViewModel>
{
    public long ProcessLinkedId { get; set; }
    public IHtmlContent ProcessLinkedDescription { get; set; }
    public HashSet<ItemLinkedViewModel> ItemLinkedList { get; set; }
    public ProcessLinkedRuleViewModel ProcessLinkedRule { get; set; }
    public bool AddNewLinkedItemIsDisabled { get; set; }


    /// <summary>
    /// equality
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ItemLinkedByProcessViewModel other)
    {
        return ProcessLinkedId == other.ProcessLinkedId;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return ProcessLinkedId.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ItemLinkedByProcessViewModel);
    }
}