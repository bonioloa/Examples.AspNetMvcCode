namespace Examples.AspNetMvcCode.Localization;

public static class SupportedCulturesConstants
{
    public const string IsoCodeItalian = "it";
    public const string IsoCodeEnglish = "en";
    public const string IsoCodeSpanish = "es";

    public const string IsoCodeDefault = IsoCodeItalian;


    internal static readonly CultureInfo CultureItalian = new(IsoCodeItalian);
    internal static readonly CultureInfo CultureEnglish = new(IsoCodeEnglish);
    internal static readonly CultureInfo CultureSpanish = new(IsoCodeSpanish);

    public static readonly CultureInfo CultureDefault = CultureItalian;


    private static readonly CultureInfo[] SupportedCulturesArr = { CultureItalian, CultureEnglish, CultureSpanish };
    private static readonly ReadOnlyCollection<CultureInfo> SupportedCulturesReadonly = Array.AsReadOnly(SupportedCulturesArr);
    /// <summary>
    /// returns a <see cref="ReadOnlyCollection{T}"/> of <see cref="CultureInfo"/> supported by application boxed in a <see cref="IList{T}"/> to be easier to handle
    /// </summary>
    public static IList<CultureInfo> ConfiguredCultures
    {
        get
        {
            return SupportedCulturesReadonly;
        }
    }
}