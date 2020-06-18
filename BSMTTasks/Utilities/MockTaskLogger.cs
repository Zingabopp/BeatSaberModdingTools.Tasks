using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSMTTasks.Utilties
{
    public class MockTaskLogger : ITaskLogger
    {
        public List<MockLogEntry> LogEntries = new List<MockLogEntry>();
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

        public void LogError(string message, params object[] messageArgs)
        {
            LogEntries.Add(new MockLogEntry(LogEntryType.Error)
            {
                Message = message,
                MessageArgs = messageArgs,
                Importance = MessageImportance.High
            });
        }

        public void LogErrorFromException(Exception exception)
        {
            LogEntries.Add(new MockLogEntry(LogEntryType.Exception)
            {
                Exception = exception,
                Importance = MessageImportance.High
            });
        }

        public void LogMessage(MessageImportance importance, string message, params object[] messageArgs)
        {
            LogEntries.Add(new MockLogEntry(LogEntryType.Message)
            {
                Message = message,
                MessageArgs = messageArgs,
                Importance = importance
            });
        }

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

    public struct MockLogEntry
    {
        public string SubCategory;
        public string MessageCode;
        public string HelpKeyword;
        public string File;
        public int LineNumber;
        public int ColumnNumber;
        public int EndLineNumber;
        public int EndColumnNumber;
        public string Message;
        public object[] MessageArgs;
        public Exception Exception;
        public MessageImportance Importance;
        public LogEntryType EntryType;

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

        public override string ToString()
        {
            if (EntryType == LogEntryType.Exception)
                return Exception.Message;
            string message = Message;
            for(int i = 0; i < MessageArgs.Length; i++)
            {
                message = message.Replace($"{{{i}}}", MessageArgs[i]?.ToString() ?? string.Empty);
            }
            return message;
        }
    }
    public enum LogEntryType
    {
        Message,
        Warning,
        Error,
        Exception
    }
}
