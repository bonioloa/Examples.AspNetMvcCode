namespace Examples.AspNetMvcCode.Localization;

public static class ContextAppExtensions
{
    public static CultureInfo GetCurrentCulture(this ContextApp contextApp)
    {
        Guard.Against.Null(contextApp, nameof(contextApp));

        return GetFromIsoCode(contextApp.CurrentCultureIsoCode);
    }


    private static CultureInfo GetFromIsoCode(string cultureIsoCode)
    {
        cultureIsoCode = cultureIsoCode.Clean();//prevent null

        return
            cultureIsoCode switch
            {
                SupportedCulturesConstants.IsoCodeItalian => SupportedCulturesConstants.CultureItalian,
                SupportedCulturesConstants.IsoCodeEnglish => SupportedCulturesConstants.CultureEnglish,
                SupportedCulturesConstants.IsoCodeSpanish => SupportedCulturesConstants.CultureSpanish,
                _ => throw new PmCommonException($"{nameof(GetFromIsoCode)} - culture '{cultureIsoCode}' is not supported"),
            };
    }
}