using Serilog;

namespace TechnoWorld_API.Services
{
    public class LogService
    {
        public static void LodMessage(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Log.Debug(message);
                    break;
                case LogLevel.Info:
                    Log.Information(message);
                    break;
                case LogLevel.Warning:
                    Log.Warning(message);
                    break;
                case LogLevel.Error:
                    Log.Error(message);
                    break;
                case LogLevel.Fatal:
                    Log.Fatal(message);
                    break;
                default:
                    break;
            }
        }
    }
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Fatal = 4
    }
}
