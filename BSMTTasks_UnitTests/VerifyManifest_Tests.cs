using BeatSaberModdingTools.Tasks;
using BeatSaberModdingTools.Tasks.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace BSMTTasks_UnitTests
{

    [TestClass]
    public class VerifyManifest_Tests
    {
        [TestMethod]
        public void Matching_DefaultPaths()
        {
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string pluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo();

            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
        }


        [TestMethod]
        public void AssemblyFileVersionFirst()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "AssemblyFileVersionFirst.cs");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo()
            {
                AssemblyInfoPath = assemblyFilePath
            };
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
        }

        [TestMethod]
        public void MissingAssemblyVersion()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "MissingAssemblyVersion.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = MessageCodes.ErrorString;
            string expectedPluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo
            {
                AssemblyInfoPath = assemblyFilePath
            };
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(2, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries[0];
            Assert.AreEqual($"Unable to parse the AssemblyVersion from {assemblyFilePath}", logEntry.ToString());
            logEntry = mockTaskLogger.LogEntries[1];
            Assert.AreEqual("AssemblyVersion could not be determined.", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
        }

        [TestMethod]
        public void MissingAssemblyVersion_FailOnError()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "MissingAssemblyVersion.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = MessageCodes.ErrorString;
            string expectedPluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo
            {
                AssemblyInfoPath = assemblyFilePath,
                FailOnError = true
            };
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual($"Unable to parse the AssemblyVersion from {assemblyFilePath}", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
        }

        [TestMethod]
        public void NoAssemblyFileVersion()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "NoAssemblyFileVersion.cs");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo();
            task.AssemblyInfoPath = assemblyFilePath;
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
        }


        [TestMethod]
        public void NoAssemblyFileVersion_FailOnError()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "NoAssemblyFileVersion.cs");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo() 
            { 
                FailOnError = true
            };
            task.AssemblyInfoPath = assemblyFilePath;
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
            
        }

        [TestMethod]
        public void AssemblyFileMismatch()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "AssemblyFileMismatch.cs");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo();
            task.AssemblyInfoPath = assemblyFilePath;
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual($"AssemblyVersion 1.1.0 does not match AssemblyFileVersion 1.2.0 in {assemblyFilePath}", logEntry.ToString());
            Assert.AreEqual(LogEntryType.Warning, logEntry.EntryType);
            Assert.AreEqual(36, logEntry.LineNumber);
            Assert.AreEqual(36, logEntry.EndLineNumber);
            Assert.AreEqual(33, logEntry.ColumnNumber);
            Assert.AreEqual(38, logEntry.EndColumnNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
            
        }

        [TestMethod]
        public void AssemblyFileMismatch_FailOnError()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "AssemblyFileMismatch.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = MessageCodes.ErrorString;
            string expectedPluginVersion = "1.1.0";
            GetAssemblyInfo task = new GetAssemblyInfo()
            {
                FailOnError = true 
            };
            task.AssemblyInfoPath = assemblyFilePath;
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual($"AssemblyVersion 1.1.0 does not match AssemblyFileVersion 1.2.0 in {assemblyFilePath}", logEntry.ToString());
            Assert.AreEqual(LogEntryType.Error, logEntry.EntryType);
            Assert.AreEqual(36, logEntry.LineNumber);
            Assert.AreEqual(36, logEntry.EndLineNumber);
            Assert.AreEqual(33, logEntry.ColumnNumber);
            Assert.AreEqual(38, logEntry.EndColumnNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
        }

        [TestMethod]
        public void BadAssemblyFileVersion()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "BadAssemblyFileVersion.cs");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo();
            task.AssemblyInfoPath = assemblyFilePath;
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual($"Unable to parse the AssemblyFileVersion from {assemblyFilePath}", logEntry.ToString());
            Assert.AreEqual(LogEntryType.Warning, logEntry.EntryType);
            Assert.AreEqual(36, logEntry.LineNumber);
            Assert.AreEqual(36, logEntry.EndLineNumber);
            Assert.AreEqual(32, logEntry.ColumnNumber);
            Assert.AreEqual(32, logEntry.EndColumnNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
        }

        [TestMethod]
        public void BadAssemblyFileVersion_FailOnError()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "BadAssemblyFileVersion.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = MessageCodes.ErrorString;
            string expectedPluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo
            {
                AssemblyInfoPath = assemblyFilePath,
                FailOnError = true
            };
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual($"Unable to parse the AssemblyFileVersion from {assemblyFilePath}", logEntry.ToString());
            Assert.AreEqual(LogEntryType.Error, logEntry.EntryType);
            Assert.AreEqual(36, logEntry.LineNumber);
            Assert.AreEqual(36, logEntry.EndLineNumber);
            Assert.AreEqual(32, logEntry.ColumnNumber);
            Assert.AreEqual(32, logEntry.EndColumnNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
        }



        [TestMethod]
        public void MissingAssemblyInfo()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "MissingAssemblyInfo.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = MessageCodes.ErrorString;
            string expectedPluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo
            {
                AssemblyInfoPath = assemblyFilePath
            };
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(2, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries[0];
            Assert.AreEqual($"Could not find AssemblyInfo: {assemblyFilePath}", logEntry.ToString());
            logEntry = mockTaskLogger.LogEntries[1];
            Assert.AreEqual("AssemblyVersion could not be determined.", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
            
        }

        [TestMethod]
        public void MissingAssemblyInfo_FailOnError()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "MissingAssemblyInfo.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = MessageCodes.ErrorString;
            string expectedPluginVersion = "1.1.0";

            GetAssemblyInfo task = new GetAssemblyInfo
            {
                AssemblyInfoPath = assemblyFilePath,
                FailOnError = true
            };
            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries[0];
            Assert.AreEqual($"Error in GetAssemblyInfo: Could not find AssemblyInfo: {assemblyFilePath}", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, task.AssemblyVersion);
            
        }
    }
}
