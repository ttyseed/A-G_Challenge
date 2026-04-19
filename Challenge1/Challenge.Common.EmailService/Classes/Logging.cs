using Microsoft.Extensions.Logging;
using challenge1.Common.EmailService.Classes.Interfaces;
using System.Runtime.CompilerServices;

namespace challenge1.Common.EmailService.Classes
{
    internal class Logging : ILogging
    {
        private readonly ILogger<Logging> _logger;

        public Logging(ILogger<Logging> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            try
            {
                _logger.LogInformation("Info {fileName} - {methodName} - {message}",
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
                _logger.LogError("Error {fileName} - {methodName} - {message}", fileName, methodName, message);
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
                _logger.LogError("BLL Error {fileName} - {methodName} - {message}", fileName, methodName, message);
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
                _logger.LogError("Repository Error {fileName} - {methodName} - {message}", fileName, methodName, message);
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
                _logger.LogError("Database Error {fileName} - {methodName} - {message}", fileName, methodName, message);
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
                _logger.LogError("Controller Error {fileName} - {methodName} - {message}", fileName, methodName, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }
    }
}
