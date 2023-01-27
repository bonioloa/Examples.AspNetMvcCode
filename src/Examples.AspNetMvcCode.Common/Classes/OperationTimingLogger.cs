using Serilog;
using System.Diagnostics;

namespace Examples.AspNetMvcCode.Common;

/// <summary>
/// classe temporanea sostituibile con attributo wrapper con PostSharp (al momento PostSharp non supporta .NET 7 ed è necessario acquistarlo)
/// </summary>
public class OperationTimingLogger
{
    private readonly string _logMessage = string.Empty;
    private readonly object[] _logMessageParams = Array.Empty<object>();


    protected readonly Stopwatch timer = new();

    /// <summary>
    /// start timer
    /// </summary>
    /// <param name="structuredMessage"></param>
    /// <param name="logMessageParams"></param>
    public OperationTimingLogger(string structuredMessage, params object[] logMessageParams)
    {
        _logMessage = structuredMessage ?? string.Empty;
        _logMessageParams = logMessageParams ?? Array.Empty<object>();
        timer.Start();
    }

    public OperationTimingLogger(string logMessage) :
        this(logMessage, Array.Empty<object>())
    {
    }

    /// <summary>
    /// call this method to log elapsed time for this instance
    /// </summary>
    [SuppressMessage("CodeQuality", "Serilog004:Constant MessageTemplate verifier"
        , Justification = "we need to include dynamically the elapsed timing in both message template and message params")]
    public void LogCompletion()
    {
        if (timer is null || !timer.IsRunning || Log.Logger is null)
        {
            return;
        }

        timer.Stop();

        string message = _logMessage + " | Completion: {TotalSeconds} s; {TotalMilliseconds} ms";
        TimeSpan timeSpan = timer.Elapsed;
        List<object> messageParams = _logMessageParams.ToList();

        // When measuring small time periods the StopWatch.Elapsed*  properties can return negative values.
        // This is due to bugs in the basic input/output system (BIOS) or the hardware abstraction layer
        // (HAL) on machines with variable-speed CPUs (e.g. Intel SpeedStep).
        messageParams.Add(timeSpan.TotalSeconds < 0 ? 0 : timeSpan.TotalSeconds);
        messageParams.Add(timeSpan.TotalMilliseconds < 0 ? 0 : timeSpan.TotalMilliseconds);

        Log.Logger.Information(message, messageParams.ToArray());
    }
}
