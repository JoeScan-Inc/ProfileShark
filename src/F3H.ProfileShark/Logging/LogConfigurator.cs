using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace F3H.ProfileShark.Logging;

public static class LogConfigurator
{
    public static void ConfigureLogging(string[] commandLineArgs)
    {
        LoggingConfiguration conf = new LoggingConfiguration();
      

        var consoleTarget = new ConsoleTarget("consoleTarget")
        {
            Layout = "[${level}] ${message} ${exception} (${logger})",
        };
        conf.AddTarget(consoleTarget);
        conf.AddRule(ParseLogLevel(commandLineArgs), LogLevel.Fatal, consoleTarget);;


        LogManager.Configuration = conf;
        LogManager.ReconfigExistingLoggers();
    }

    public static LogLevel ParseLogLevel(string[] commandLineArgs)
    {
        // Default log level if not specified
        var defaultLogLevel = LogLevel.Trace;

        // Find the --loglevel argument
        var logLevelIndex = Array.FindIndex(commandLineArgs,
            arg => arg.Equals("--loglevel", StringComparison.OrdinalIgnoreCase));

        // If --loglevel is not found or it's the last argument, return default
        if (logLevelIndex == -1 || logLevelIndex == commandLineArgs.Length - 1)
        {
            return defaultLogLevel;
        }

        // Get the value after --loglevel
        var logLevelValue = commandLineArgs[logLevelIndex + 1].ToLower();

        // Map string to NLog.LogLevel
        return logLevelValue switch
        {
            "trace" => LogLevel.Trace,
            "debug" => LogLevel.Debug,
            "info" => LogLevel.Info,
            "warn" or "warning" => LogLevel.Warn,
            "error" => LogLevel.Error,
            "fatal" => LogLevel.Fatal,
            "off" => LogLevel.Off,
            _ => defaultLogLevel
        };
    }
}