using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;

namespace BeatSaberModdingTools.Tasks.Utilties
{
    /// <summary>
    /// A mock logger that implements <see cref="ITaskLogger"/> for unit testing.
    /// </summary>
    public class MockTaskLogger : ITaskLogger
    {
        /// <summary>
        /// List of log entries created by this instance.
        /// </summary>
        public List<MockLogEntry> LogEntries = new List<MockLogEntry>();

        /// <inheritdoc/>
        public void LogError(string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
        {
            LogEntries.Add(new MockLogEntry(LogEntryType.Error)
            {
                SubCategory = subcategory,
                MessageCode = errorCode,
                HelpKeyword = helpKeyword,
                File = file,
                LineNumber = lineNumber,
                ColumnNumber = columnNumber,
                EndLineNumber = endLineNumber,
                EndColumnNumber = endColumnNumber,
                Message = message,
                MessageArgs = messageArgs,
                Importance = MessageImportance.High
            });
        }

        /// <inheritdoc/>
        public void LogError(string message, params object[] messageArgs)
        {
            LogEntries.Add(new MockLogEntry(LogEntryType.Error)
            {
                Message = message,
                MessageArgs = messageArgs,
                Importance = MessageImportance.High
            });
        }

        /// <inheritdoc/>
        public void LogErrorFromException(Exception exception)
        {
            LogEntries.Add(new MockLogEntry(LogEntryType.Exception)
            {
                Exception = exception,
                Importance = MessageImportance.High
            });
        }

        /// <inheritdoc/>
        public void LogMessage(MessageImportance importance, string message, params object[] messageArgs)
        {
            LogEntries.Add(new MockLogEntry(LogEntryType.Message)
            {
                Message = message,
                MessageArgs = messageArgs,
                Importance = importance
            });
        }

        /// <inheritdoc/>
        public void LogMessage(string subcategory, string code, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, MessageImportance messageImportance, string message, params object[] messageArgs)
        {
            LogEntries.Add(new MockLogEntry(LogEntryType.Message)
            {
                SubCategory = subcategory,
                MessageCode = code,
                HelpKeyword = helpKeyword,
                File = file,
                LineNumber = lineNumber,
                ColumnNumber = columnNumber,
                EndLineNumber = endLineNumber,
                EndColumnNumber = endColumnNumber,
                Message = message,
                MessageArgs = messageArgs,
                Importance = MessageImportance.High
            });
        }

        /// <inheritdoc/>
        public void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
        {
            LogEntries.Add(new MockLogEntry(LogEntryType.Warning)
            {
                SubCategory = subcategory,
                MessageCode = warningCode,
                HelpKeyword = helpKeyword,
                File = file,
                LineNumber = lineNumber,
                ColumnNumber = columnNumber,
                EndLineNumber = endLineNumber,
                EndColumnNumber = endColumnNumber,
                Message = message,
                MessageArgs = messageArgs,
                Importance = MessageImportance.High
            });
        }

        /// <inheritdoc/>
        public void LogWarning(string message, params object[] messageArgs)
        {
            LogEntries.Add(new MockLogEntry(LogEntryType.Warning)
            {
                Message = message,
                MessageArgs = messageArgs,
                Importance = MessageImportance.High
            });
        }
    }

    /// <summary>
    /// Stores log entry data.
    /// </summary>
    public struct MockLogEntry
    {
        /// <summary>
        /// Log entry SubCategory.
        /// </summary>
        public string SubCategory;
        /// <summary>
        /// Log entry MessageCode.
        /// </summary>
        public string MessageCode;
        /// <summary>
        /// Log entry HelpKeyword.
        /// </summary>
        public string HelpKeyword;
        /// <summary>
        /// Log entry File name.
        /// </summary>
        public string File;
        /// <summary>
        /// Log entry line number.
        /// </summary>
        public int LineNumber;
        /// <summary>
        /// Log entry column number.
        /// </summary>
        public int ColumnNumber;
        /// <summary>
        /// Log entry end line number.
        /// </summary>
        public int EndLineNumber;
        /// <summary>
        /// Log entry end column number.
        /// </summary>
        public int EndColumnNumber;
        /// <summary>
        /// Log entry Message.
        /// </summary>
        public string Message;
        /// <summary>
        /// Log entry Message args.
        /// </summary>
        public object[] MessageArgs;
        /// <summary>
        /// Log entry Exception.
        /// </summary>
        public Exception Exception;
        /// <summary>
        /// Log entry Importance.
        /// </summary>
        public MessageImportance Importance;
        /// <summary>
        /// Log entry type.
        /// </summary>
        public LogEntryType EntryType;

        /// <summary>
        /// Creates a new <see cref="MockLogEntry"/> of the given type.
        /// </summary>
        /// <param name="entryType"></param>
        public MockLogEntry(LogEntryType entryType)
        {
            SubCategory = null;
            MessageCode = null;
            HelpKeyword = null;
            File = null;
            LineNumber = 0;
            ColumnNumber = 0;
            EndLineNumber = 0;
            EndColumnNumber = 0;
            Message = null;
            MessageArgs = Array.Empty<object>();
            Exception = null;
            Importance = MessageImportance.High;
            EntryType = entryType;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (EntryType == LogEntryType.Exception)
                return Exception.Message;
            string message = Message;
            for (int i = 0; i < MessageArgs.Length; i++)
            {
                message = message.Replace($"{{{i}}}", MessageArgs[i]?.ToString() ?? string.Empty);
            }
            return message;
        }
    }
    /// <summary>
    /// Log entry type.
    /// </summary>
    public enum LogEntryType
    {
        /// <summary>
        /// Message
        /// </summary>
        Message,
        /// <summary>
        /// Warning
        /// </summary>
        Warning,
        /// <summary>
        /// Error
        /// </summary>
        Error,
        /// <summary>
        /// Exception
        /// </summary>
        Exception
    }
}
