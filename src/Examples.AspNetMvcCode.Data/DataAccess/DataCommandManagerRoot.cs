namespace Examples.AspNetMvcCode.Data;

/// <summary>
/// represents root database which stores tenant base identification and connection string.
/// Configuration, before token validation, can be retrieved from appsettings
/// </summary>
public class DataCommandManagerRoot : DataCommandManager, IDataCommandManagerRoot
{
    private readonly ILogger<DataCommandManagerRoot> _logger;

    private readonly IOptionsSnapshot<DataAccessRootSettings> _optDataAccessRoot;

    private readonly IDefaultCryptManager _cryptDefaultManager;

    public DataCommandManagerRoot(
        ILogger<DataCommandManagerRoot> logger
        , IOptionsSnapshot<DataAccessRootSettings> optDataAccessRoot
        , IDefaultCryptManager cryptDefaultManager
        ) : base(logger)
    {
        _logger = logger;
        _optDataAccessRoot = optDataAccessRoot;
        _cryptDefaultManager = cryptDefaultManager;


        InitializeFromSettings();//slight violation of DI design, no other way for NOW, TODO try with a factory
    }


    private void InitializeFromSettings()
    {
        Guard.Against.Null(_optDataAccessRoot, nameof(IOptionsSnapshot<DataAccessRootSettings>));

        Guard.Against.Null(_optDataAccessRoot.Value, nameof(_optDataAccessRoot.Value));

        DataCommandUtility.Validate(_optDataAccessRoot.Value);

        Guard.Against.NullOrWhiteSpace(
            _optDataAccessRoot.Value.RootDbNameEncrypted
            , nameof(_optDataAccessRoot.Value.RootDbNameEncrypted)
            );

        Guard.Against.NullOrWhiteSpace(
            _optDataAccessRoot.Value.RootDbLoginEncrypted
            , nameof(_optDataAccessRoot.Value.RootDbLoginEncrypted))
            ;

        Guard.Against.NullOrWhiteSpace(
            _optDataAccessRoot.Value.RootDbPasswordEncrypted
            , nameof(_optDataAccessRoot.Value.RootDbPasswordEncrypted)
            );

        base._dataAccessProperties =
            new DataAccessProperties(
                SqlCommandTimeoutInSeconds: _optDataAccessRoot.Value.SqlCommandTimeoutInSeconds
                , EnableAllCommandLogging: _optDataAccessRoot.Value.EnableAllCommandLogging
                , DisableAllCommandLogging: _optDataAccessRoot.Value.DisableAllCommandLogging
                , SqlConnectionStringBuilder:
                        new SqlConnectionStringBuilder()
                        {
                            DataSource =
                                DataCommandUtility.ReplaceInstanceWithEnvironmentSetting(
                                    _cryptDefaultManager.DecryptAndCleanString(
                                        _optDataAccessRoot.Value.SqlInstance
                                        )
                                    ),
                            InitialCatalog = _cryptDefaultManager.DecryptAndCleanString(_optDataAccessRoot.Value.RootDbNameEncrypted),
                            UserID = _cryptDefaultManager.DecryptAndCleanString(_optDataAccessRoot.Value.RootDbLoginEncrypted),
                            Password = _cryptDefaultManager.DecryptAndCleanString(_optDataAccessRoot.Value.RootDbPasswordEncrypted),
                            IntegratedSecurity = false,
                            MultipleActiveResultSets = true,
                        }
                , IsolationLevel: _optDataAccessRoot.Value.IsolationLevel
                );
    }
}