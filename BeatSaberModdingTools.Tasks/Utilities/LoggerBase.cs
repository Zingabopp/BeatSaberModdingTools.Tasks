using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
        public void LogError(string subcategory, string errorCode, string helpKeyword, string file, Position position, string message, params object[] messageArgs)
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
        public void LogMessage(string subcategory, string code, string helpKeyword, string file, Position position, MessageImportance messageImportance, string message, params object[] messageArgs)
            => LogMessage(subcategory, code, helpKeyword, file, position.StartLine, position.StartColumn, position.EndLine, position.EndColumn, messageImportance, message, messageArgs);

        /// <inheritdoc/>
        public abstract void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);
        /// <inheritdoc/>
        public void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, Position position, string message, params object[] messageArgs)
            => LogWarning(subcategory, warningCode, helpKeyword, file, position.StartLine, position.StartColumn, position.EndLine, position.EndColumn, message,  messageArgs);
        /// <inheritdoc/>
        public abstract void LogWarning(string message, params object[] messageArgs);
    }
}
