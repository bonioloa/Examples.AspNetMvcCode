namespace Examples.AspNetMvcCode.Data;

/// <summary>
/// utility model class useful for queuing
/// Internal because the use is only reserved for namespace Data
/// </summary>
public class CommandExecutionDb
{
    /// <summary>
    /// Specify only if command is not a plain text query.
    /// See <see cref="System.Data.CommandType"/> for available types
    /// </summary>
    internal CommandType CommandType { get; set; } = CommandType.Text;

    /// <summary>
    /// query SQL code or function/stored procedure name, depends on <see cref="CommandType"/>
    /// </summary>
    internal string CommandText { get; set; }

    /// <summary>
    /// parameters for command
    /// </summary>
    internal HashSet<CommandParameterDb> Parameters { get; set; } = new();

    /// <summary>
    /// enable this flag to write a log for <see cref="CommandText"/> and <see cref="Parameters"/> values.
    /// </summary>
    /// <remarks>Commands enqueued in a batch will always write a log, overriding of this setting</remarks>
    internal bool WriteCommandLog { get; set; }
}