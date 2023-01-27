namespace Examples.AspNetMvcCode.Localization;

/// <summary>
/// https://stackoverflow.com/questions/38170739/handle-culture-in-route-url-via-requestcultureproviders
/// Custom provider because default for framework has problems
/// </summary>
public class RouteCultureProvider : IRequestCultureProvider
{
    private readonly CultureInfo defaultCulture;
    private readonly CultureInfo defaultUICulture;


    public RouteCultureProvider(RequestCulture requestCulture)
    {
        defaultCulture = requestCulture.Culture;
        defaultUICulture = requestCulture.UICulture;
    }


    public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        //Parsing language from url path, which looks like "<subpath>/en/home/index"
        PathString url = httpContext.Request.Path;

        // Test any culture in route
        if (url.ToString().Length <= 1)
        {
            // Set default Culture and default UICulture
            return
                Task.FromResult(
                    new ProviderCultureResult(
                        defaultCulture.TwoLetterISOLanguageName
                        , defaultUICulture.TwoLetterISOLanguageName)
                    );
        }

        string[] parts = httpContext.Request.Path.Value.Split('/');
        string culture = parts[LocalizationConstants.UrlPathCultureIndex];

        // If the culture is not properly formatted or not valid fallback to default culture
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        if (!Regex.IsMatch(culture, AppRegexPatterns.Culture))
        {
            // Set default Culture and default UICulture
            return
                Task.FromResult(
                    new ProviderCultureResult(
                        defaultCulture.TwoLetterISOLanguageName
                        , defaultUICulture.TwoLetterISOLanguageName
                        )
                    );
        }
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

        // Set Culture and UICulture from route culture parameter
        return
            Task.FromResult(
                new ProviderCultureResult(culture, culture)
                );
    }
}