namespace Examples.AspNetMvcCode.Data;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// service initialization for this library
    /// </summary>
    /// <param name="services"></param>
    public static void AddDataLayerServices(this IServiceCollection services)
    {
        services.AddDatabaseInfrastructure();
        services.AddDataServices();
        services.AddDataUnitOfWorkServices();
    }

    /// <summary>
    /// add service implementations for database infrastructure
    /// </summary>
    /// <param name="services"></param>
    private static void AddDatabaseInfrastructure(this IServiceCollection services)
    {
        //crypt manager for root db
        services.AddTransient<IDefaultCryptManager, DefaultCryptManager>();

        //scoped to keep tenant crypting config for web call duration
        services.AddScoped<ITenantCryptManager, TenantCryptManager>();

        //we do not need batch command for this db type, so we can use transient safely
        services.AddTransient<IDataCommandManagerRoot, DataCommandManagerRoot>();

        //scoped to keep alive queue of commands for web call duration
        services.AddScoped<IDataCommandManagerTenant, DataCommandManagerTenant>();

        //services.AddTransient<,>();
    }


    /// <summary>
    /// adds data services implementations
    /// </summary>
    /// <param name="services"></param>
    private static void AddDataServices(this IServiceCollection services)
    {
        //add services here following folder structure of classes
        services.AddTransient<ISsoQueries, SsoQueries>();
        services.AddTransient<IPersonalizationQueries, PersonalizationQueries>();

        services.AddTransient<IAuditEventWriteQueries, AuditEventWriteQueries>();
        services.AddTransient<IAuditFileWriteQueries, AuditFileWriteQueries>();
        services.AddTransient<IAuditReadQueries, AuditReadQueries>();

        services.AddTransient<ICalculationRuleQueries, CalculationRuleQueries>();
        services.AddTransient<ICalculationExecutionItemFormQueries, CalculationExecutionItemFormQueries>();
        services.AddTransient<ICalculationExecutionForExpirationQueries, CalculationExecutionForExpirationQueries>();

        services.AddTransient<ICompanyGroupQueries, CompanyGroupQueries>();

        services.AddTransient<IDataGridReadQueries, DataGridReadQueries>();
        services.AddTransient<IDataGridWriteQueries, DataGridWriteQueries>();

        services.AddTransient<IEmailQueries, EmailQueries>();

        services.AddTransient<IExpirationQueries, ExpirationQueries>();

        services.AddTransient<IFileAttachmentReadQueries, FileAttachmentReadQueries>();
        services.AddTransient<IFileAttachmentWriteQueries, FileAttachmentWriteQueries>();

        services.AddTransient<IFieldsConfigurationQueries, FieldsConfigurationQueries>();
        services.AddTransient<IFieldsLogicsMappingQueries, FieldsLogicsMappingQueries>();
        services.AddTransient<IFormTableReadQueries, FormTableReadQueries>();
        services.AddTransient<IFormTableWriteQueries, FormTableWriteQueries>();
        services.AddTransient<IOptionsQueries, OptionsQueries>();

        services.AddTransient<IIdentityDisclosureReadQueries, IdentityDisclosureReadQueries>();
        services.AddTransient<IIdentityDisclosureWriteQueries, IdentityDisclosureWriteQueries>();

        services.AddTransient<IItemAggregationQueries, ItemAggregationQueries>();
        services.AddTransient<IItemCalculationRuleQueries, ItemCalculationRuleQueries>();
        services.AddTransient<IItemQueries, ItemQueries>();
        services.AddTransient<IItemWriteQueries, ItemWriteQueries>();

        services.AddTransient<IMessageReadQueries, MessageReadQueries>();
        services.AddTransient<IMessageWriteQueries, MessageWriteQueries>();

        services.AddTransient<IParametersQueries, ParametersQueries>();

        services.AddTransient<IPermissionItemReadQueries, PermissionItemReadQueries>();
        services.AddTransient<IPermissionWriteQueries, PermissionWriteQueries>();
        services.AddTransient<IPermissionProcessReadQueries, PermissionProcessReadQueries>();
        services.AddTransient<IPermissionInclusionReadQueries, PermissionInclusionReadQueries>();

        services.AddTransient<IProcessQueries, ProcessQueries>();
        services.AddTransient<IProcessLinkedQueries, ProcessLinkedQueries>();

        services.AddTransient<IProgressiveWriteQueries, ProgressiveWriteQueries>();

        services.AddTransient<IReportFieldCalcQueries, ReportFieldCalcQueries>();

        services.AddTransient<IRoleAdminManagedHistoryWriteQueries, RoleAdminManagedHistoryWriteQueries>();
        services.AddTransient<IRoleReadQueries, RoleReadQueries>();
        services.AddTransient<IRoleWriteQueries, RoleWriteQueries>();

        services.AddTransient<IRootQueries, RootQueries>();

        services.AddTransient<IUserAdminManagedHistoryWriteQueries, UserAdminManagedHistoryWriteQueries>();
        services.AddTransient<IUserAdminManagedWriteQueries, UserAdminManagedWriteQueries>();
        services.AddTransient<IUserDataReadQueries, UserDataReadQueries>();
        services.AddTransient<IUserReadQueries, UserReadQueries>();
        services.AddTransient<IUserWriteQueries, UserWriteQueries>();
        services.AddTransient<IUserNotificationQueries, UserNotificationQueries>();
        //services.AddTransient<,>();
    }


    /// <summary>
    /// adds tenant unit of work services implementations
    /// </summary>
    /// <param name="services"></param>
    private static void AddDataUnitOfWorkServices(this IServiceCollection services)
    {
        services.AddTransient<IAuditEventUow, AuditEventUow>();
        services.AddTransient<IAuditFileUow, AuditFileUow>();

        services.AddTransient<IDataGridUow, DataGridUow>();

        services.AddTransient<IFileAttachmentUow, FileAttachmentUow>();

        services.AddTransient<IItemFormUow, ItemFormUow>();

        services.AddTransient<IIdentityDisclosureUow, IdentityDisclosureUow>();

        services.AddTransient<IItemNewUow, ItemNewUow>();
        services.AddTransient<IItemEditUow, ItemEditUow>();
        services.AddTransient<IItemUsersChatUow, ItemUsersChatUow>();

        services.AddTransient<IPermissionUow, PermissionUow>();

        services.AddTransient<IRoleUow, RoleUow>();

        services.AddTransient<IUserUow, UserUow>();
        services.AddTransient<IUserRoleAdminManagedUow, UserRoleAdminManagedUow>();
        //services.AddTransient<,>();
    }
}