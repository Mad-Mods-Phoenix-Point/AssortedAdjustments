using System;
using System.IO;

public class Logger
{
    private static string _logPath;
    private static int _debugLevel;
    private static string _modDirectory;
    private static string _modName;
    private static bool _awake;

    public static void Initialize(string logPath, int debugLevel, string modDirectory, string modName)
    {
        _logPath = logPath;
        _debugLevel = debugLevel;
        _modDirectory = modDirectory;
        _modName = modName;
        _awake = true;

        Cleanup();
        Always($"Logger.Initialize({logPath}, {debugLevel}, {modDirectory}, {modName})");
    }


    public static void Sleep()
    {
        _awake = false;
    }

    public static void Wake()
    {
        _awake = true;
    }


    public static void Cleanup()
    {
        using (StreamWriter writer = new StreamWriter(_logPath, false))
        {
            writer.WriteLine($"[{_modName} @ {DateTime.Now.ToString()}] CLEANED UP");
        }
    }


    public static void Error(Exception ex)
    {
        if (_awake && _debugLevel >= 1)
        {
            using (StreamWriter writer = new StreamWriter(_logPath, true))
            {
                writer.WriteLine("----------------------------------------------------------------------------------------------------");
                writer.WriteLine($"[{_modName} @ {DateTime.Now.ToString()}] EXCEPTION:");
                writer.WriteLine("Message: " + ex.Message + "<br/>" + Environment.NewLine + "StackTrace: " + ex.StackTrace);
                writer.WriteLine("----------------------------------------------------------------------------------------------------");
            }
        }
    }
    [Obsolete("Logger.Error is deprecated, please use Logger.Error instead")]
    public static void LogError(Exception ex)
    {
        Logger.Error(ex);
    }


    public static void Debug(String line, bool showPrefix = true)
    {
        if (_awake && _debugLevel >= 2)
        {
            using (StreamWriter writer = new StreamWriter(_logPath, true))
            {
                string prefix = showPrefix ? $"[{_modName} @ {DateTime.Now.ToString()}] " : "";
                writer.WriteLine(prefix + line);
            }
        }
    }
    [Obsolete("Logger.Debug is deprecated, please use Logger.Debug instead")]
    public static void LogLine(String line, bool showPrefix = true)
    {
        Logger.Debug(line, showPrefix);
    }


    public static void Info(String line, bool showPrefix = true)
    {
        if (_awake && _debugLevel >= 3)
        {
            Logger.Debug(line, showPrefix);
        }
    }
    [Obsolete("Logger.LogInfo is deprecated, please use Logger.Info instead")]
    public static void LogInfo(String line, bool showPrefix = true)
    {
        Logger.Info(line, showPrefix);
    }


    public static void Always(String line, bool showPrefix = true)
    {
        using (StreamWriter writer = new StreamWriter(_logPath, true))
        {
            string prefix = showPrefix ? $"[{_modName} @ {DateTime.Now.ToString()}] " : "";
            writer.WriteLine(prefix + line);
        }
    }
    [Obsolete("Logger.LogAlways is deprecated, please use Logger.Always instead")]
    public static void LogAlways(String line, bool showPrefix = true)
    {
        Logger.Always(line, showPrefix);
    }
}
