namespace Comunica.ProcessManager.Web.Models;

public class ItemCalculatedFieldViewModel : IEquatable<ItemCalculatedFieldViewModel>
{
    public string FieldName { get; set; }
    public string Value { get; set; }
    public IHtmlContent Description { get; set; }
    public int Progressive { get; set; }

    /// <summary>
    /// equality implementation
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ItemCalculatedFieldViewModel other)
    {
        return (this is null && other is null)
                || (this != null && other != null
                        && FieldName.Clean().EqualsInvariant(other.FieldName));
    }

    /// <summary>
    /// for hashtable intended implementation
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        if (this is null)
        {
            return -1;
        }
        return FieldName.Clean().GetHashCode();
    }

    /// <summary>
    /// equality override
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as ItemCalculatedFieldViewModel);
    }
}