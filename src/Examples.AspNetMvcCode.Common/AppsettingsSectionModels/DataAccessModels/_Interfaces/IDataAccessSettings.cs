namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// encloses properties needed for database settings types 
/// </summary>
public interface IDataAccessSettings
{
    public string SqlInstance { get; init; }

    /// <summary>
    /// Define isolation level for transaction
    /// </remarks>
    public IsolationLevel IsolationLevel { get; init; }

    /// <summary>
    /// cutoff for command execution
    /// </summary>
    public int SqlCommandTimeoutInSeconds { get; init; }

    /// <summary>
    /// with this at true all queries will be written to log, no matter the single query configuration
    /// </summary>
    /// <remarks>don't set true this and also <see cref="DisableAllCommandLogging"/> , an exception will be thrown</remarks>
    public bool EnableAllCommandLogging { get; init; }

    /// <summary>
    /// if true all queries will not be written to log, no matter the single query configuration
    /// </summary>
    /// <remarks>don't set true this and also <see cref="EnableAllCommandLogging"/>, an exception will be thrown</remarks>
    public bool DisableAllCommandLogging { get; init; }
}