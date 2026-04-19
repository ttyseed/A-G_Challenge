using System.Runtime.CompilerServices;

namespace challenge1.Common.Logging
{
    public interface ILogging
    {
        /// <summary>
        /// CallerMemberName attribute is used to capture the name of the method that called this method.
        /// CallerFilePath attribute is used to capture the file path of the source code file that contains the caller.
        /// Pass in values for these parameters automatically when the method is called. Overwriting these parameters is optional.
        /// </summary>
        void LogInfo(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "");
        void LogError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "");
        void LogBllError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "");
        void LogRepoError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "");
        void LogDatabaseError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "");
        void LogControllerError(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "");
    }
}
