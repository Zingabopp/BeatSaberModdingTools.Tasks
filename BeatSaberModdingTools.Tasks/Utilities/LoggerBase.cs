using Microsoft.Build.Framework;
using System;

namespace BeatSaberModdingTools.Tasks.Utilities
{
    /// <summary>
    /// Base class for an <see cref="ITaskLogger"/>
    /// </summary>
    public abstract class LoggerBase : ITaskLogger
    {
        /// <inheritdoc/>
        public abstract void LogError(string subcategory, string errorCode, string helpKeyword, string file,
            int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);
        /// <inheritdoc/>
        public void LogError(string subcategory, string errorCode, string helpKeyword, string file, FilePosition position, string message, params object[] messageArgs)
            => LogError(subcategory, errorCode, helpKeyword, file,
            position.StartLine, position.StartColumn, position.EndLine, position.EndColumn, message, messageArgs);

        /// <inheritdoc/>
        public abstract void LogError(string message, params object[] messageArgs);

        /// <inheritdoc/>
        public abstract void LogErrorFromException(Exception exception);

        /// <inheritdoc/>
        public abstract void LogMessage(MessageImportance importance, string message, params object[] messageArgs);
        /// <inheritdoc/>
        public abstract void LogMessage(string subcategory, string code, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, MessageImportance messageImportance, string message, params object[] messageArgs);
        /// <inheritdoc/>
        public void LogMessage(string subcategory, string code, string helpKeyword, string file, FilePosition position, MessageImportance messageImportance, string message, params object[] messageArgs)
            => LogMessage(subcategory, code, helpKeyword, file, position.StartLine, position.StartColumn, position.EndLine, position.EndColumn, messageImportance, message, messageArgs);

        /// <inheritdoc/>
        public abstract void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);
        /// <inheritdoc/>
        public void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, FilePosition position, string message, params object[] messageArgs)
            => LogWarning(subcategory, warningCode, helpKeyword, file, position.StartLine, position.StartColumn, position.EndLine, position.EndColumn, message, messageArgs);
        /// <inheritdoc/>
        public abstract void LogWarning(string message, params object[] messageArgs);

        /// <inheritdoc/>
        public void Log(string subcategory, string code, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, LogMessageLevel level, string message, params object[] messageArgs)
        {
            switch (level)
            {
                case LogMessageLevel.Message:
                    LogMessage(subcategory, code, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, MessageImportance.High, message, messageArgs);
                    break;
                case LogMessageLevel.Warning:
                    LogWarning(subcategory, code, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, messageArgs);
                    break;
                case LogMessageLevel.Error:
                    LogError(subcategory, code, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, messageArgs);
                    break;
                default:
                    LogMessage(subcategory, code, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, MessageImportance.High, message, messageArgs);
                    break;
            }
        }

        /// <inheritdoc/>
        public void Log(string subcategory, string code, string helpKeyword, string file, FilePosition position, LogMessageLevel level,
            string message, params object[] messageArgs)
            => Log(subcategory, code, helpKeyword, file, position.StartLine, position.StartColumn, position.EndLine, position.EndColumn,
                level, message, messageArgs);

        /// <inheritdoc/>
        public abstract void Log(LogMessageLevel level, string message, params object[] messageArgs);
    }
}
