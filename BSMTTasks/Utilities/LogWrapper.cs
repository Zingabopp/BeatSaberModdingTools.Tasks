using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberModdingTools.Tasks.Utilties
{
    public class LogWrapper : ITaskLogger
    {
        public TaskLoggingHelper Logger;
        public LogWrapper(TaskLoggingHelper logger) => Logger = logger;
        public void LogError(string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
            => Logger.LogError(subcategory, errorCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, messageArgs);

        public void LogError(string message, params object[] messageArgs) => Logger.LogError(message, messageArgs);

        public void LogErrorFromException(Exception exception) => LogErrorFromException(exception);

        public void LogMessage(MessageImportance importance, string message, params object[] messageArgs) => LogMessage(importance, message, messageArgs);

        public void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
            => Logger.LogWarning( subcategory,  warningCode,  helpKeyword,  file,  lineNumber,  columnNumber,  endLineNumber,  endColumnNumber,  message,  messageArgs);

        public void LogWarning(string message, params object[] messageArgs) => LogWarning(message, messageArgs);
    }
}
