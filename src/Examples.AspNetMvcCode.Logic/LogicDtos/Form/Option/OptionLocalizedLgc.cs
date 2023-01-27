namespace Examples.AspNetMvcCode.Logic;
public class OptionLocalizedLgc : IEquatable<OptionLocalizedLgc>
{
    public long Progressive { get; set; }
    public string Value { get; set; }
    public HashSet<LocalizationLgc> Localizations { get; set; } = new();
    public string ImagePath { get; set; }
    public string ColorValue { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }


    // <summary>
    /// equality
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(OptionLocalizedLgc other)
    {
        return (this is null && other is null)
                || (this != null && other != null &&
                    Value.Clean().EqualsInvariant(other.Value.Clean())
                    //&& Localizations.Equals(other.Localizations)
                    && DateStart == other.DateStart
                    && DateEnd == other.DateEnd);
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
        return (
            Value.Clean()
            //+ JsonSerializer.Serialize(Localizations is null ? new() : Localizations)
            + DateStart.ToString("O")
            + DateEnd.ToString("O")
            )
            .GetHashCode(StringComparison.InvariantCultureIgnoreCase);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as OptionLocalizedLgc);
    }
}