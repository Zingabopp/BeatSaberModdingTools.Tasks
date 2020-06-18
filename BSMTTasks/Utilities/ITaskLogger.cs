using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSMTTasks.Utilties
{
    public interface ITaskLogger
    {
        void LogMessage(MessageImportance importance, string message, params object[] messageArgs);
        void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs); 
        void LogWarning(string message, params object[] messageArgs);
        void LogError(string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);
        void LogError(string message, params object[] messageArgs);
        void LogErrorFromException(Exception exception);
    }
}
