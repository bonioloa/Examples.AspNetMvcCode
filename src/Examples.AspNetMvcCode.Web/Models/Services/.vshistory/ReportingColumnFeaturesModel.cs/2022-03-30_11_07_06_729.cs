namespace Comunica.ProcessManager.Web.Models;

public class ReportingColumnFeaturesModel : IEquatable<ReportingColumnFeaturesModel>
{
    public string ColumnName { get; set; }
    public string SerializedOptionValues { get; set; }
    public string SerializedOptionColors { get; set; }
    public int Progressive { get; set; }
    public bool Visible { get; set; }



    /// <summary>
    /// equality
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ReportingColumnFeaturesModel other)
    {
        //IMPORTANT: do not clean ColumnName. Eventual trailing whitespaces are necessary to prevent column name duplications
        return (this is null && other is null)
                || (this != null && other != null 
                    && ColumnName.Clean().EqualsInvariant(other.ColumnName.Clean()));
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
        return ColumnName.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ReportingColumnFeaturesModel);
    }
}