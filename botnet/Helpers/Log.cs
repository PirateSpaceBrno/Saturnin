using System;
using System.IO;
using System.Threading.Tasks;
using Saturnin.Texts;

namespace Saturnin.Helpers
{
    public static class Log
    {
        public static async void Write(string logMessage, LogLevel logLevel = LogLevel.INFO)
        {
            if (IsDebugRun() == false && logLevel == LogLevel.DEBUG) return;

            await Task.Run(() =>
            {
                Console.WriteLine(DateTime.Now.ToString("<yyyy-MM-dd HH:mm:ss>") + " [" + logLevel.ToString() + "] " + logMessage);
            });
            
        }

        public static async void Line()
        {
            await Task.Run(() =>
            {
                Console.WriteLine("------------------------------");
            });
            
        }

        public static bool IsDebugRun()
        {
            return File.Exists($"{Environment.CurrentDirectory}/DEBUG");
        }

        public enum LogLevel
        {
            INFO,
            DEBUG,
            WARN,
            ERROR,
            FATAL
        }
    }
}
