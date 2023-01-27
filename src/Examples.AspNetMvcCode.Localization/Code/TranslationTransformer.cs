namespace Examples.AspNetMvcCode.Localization;

/// <summary>
/// at this moment this class is not used for endpoints. 
/// If requirements change, enable it on "Startup" -> "UseEndpoints"
/// and map translation in referenced service class
/// </summary>
public class TranslationTransformer : DynamicRouteValueTransformer
{
    private readonly TranslationDatabase _translationDatabase;

    public TranslationTransformer(TranslationDatabase translationDatabase)
    {
        _translationDatabase = translationDatabase;
    }



    public override async ValueTask<RouteValueDictionary> TransformAsync(
        HttpContext httpContext
        , RouteValueDictionary values
        )
    {
        string language = (string)values[RouteParams.Language];

        if (language.Empty()
            || !SupportedCulturesConstants.ConfiguredCultures
                    .Any(sc => sc.TwoLetterISOLanguageName.EqualsInvariant(language)))
        {
            //override language if unknown
            language = SupportedCulturesConstants.IsoCodeDefault;
            values[RouteParams.Language] = language;
        }


        if (!values.ContainsKey(RouteParams.Language)
            || !values.ContainsKey(RouteParams.Controller)
            || !values.ContainsKey(RouteParams.Action))
        {
            return values;
        }


        string controller =
            await _translationDatabase
                .ResolveAsync(language, (string)values[RouteParams.Controller])
                .ConfigureAwait(false);

        if (controller == null)
        {
            return values;
        }


        values[RouteParams.Controller] = controller;


        string action =
            await _translationDatabase
                .ResolveAsync(language, (string)values[RouteParams.Action])
                .ConfigureAwait(false);

        if (action == null)
        {
            return values;
        }


        values[RouteParams.Action] = action;

        return values;
    }
}