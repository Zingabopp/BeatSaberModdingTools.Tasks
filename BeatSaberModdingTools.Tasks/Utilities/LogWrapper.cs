using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;

namespace BeatSaberModdingTools.Tasks.Utilities
{
    /// <summary>
    /// Wrapper for <see cref="TaskLoggingHelper"/> that implements <see cref="ITaskLogger"/>.
    /// </summary>
    public class LogWrapper : LoggerBase
    {
        /// <summary>
        /// The <see cref="TaskLoggingHelper"/> instance.
        /// </summary>
        public TaskLoggingHelper Logger;
        /// <summary>
        /// Creates a new <see cref="LogWrapper"/> using a <see cref="TaskLoggingHelper"/>.
        /// </summary>
        /// <param name="logger"></param>
        public LogWrapper(TaskLoggingHelper logger) => Logger = logger;
        /// <inheritdoc/>
        public override void LogError(string subcategory, string errorCode, string helpKeyword, string file, 
            int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
            => Logger.LogError(subcategory, errorCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, messageArgs);

        /// <inheritdoc/>
        public override void LogError(string message, params object[] messageArgs) => Logger.LogError(message, messageArgs);

        /// <inheritdoc/>
        public override void LogErrorFromException(Exception exception) => Logger.LogErrorFromException(exception);

        /// <inheritdoc/>
        public override void LogMessage(MessageImportance importance, string message, params object[] messageArgs) => Logger.LogMessage(importance, message, messageArgs);

        /// <inheritdoc/>
        public override void LogMessage(string subcategory, string code, string helpKeyword, string file,
            int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber,
            MessageImportance messageImportance, string message, params object[] messageArgs)
        => Logger.LogMessage(subcategory, code, helpKeyword, file,
                lineNumber, columnNumber, endLineNumber, endColumnNumber,
                messageImportance, message, messageArgs);


        /// <inheritdoc/>
        public override void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
            => Logger.LogWarning(subcategory, warningCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, messageArgs);

        /// <inheritdoc/>
        public override void LogWarning(string message, params object[] messageArgs) => Logger.LogWarning(message, messageArgs);
    }
}
