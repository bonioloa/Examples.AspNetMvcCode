namespace Examples.AspNetMvcCode.Data;

internal static class DataCommandUtility
{
    private const string DbInstancePlaceholder = "PlhInstanceFromEnvironmentVariable";
    internal static string ReplaceInstanceWithEnvironmentSetting(string instance)
    {
        string localDbInstance = Environment.GetEnvironmentVariable("ASPNETCORE_LOCAL_DB_INSTANCE");
        if (localDbInstance.Empty())
        {
            localDbInstance = "(LocalDB)\\MSSQLLocalDB";
        }


        if (instance.ContainsInvariant(DbInstancePlaceholder))
        {
            return instance.ReplaceInvariant(DbInstancePlaceholder, localDbInstance);
        }

        return instance;
    }

    internal static void Validate(IDataAccessSettings dataAccessSettings)
    {
        //dataAccessSettings not null should be also validated externally to have a log of the correct original type
        Guard.Against.Null(dataAccessSettings, nameof(IDataAccessSettings));

        Guard.Against.NullOrWhiteSpace(dataAccessSettings.SqlInstance, nameof(dataAccessSettings.SqlInstance));

        Guard.Against.InvalidInput(
            dataAccessSettings.IsolationLevel
            , nameof(dataAccessSettings.IsolationLevel)
            , (input) => input != IsolationLevel.Unspecified
            , $"value '{dataAccessSettings.IsolationLevel}' is not allowed. Provide a specific value of {nameof(IsolationLevel)}"
            );

        Guard.Against.NegativeOrZero(
            dataAccessSettings.SqlCommandTimeoutInSeconds
            , nameof(dataAccessSettings.SqlCommandTimeoutInSeconds)
            );

        if (dataAccessSettings.EnableAllCommandLogging && dataAccessSettings.DisableAllCommandLogging)
        {
            throw new PmDataException($"{nameof(dataAccessSettings.EnableAllCommandLogging)} and {nameof(dataAccessSettings.DisableAllCommandLogging)} can't be both configured at true");
        }
    }
}