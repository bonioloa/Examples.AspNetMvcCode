namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// model for section in appsettings for root database properties
/// </summary>
public class DataAccessRootSettings : IDataAccessSettings
{
    /// <inheritdoc cref="IDataAccessSettings.SqlInstance"/>
    public string SqlInstance { get; init; }

    /// <inheritdoc cref="IDataAccessSettings.IsolationLevel"/>
    public IsolationLevel IsolationLevel { get; init; }

    /// <inheritdoc cref="IDataAccessSettings.SqlCommandTimeoutInSeconds"/>
    public int SqlCommandTimeoutInSeconds { get; init; }

    /// <inheritdoc cref="IDataAccessSettings.EnableAllCommandLogging"/>
    public bool EnableAllCommandLogging { get; init; }

    /// <inheritdoc cref="IDataAccessSettings.DisableAllCommandLogging"/>
    public bool DisableAllCommandLogging { get; init; }


    public string RootDbNameEncrypted { get; init; }
    public string RootDbLoginEncrypted { get; init; }
    public string RootDbPasswordEncrypted { get; init; }
}