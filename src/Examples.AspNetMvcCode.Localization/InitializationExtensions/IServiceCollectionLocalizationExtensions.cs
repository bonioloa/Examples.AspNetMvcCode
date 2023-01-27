namespace Examples.AspNetMvcCode.Localization;

public static class IServiceCollectionLocalizationExtensions
{
    /// <summary>
    /// custom localization configuration to include in <see cref="IServiceCollection"/> initialization
    /// </summary>
    public static void AddLocalizationByRoute(this IServiceCollection services)
    {
        services.AddLocalization();

        services.Configure<RequestLocalizationOptions>(
            options =>
            {
                CultureInfo[] supportedCultures = SupportedCulturesConstants.ConfiguredCultures.ToArray();

                // State what the default culture for your application is. This will be used if no specific culture
                // can be determined for a given request.
                options.DefaultRequestCulture =
                    new RequestCulture(
                        culture: SupportedCulturesConstants.CultureDefault
                        , uiCulture: SupportedCulturesConstants.CultureDefault
                        );

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
                options.SupportedUICultures = supportedCultures;

                // You can change which providers are configured to determine the culture for requests, or even add a custom
                // provider with your own logic. The providers will be asked in order to provide a culture for each request,
                // and the first to provide a non-null result that is in the configured supported cultures list will be used.
                // By default, the following built-in providers are configured:
                // - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
                // - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
                // - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header
                //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
                //{
                //  // My custom request culture logic
                //  return new ProviderCultureResult("en");
                //}));

                options.RequestCultureProviders.Insert(
                      index: 0
                      , item: new RouteCultureProvider(options.DefaultRequestCulture)
                      );

                //use the custom provider above instead of following
                //options.RequestCultureProviders = new[] {
                //    new RouteDataRequestCultureProvider()
                //    {
                //        Options = options,
                //        RouteDataStringKey = "language",
                //        UIRouteDataStringKey = "language"
                //    }
                //};

                //also other useful links
                //https://andrewlock.net/applying-the-routedatarequest-cultureprovider-globally-with-middleware-as-filters/
                //https://gunnarpeipman.com/aspnet-core-simple-localization/
            });

        services.AddSingleton<TranslationTransformer>();//not used for now, maybe useful in future
        services.AddSingleton<TranslationDatabase>();//not used for now, maybe useful in future

        services.AddLocalizers();
    }

    private static void AddLocalizers(this IServiceCollection services)
    {
        services.AddScoped<IHtmlMainLocalizer, HtmlMainLocalizer>();
        services.AddScoped<IMainLocalizer, MainLocalizer>();
        services.AddScoped<ITemplateLocalizer, TemplateLocalizer>();
        services.AddScoped<IHtmlTemplateLocalizer, HtmlTemplateLocalizer>();
    }
}