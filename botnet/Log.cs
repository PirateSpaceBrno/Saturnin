using System;

namespace botnet
{
    public static class Log
    {
        public static void Write(string logMessage, LogLevel logLevel = LogLevel.INFO)
        {
            if (Configuration.DEBUG == false && logLevel == LogLevel.DEBUG) return;

            Console.WriteLine(DateTime.Now.ToString("<yyyy-MM-dd HH:mm:ss>") + " [" + logLevel.ToString() + "] " + logMessage);
        }

        public static void Line()
        {
            Console.WriteLine("------------------------------");
        }

        public enum LogLevel
        {
            INFO,
            DEBUG,
            FAIL,
            FATAL
        }
    }
}
