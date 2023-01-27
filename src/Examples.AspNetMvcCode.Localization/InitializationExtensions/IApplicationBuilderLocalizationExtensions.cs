namespace Examples.AspNetMvcCode.Localization;

public static class IApplicationBuilderLocalizationExtensions
{
    /// <summary>
    /// custom localization configuration to include in <see cref="IApplicationBuilder"/> initialization
    /// </summary>
    /// <param name="app"></param>
    public static void AddApplicationRouteLocalization(this IApplicationBuilder app)
    {
        //no customization for now, we abstract the call here to make it easily editable in future if needed
        app.UseRequestLocalization();
    }
}