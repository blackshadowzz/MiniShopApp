using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.InformationLogs
{
    public static class SystemLogs
    {
        private static readonly object _userLogLock = new object();
        public static void UserLogPlainText(string logs)
        {
            // Ensure the wwwroot directory exists
            var wwwrootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }
            var logFilePath = Path.Combine(wwwrootPath, "UserLog.txt");
            lock (_userLogLock)
            {
                File.AppendAllText(logFilePath, logs);
            }
        }
        public static void GeneralLogPlainText(string logs)
        {
            // Ensure the wwwroot directory exists
            var wwwrootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }
            var logFilePath = Path.Combine(wwwrootPath, "GeneralLog.txt");
            lock (_userLogLock)
            {
                File.AppendAllText(logFilePath, logs);
            }
        }
        public static string? ReadLog(string? logFileName)
        {
            var wwwrootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
            var logFilePath = Path.Combine(wwwrootPath, logFileName!);
            string? UserLogContent = "";
            if (File.Exists(logFilePath))
            {
               return UserLogContent = File.ReadAllText(logFilePath);
            }
            else
            {
               return UserLogContent = "No log file found.";
            }
        }
    }
}
