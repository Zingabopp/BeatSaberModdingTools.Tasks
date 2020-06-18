using BSMTTasks;
using BSMTTasks.Utilties;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace BSMTTasks_UnitTests
{

    [TestClass]
    public class GetManifestInfo_Tests
    {
        [TestMethod]
        public void Matching_DefaultPaths()
        {
            GetManifestInfo getManifestInfo = new GetManifestInfo();
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }


        [TestMethod]
        public void Matching_KnownAssemblyVersion()
        {
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                KnownAssemblyVersion = expectedAssemblyVersion
            };
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void AssemblyFileVersionFirst()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "AssemblyFileVersionFirst.cs");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                AssemblyInfoPath = assemblyFilePath
            };
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void MissingAssemblyVersion()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "MissingAssemblyVersion.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = GetManifestInfo.ErrorString;
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo
            {
                AssemblyInfoPath = assemblyFilePath
            };
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(2, mockTaskLogger.LogEntries.Count); 
            MockLogEntry logEntry = mockTaskLogger.LogEntries[0];
            Assert.AreEqual("Unable to parse the AssemblyVersion from AssemblyInfos\\MissingAssemblyVersion.cs", logEntry.ToString());
            logEntry = mockTaskLogger.LogEntries[1];
            Assert.AreEqual("AssemblyVersion could not be determined.", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void MissingAssemblyVersion_ErrorOnMismatch()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "MissingAssemblyVersion.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = GetManifestInfo.ErrorString;
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo
            {
                AssemblyInfoPath = assemblyFilePath,
                ErrorOnMismatch = true
            };
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual("Unable to parse the AssemblyVersion from AssemblyInfos\\MissingAssemblyVersion.cs", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void NoAssemblyFileVersion()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "NoAssemblyFileVersion.cs");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo();
            getManifestInfo.AssemblyInfoPath = assemblyFilePath;
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }


        [TestMethod]
        public void NoAssemblyFileVersion_ErrorOnMismatch()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "NoAssemblyFileVersion.cs");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo() { ErrorOnMismatch = true };
            getManifestInfo.AssemblyInfoPath = assemblyFilePath;
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void AssemblyFileMismatch()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "AssemblyFileMismatch.cs");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo();
            getManifestInfo.AssemblyInfoPath = assemblyFilePath;
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual("AssemblyVersion 1.1.0 does not match AssemblyFileVersion 1.2.0 in AssemblyInfo.cs",logEntry.ToString());
            Assert.AreEqual(LogEntryType.Warning, logEntry.EntryType);
            Assert.AreEqual(36, logEntry.LineNumber);
            Assert.AreEqual(36, logEntry.EndLineNumber);
            Assert.AreEqual(33, logEntry.ColumnNumber);
            Assert.AreEqual(38, logEntry.EndColumnNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void AssemblyFileMismatch_ErrorOnMismatch()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "AssemblyFileMismatch.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = GetManifestInfo.ErrorString;
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";
            GetManifestInfo getManifestInfo = new GetManifestInfo() { ErrorOnMismatch = true };
            getManifestInfo.AssemblyInfoPath = assemblyFilePath;
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual("AssemblyVersion 1.1.0 does not match AssemblyFileVersion 1.2.0 in AssemblyInfo.cs", logEntry.ToString());
            Assert.AreEqual(LogEntryType.Error, logEntry.EntryType);
            Assert.AreEqual(36, logEntry.LineNumber);
            Assert.AreEqual(36, logEntry.EndLineNumber);
            Assert.AreEqual(33, logEntry.ColumnNumber);
            Assert.AreEqual(38, logEntry.EndColumnNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void ManifestNotFound()
        {
            string manifestPath = Path.Combine("Manifests", "DoesNotExist.json");
            bool expectedResult = false;
            string expectedAssemblyVersion = GetManifestInfo.ErrorString;
            string expectedPluginVersion = GetManifestInfo.ErrorString;
            string expectedGameVersion = GetManifestInfo.ErrorString;
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath
            };

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void NoVersionLine()
        {
            string manifestPath = Path.Combine("Manifests", "NoVersionLine.json");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = GetManifestInfo.ErrorString;
            string expectedGameVersion = "1.9.1";
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath
            };

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void NoVersionLine_ErrorOnMismatch()
        {
            string manifestPath = Path.Combine("Manifests", "NoVersionLine.json");
            bool expectedResult = false;
            string expectedAssemblyVersion = GetManifestInfo.ErrorString;
            string expectedPluginVersion = GetManifestInfo.ErrorString;
            string expectedGameVersion = GetManifestInfo.ErrorString;
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath,
                ErrorOnMismatch = true
            };

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }


        [TestMethod]
        public void NoGameVersionLine()
        {
            string manifestPath = Path.Combine("Manifests", "NoGameVersionLine.json");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = GetManifestInfo.ErrorString;
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath
            };

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void NoGameVersionLine_ErrorOnMismatch()
        {
            string manifestPath = Path.Combine("Manifests", "NoGameVersionLine.json");
            bool expectedResult = false;
            string expectedAssemblyVersion = GetManifestInfo.ErrorString;
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = GetManifestInfo.ErrorString;
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath,
                ErrorOnMismatch = true
            };

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual("GameVersion not found in Manifests\\NoGameVersionLine.json", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }


        [TestMethod]
        public void BadAssemblyFileVersion()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "BadAssemblyFileVersion.cs");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo();
            getManifestInfo.AssemblyInfoPath = assemblyFilePath;
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual("Unable to parse the AssemblyFileVersion from AssemblyInfos\\BadAssemblyFileVersion.cs", logEntry.ToString());
            Assert.AreEqual(LogEntryType.Warning, logEntry.EntryType);
            Assert.AreEqual(36, logEntry.LineNumber);
            Assert.AreEqual(36, logEntry.EndLineNumber);
            Assert.AreEqual(32, logEntry.ColumnNumber);
            Assert.AreEqual(32, logEntry.EndColumnNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void BadAssemblyFileVersion_ErrorOnMismatch()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "BadAssemblyFileVersion.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = GetManifestInfo.ErrorString;
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo
            {
                AssemblyInfoPath = assemblyFilePath,
                ErrorOnMismatch = true
            };
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual("Unable to parse the AssemblyFileVersion from AssemblyInfos\\BadAssemblyFileVersion.cs", logEntry.ToString());
            Assert.AreEqual(LogEntryType.Error, logEntry.EntryType);
            Assert.AreEqual(36, logEntry.LineNumber);
            Assert.AreEqual(36, logEntry.EndLineNumber);
            Assert.AreEqual(32, logEntry.ColumnNumber);
            Assert.AreEqual(32, logEntry.EndColumnNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }


        [TestMethod]
        public void MismatchedVersions()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "MismatchedVersions.cs");
            string manifestFilePath = Path.Combine("Manifests", "MismatchedVersions.json");
            bool expectedResult = true;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.2.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo
            {
                AssemblyInfoPath = assemblyFilePath,
                ManifestPath = manifestFilePath
            };
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual("PluginVersion 1.2.0 does not match AssemblyVersion 1.1.0", logEntry.ToString());
            Assert.AreEqual(LogEntryType.Message, logEntry.EntryType);
            Assert.AreEqual(0, logEntry.LineNumber);
            Assert.AreEqual(0, logEntry.EndLineNumber);
            Assert.AreEqual(0, logEntry.ColumnNumber);
            Assert.AreEqual(0, logEntry.EndColumnNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }


        [TestMethod]
        public void MismatchedVersions_ErrorOnMismatch()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "MismatchedVersions.cs");
            string manifestFilePath = Path.Combine("Manifests", "MismatchedVersions.json");
            bool expectedResult = false;
            string expectedAssemblyVersion = "1.1.0";
            string expectedPluginVersion = "1.2.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo
            {
                AssemblyInfoPath = assemblyFilePath,
                ManifestPath = manifestFilePath,
                ErrorOnMismatch = true
            };
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual("PluginVersion 1.2.0 in Manifests\\MismatchedVersions.json does not match AssemblyVersion 1.1.0 in AssemblyInfos\\MismatchedVersions.cs", logEntry.ToString());
            Assert.AreEqual(LogEntryType.Error, logEntry.EntryType);
            Assert.AreEqual(8, logEntry.LineNumber);
            Assert.AreEqual(8, logEntry.EndLineNumber);
            Assert.AreEqual(1, logEntry.ColumnNumber);
            Assert.AreEqual(1, logEntry.EndColumnNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }


        [TestMethod]
        public void MissingAssemblyInfo()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "MissingAssemblyInfo.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = GetManifestInfo.ErrorString;
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo
            {
                AssemblyInfoPath = assemblyFilePath
            };
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(2, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries[0];
            Assert.AreEqual(@"Could not find AssemblyInfo: AssemblyInfos\MissingAssemblyInfo.cs", logEntry.ToString());
            logEntry = mockTaskLogger.LogEntries[1];
            Assert.AreEqual("AssemblyVersion could not be determined.", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void MissingAssemblyInfo_ErrorOnMismatch()
        {
            string assemblyFilePath = Path.Combine("AssemblyInfos", "MissingAssemblyInfo.cs");
            bool expectedResult = false;
            string expectedAssemblyVersion = GetManifestInfo.ErrorString;
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo
            {
                AssemblyInfoPath = assemblyFilePath,
                ErrorOnMismatch = true
            };
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (var entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries[0];
            Assert.AreEqual(@"Could not find AssemblyInfo: AssemblyInfos\MissingAssemblyInfo.cs", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedAssemblyVersion, getManifestInfo.AssemblyVersion);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }
    }

    public class TestLogger : TaskLoggingHelper
    {
        public TestLogger(ITask taskInstance) : base(taskInstance)
        {

        }
        public TestLogger(IBuildEngine buildEngine, string taskName) : base(buildEngine, taskName) { }

    }
}
