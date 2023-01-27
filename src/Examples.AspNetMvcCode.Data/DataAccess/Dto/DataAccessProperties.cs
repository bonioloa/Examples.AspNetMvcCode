namespace Examples.AspNetMvcCode.Data;

/// <summary>
/// 
/// </summary>
/// <param name="SqlCommandTimeoutInSeconds"></param>
/// <param name="EnableAllCommandLogging"></param>
/// <param name="DisableAllCommandLogging"></param>
/// <param name="IsolationLevel">
/// Transactions isolation level
/// </param>
/// <param name="SqlConnectionStringBuilder">
/// <see href="https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/connection-strings"/><br/>
/// </param>
internal record DataAccessProperties(
    int SqlCommandTimeoutInSeconds
    , bool EnableAllCommandLogging
    , bool DisableAllCommandLogging
    , IsolationLevel IsolationLevel
    , SqlConnectionStringBuilder SqlConnectionStringBuilder
    );