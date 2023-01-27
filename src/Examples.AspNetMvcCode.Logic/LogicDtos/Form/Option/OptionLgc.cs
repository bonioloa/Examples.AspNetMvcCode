namespace Examples.AspNetMvcCode.Logic;

/// <summary>
/// generic object for option choice representation.
/// Each of this objects is distinguished by <see cref="Value"/>
/// </summary>
public class OptionLgc : IEquatable<OptionLgc>
{
    public bool Selected { get; set; }

    /// <summary>
    /// this value must be unique in respect of other objects of the same type
    /// </summary>
    public string Value { get; set; }

    [JsonConverter(typeof(IHtmlContentConverter))]
    public IHtmlContent Description { get; set; }

    /// <summary>
    /// in case the option must render a image instead of a description
    /// </summary>
    public string ImagePath { get; set; }

    /// <summary>
    /// in case this object must be rendered only for display and not selectable
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// color associated to this object
    /// </summary>
    public string ColorValue { get; set; }


    /// <summary>
    /// equality
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(OptionLgc other)
    {
        return (this is null && other is null)
                || (this != null && other != null
                        && Value.Clean()
                                .EqualsInvariant(other.Value.Clean()));
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
        return Value.Clean()
                    .GetHashCode(StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as OptionLgc);
    }
}