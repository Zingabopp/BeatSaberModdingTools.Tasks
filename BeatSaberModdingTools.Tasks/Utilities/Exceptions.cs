using System;

namespace BeatSaberModdingTools.Tasks.Utilities
{

    /// <summary>
    /// Exception thrown on parsing errors.
    /// </summary>
    public class ParsingException : Exception
    {
        /// <summary>
        /// Log entry SubCategory.
        /// </summary>
        public readonly string SubCategory;
        /// <summary>
        /// Log entry MessageCode.
        /// </summary>
        public readonly string MessageCode;
        /// <summary>
        /// Log entry HelpKeyword.
        /// </summary>
        public readonly string HelpKeyword;
        /// <summary>
        /// Log entry File name.
        /// </summary>
        public readonly string File;
        /// <summary>
        /// Log entry line number.
        /// </summary>
        public readonly int LineNumber;
        /// <summary>
        /// Log entry column number.
        /// </summary>
        public readonly int ColumnNumber;
        /// <summary>
        /// Log entry end line number.
        /// </summary>
        public readonly int EndLineNumber;
        /// <summary>
        /// Log entry end column number.
        /// </summary>
        public readonly int EndColumnNumber;
        /// <summary>
        /// Log entry Message args.
        /// </summary>
        public readonly object[] MessageArgs;

        /// <inheritdoc/>
        public ParsingException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public ParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }
        /// <summary>
        /// Creates a <see cref="ParsingException"/> using the given message data.
        /// </summary>
        /// <param name="subCategory"></param>
        /// <param name="messageCode"></param>
        /// <param name="helpKeyword"></param>
        /// <param name="file"></param>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        /// <param name="endLineNumber"></param>
        /// <param name="endColumnNumber"></param>
        /// <param name="message"></param>
        /// <param name="messageArgs"></param>
        public ParsingException(string subCategory, string messageCode, string helpKeyword,
            string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber,
            string message, params object[] messageArgs)
            : base(string.Format(message, messageArgs))
        {
            SubCategory = subCategory;
            MessageCode = messageCode;
            HelpKeyword = helpKeyword;
            File = file;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            EndLineNumber = endLineNumber;
            EndColumnNumber = endColumnNumber;
            MessageArgs = messageArgs;
        }
    }

    /// <summary>
    /// Exception thrown when matching versions.
    /// </summary>
    public class VersionMatchException : ArgumentException
    {
        /// <inheritdoc/>
        public VersionMatchException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public VersionMatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    /// <summary>
    /// Extensions for <see cref="ParsingException"/>.
    /// </summary>
    public static class ParsingExceptionExtensions
    {
        /// <summary>
        /// Logs an error from the <see cref="ParsingException"/> using the given <see cref="ITaskLogger"/>.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="Log"></param>
        public static void LogErrorFromException(this ParsingException ex, ITaskLogger Log)
        {
            Log.LogError(null, ex.MessageCode, "", ex.File, ex.LineNumber, ex.ColumnNumber, ex.EndLineNumber, ex.EndColumnNumber, ex.Message, ex.MessageArgs);
        }

        /// <summary>
        /// Logs an error from the <see cref="ParsingException"/> using the given <see cref="ITaskLogger"/>.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="file"></param>
        /// <param name="position"></param>
        /// <param name="Log"></param>
        public static void LogErrorFromException(this ParsingException ex, ITaskLogger Log, string file, FilePosition position)
        {
            Log.LogError(null, ex.MessageCode, "", file, position, ex.Message, ex.MessageArgs);
        }
    }
}
