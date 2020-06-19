using Microsoft.Build.Framework;
using System;

namespace BeatSaberModdingTools.Tasks.Utilties
{
    /// <summary>
    /// Logging interface for easier unit testing.
    /// </summary>
    public interface ITaskLogger
    {
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="importance"></param>
        /// <param name="message"></param>
        /// <param name="messageArgs"></param>
        void LogMessage(MessageImportance importance, string message, params object[] messageArgs);
        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="subcategory"></param>
        /// <param name="warningCode"></param>
        /// <param name="helpKeyword"></param>
        /// <param name="file"></param>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        /// <param name="endLineNumber"></param>
        /// <param name="endColumnNumber"></param>
        /// <param name="message"></param>
        /// <param name="messageArgs"></param>
        void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);
        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageArgs"></param>
        void LogWarning(string message, params object[] messageArgs);
        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="subcategory"></param>
        /// <param name="errorCode"></param>
        /// <param name="helpKeyword"></param>
        /// <param name="file"></param>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        /// <param name="endLineNumber"></param>
        /// <param name="endColumnNumber"></param>
        /// <param name="message"></param>
        /// <param name="messageArgs"></param>
        void LogError(string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);
        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageArgs"></param>
        void LogError(string message, params object[] messageArgs);
        /// <summary>
        /// Logs an error from an <see cref="Exception"/>.
        /// </summary>
        /// <param name="exception"></param>
        void LogErrorFromException(Exception exception);
    }
}
