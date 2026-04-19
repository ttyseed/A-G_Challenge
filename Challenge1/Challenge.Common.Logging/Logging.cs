using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace challenge1.Common.Logging
{
    public class Logging : ILogging
    {
        private readonly ILogger Logger;

        public Logging(ILogger<Logging> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogInfo(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            try
            {
                Logger.LogInformation("Info {fileName} - {methodName} - {message}",
                    Path.GetFileName(filePath), methodName, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        public void LogError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                Logger.LogError("Error {fileName} - {methodName} - {message}", fileName, methodName, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        public void LogBllError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                Logger.LogError("BLL Error {fileName} - {methodName} - {message}", fileName, methodName, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        public void LogRepoError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                Logger.LogError("Repository Error {fileName} - {methodName} - {message}", fileName, methodName, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        public void LogDatabaseError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                Logger.LogError("Database Error {fileName} - {methodName} - {message}", fileName, methodName, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        public void LogControllerError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                Logger.LogError("Controller Error {fileName} - {methodName} - {message}", fileName, methodName, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }
    }
}
