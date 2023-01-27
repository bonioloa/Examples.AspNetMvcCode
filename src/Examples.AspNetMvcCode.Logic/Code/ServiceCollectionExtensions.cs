using Examples.AspNetMvcCode.Logic.LogicServices.User;

namespace Examples.AspNetMvcCode.Logic;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// bind services implementation to interface to make it available in dependency injection container
    /// </summary>
    /// <param name="services"></param>
    public static void AddBusinessLogicLayerServices(this IServiceCollection services)
    {
        services.AddDataLayerServices();//this allows to map data layer services without having to reference data project in upper level project
        services.AddBusinessLogicServices();
    }

    /// <summary>
    /// define here interfaces concrete implementation classes
    /// </summary>
    /// <param name="services"></param>
    private static void AddBusinessLogicServices(this IServiceCollection services)
    {
        //raggruppati per folder
        
        services.AddTransient<ITenantConfiguratorLogic, TenantConfiguratorLogic>();

        services.AddTransient<IEmailLogic, EmailLogic>();
        services.AddTransient<IEmailSendBaseLogic, EmailSendBaseLogic>();
        services.AddTransient<IEmailSendAppNotificationLogic, EmailSendAppNotificationLogic>();
        services.AddTransient<IItemUsersChatLogic, ItemUsersChatLogic>();

        services.AddTransient<IRoleAdminManagedLogic, RoleAdminManagedLogic>();
        services.AddTransient<ISupervisorAdministrationLogic, SupervisorAdministrationLogic>();
        services.AddTransient<ISupervisorSaveLogic, SupervisorSaveLogic>();
        services.AddTransient<ISupervisorSaveChecksLogic, SupervisorSaveChecksLogic>();

        //services.AddTransient<, >();
    }
}