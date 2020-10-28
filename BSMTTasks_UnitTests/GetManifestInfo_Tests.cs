using BeatSaberModdingTools.Tasks;
using BeatSaberModdingTools.Tasks.Utilities;
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
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }


        [TestMethod]
        public void Matching_KnownAssemblyVersion()
        {
            bool expectedResult = true;
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = "1.9.1";

            GetManifestInfo getManifestInfo = new GetManifestInfo();
            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(0, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void ManifestNotFound()
        {
            string manifestPath = Path.Combine("Manifests", "DoesNotExist.json");
            bool expectedResult = false;
            string expectedPluginVersion = MessageCodes.ErrorString;
            string expectedGameVersion = MessageCodes.ErrorString;
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath
            };

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual($"Error in GetManifestInfo: Manifest file not found at {manifestPath}", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void NoVersionLine()
        {
            string manifestPath = Path.Combine("Manifests", "NoVersionLine.json");
            bool expectedResult = true;
            string expectedPluginVersion = MessageCodes.ErrorString;
            string expectedGameVersion = "1.9.1";
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath
            };

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void NoVersionLine_ErrorOnMismatch()
        {
            string manifestPath = Path.Combine("Manifests", "NoVersionLine.json");
            bool expectedResult = false;
            string expectedPluginVersion = MessageCodes.ErrorString;
            string expectedGameVersion = MessageCodes.ErrorString;
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath,
                FailOnError = true
            };

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }


        [TestMethod]
        public void NoGameVersionLine()
        {
            string manifestPath = Path.Combine("Manifests", "NoGameVersionLine.json");
            bool expectedResult = true;
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = MessageCodes.ErrorString;
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath
            };

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }

        [TestMethod]
        public void NoGameVersionLine_ErrorOnMismatch()
        {
            string manifestPath = Path.Combine("Manifests", "NoGameVersionLine.json");
            bool expectedResult = false;
            string expectedPluginVersion = "1.1.0";
            string expectedGameVersion = MessageCodes.ErrorString;
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath,
                FailOnError = true
            };

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual($"GameVersion not found in {manifestPath}", logEntry.ToString());
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }


        [TestMethod]
        public void BadVersion()
        {

            string manifestPath = Path.GetFullPath(Path.Combine("Manifests", "ParsingError.json"));
            Assert.IsTrue(File.Exists(manifestPath), $"File not found: '{manifestPath}'");
            Console.WriteLine(Path.GetFullPath(manifestPath));
            GetManifestInfo getManifestInfo = new GetManifestInfo()
            {
                ManifestPath = manifestPath
            };
            bool expectedResult = false;
            string expectedPluginVersion = "E.R.R";
            string expectedGameVersion = "E.R.R";

            bool taskResult = getManifestInfo.Execute();
            MockTaskLogger mockTaskLogger = getManifestInfo.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }
            Assert.AreEqual(1, mockTaskLogger.LogEntries.Count);
            MockLogEntry logEntry = mockTaskLogger.LogEntries.First();
            Assert.AreEqual($"Error reading version in manifest: Could not parse 'b' in '1.b.0' to an integer.", logEntry.ToString());
            Assert.AreEqual(8, logEntry.LineNumber);
            Assert.AreEqual(expectedResult, taskResult);
            Assert.AreEqual(expectedPluginVersion, getManifestInfo.PluginVersion);
            Assert.AreEqual(expectedGameVersion, getManifestInfo.GameVersion);
        }
    }
}
