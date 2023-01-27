namespace Comunica.ProcessManager.Web.Models;

/// <summary>
/// grid view state data
/// </summary>
public class DataGridViewStateJsonModel : IEquatable<DataGridViewStateJsonModel>
{

    /// <summary>
    /// unique identifier for state
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// user description assigned by user (hopefully comprehensible for other users).<br/>
    /// We use string for better safety on displaying what user writed
    /// </summary>
    public string UserProvidedDescription { get; set; }

    /// <summary>
    /// serialized json of data grid view state
    /// </summary>
    public string StateSerialized { get; set; }


    /// <summary>
    /// equality
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(DataGridViewStateJsonModel other)
    {
        return (this is null && other is null)
                || (this != null && other != null
                        && Id.Equals(other.Id));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        if (this is null)
        {
            return -1;
        }
        return Id.GetHashCode();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as DataGridViewStateJsonModel);
    }
}