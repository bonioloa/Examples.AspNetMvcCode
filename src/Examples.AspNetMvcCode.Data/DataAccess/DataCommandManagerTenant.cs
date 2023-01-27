namespace Examples.AspNetMvcCode.Data;

/// <summary>
/// represents interaction with tenant db, completely initialized only after token validation 
/// and retrieval of configuration info from root database
/// </summary>
public class DataCommandManagerTenant : DataCommandManager, IDataCommandManagerTenant
{
    private readonly ILogger<DataCommandManagerTenant> _logger;

    private readonly IOptionsSnapshot<DataAccessTenantSettings> _optDataAccessTenantSettings;

    private readonly IDefaultCryptManager _cryptDefaultManager;

    public DataCommandManagerTenant(
        ILogger<DataCommandManagerTenant> logger
        , IOptionsSnapshot<DataAccessTenantSettings> optDataAccessTenantSettings
        , IDefaultCryptManager cryptDefaultManager
        ) : base(logger)
    {
        _logger = logger;
        _optDataAccessTenantSettings = optDataAccessTenantSettings;
        _cryptDefaultManager = cryptDefaultManager;
    }



    public void ValidateAndInitialize(TenantConfigQr tenantConfig)
    {
        Guard.Against.Null(tenantConfig, nameof(TenantConfigQr));

        Guard.Against.NullOrWhiteSpace(tenantConfig.Token);

        Guard.Against.InvalidInput(
            tenantConfig.ConfigType
            , nameof(tenantConfig.ConfigType)
            , (input) => input != ConfigurationType.Missing
            , $"value '{tenantConfig.ConfigType}' is not allowed. Check if {nameof(tenantConfig.ConfigType)} has a valid {nameof(ConfigurationType)}"
            );

        Guard.Against.NullOrWhiteSpace(tenantConfig.DbName, nameof(tenantConfig.DbName));

        Guard.Against.NullOrWhiteSpace(tenantConfig.DbLogin, nameof(tenantConfig.DbLogin));

        Guard.Against.NullOrWhiteSpace(tenantConfig.DbPassword, nameof(tenantConfig.DbPassword));


        Guard.Against.Null(_optDataAccessTenantSettings, nameof(IOptionsSnapshot<DataAccessTenantSettings>));

        Guard.Against.Null(_optDataAccessTenantSettings.Value, nameof(DataAccessTenantSettings));

        DataCommandUtility.Validate(_optDataAccessTenantSettings.Value);


        base._dataAccessProperties =
            new DataAccessProperties(
                SqlCommandTimeoutInSeconds: _optDataAccessTenantSettings.Value.SqlCommandTimeoutInSeconds
                , EnableAllCommandLogging: _optDataAccessTenantSettings.Value.EnableAllCommandLogging
                , DisableAllCommandLogging: _optDataAccessTenantSettings.Value.DisableAllCommandLogging
                , SqlConnectionStringBuilder:
                        new SqlConnectionStringBuilder()
                        {
                            DataSource =
                                DataCommandUtility.ReplaceInstanceWithEnvironmentSetting(
                                    _cryptDefaultManager.DecryptAndCleanString(
                                        _optDataAccessTenantSettings.Value.SqlInstance
                                        )
                                    ),
                            InitialCatalog = tenantConfig.DbName,
                            UserID = tenantConfig.DbLogin,
                            Password = tenantConfig.DbPassword,
                            IntegratedSecurity = false,
                            MultipleActiveResultSets = true,
                        }
                , IsolationLevel: _optDataAccessTenantSettings.Value.IsolationLevel
                );
    }

}