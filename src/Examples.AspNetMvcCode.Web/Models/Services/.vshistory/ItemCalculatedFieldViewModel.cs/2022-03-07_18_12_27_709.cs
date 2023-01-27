namespace Comunica.ProcessManager.Web.Models;

public class ItemCalculatedFieldViewModel : IEquatable<ItemCalculatedFieldViewModel>
{
    public string FieldName { get; set; }
    public string Value { get; set; }
    public IHtmlContent Description { get; set; }
    public int Progressive { get; set; }

    /// <summary>
    /// equality override
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as ItemCalculatedFieldViewModel);
    }

    /// <summary>
    /// equality implementation
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ItemCalculatedFieldViewModel other)
    {
        return FieldName.Clean().EqualsInvariant(other.FieldName);
    }

    /// <summary>
    /// for hashtable intended implementation
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return FieldName.Clean().GetHashCode();
    }
}