namespace Examples.AspNetMvcCode.Localization;

/// <summary>
/// map in this class all translations for controller action/string
/// Not used because referencing class is disabled
/// </summary>
public class TranslationDatabase
{
    private static readonly IDictionary<string, string> BaseTranslation =
        new Dictionary<string, string>()
            {
                    //key: translation of controller/action, value: controller/action name as defined in code
                    { "translated mvc component (action/controller)", "code defined controller/action" },
            }
        .ToLowerInvariant();//we normalize all route components to lower case


    //same url translation for all languages, we use the basic MVC controller action
    //given by controller class name and methods (actions)
    private static readonly IDictionary<string, IDictionary<string, string>>
        Translations =
            new Dictionary<string, IDictionary<string, string>>
            {
                {SupportedCulturesConstants.IsoCodeItalian, BaseTranslation},
                {SupportedCulturesConstants.IsoCodeEnglish, BaseTranslation},
                {SupportedCulturesConstants.IsoCodeSpanish, BaseTranslation},
            };


    public async Task<string> ResolveAsync(string lang, string value)
    {
        return await Task.Run(() => ResolveSync(lang, value)).ConfigureAwait(false);
    }


    /// <summary>
    /// currently there is no need to translate url.
    /// If needed implement this method and uncomment directive in 
    /// Startup class
    /// UseEndpoints directive
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private string ResolveSync(string lang, string value)
    {
        throw new NotImplementedException($"{nameof(Translations)} dictionary must be filled up");
        //var normalizedLang = lang.ToLowerInvariant();
        //var normalizedValue = value.ToLowerInvariant();
        //if (Translations.ContainsKey(normalizedLang) 
        //    && Translations[normalizedLang].ContainsKey(normalizedValue))
        //{
        //    return Translations[normalizedLang][normalizedValue];
        //}
        //else
        //    return null;
    }
}