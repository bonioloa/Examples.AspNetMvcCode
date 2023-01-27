namespace Examples.AspNetMvcCode.Logic;
public class FormConfigLgc : IEquatable<FormConfigLgc>
{
    public string FormCode { get; set; }
    public List<FieldLgc> FieldBaseConfigList { get; set; } = new();
    public bool SaveOptionsDescriptionInsteadOfValue { get; set; }
    public bool HasMultipleSections { get; set; }

    /// <summary>
    /// equality
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(FormConfigLgc other)
    {
        return (this is null && other is null)
                || (this != null && other != null
                        && FormCode.Clean().EqualsInvariant(other.FormCode.Clean()));
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
        return FormCode.Clean()
                       .GetHashCode(StringComparison.InvariantCultureIgnoreCase);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as FormConfigLgc);
    }
}