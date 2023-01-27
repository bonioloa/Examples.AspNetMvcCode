namespace Examples.AspNetMvcCode.Web;

/// <summary>
/// Extensions of <see cref="IServiceCollection"/> for normal application services configuration
/// </summary>
public static class StartupServiceMainExtensions
{
    public static void AddAppLayersServices(this IServiceCollection services)
    {
        services.AddBusinessLogicLayerServices();
        services.AddWebLayerHelperServices();
    }


    private static void AddWebLayerHelperServices(this IServiceCollection services)
    {


        //services.AddTransient<>();
    }



}