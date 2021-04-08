using BeatSaberModdingTools.Tasks;
using BeatSaberModdingTools.Tasks.Utilities;
using BeatSaberModdingTools.Tasks.Utilities.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace BSMTTasks_UnitTests
{

    [TestClass]
    public class CompareVersions_Tests
    {
        [TestMethod]
        public void Matching()
        {
            bool expectedResult = true;
            string assemblyVersion = "1.1.0";
            string pluginVersion = "1.1.0";
            string[] logMessages = new string[] { };

            CompareVersions task = new CompareVersions()
            {
                AssemblyVersion = assemblyVersion,
                PluginVersion = pluginVersion
            };

            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(logMessages.Length, mockTaskLogger.LogEntries.Count); for (int i = 0; i < logMessages.Length; i++)
            {
                Assert.AreEqual(logMessages[i], mockTaskLogger.LogEntries[i].ToString());
            }
            Assert.AreEqual(expectedResult, taskResult);
        }

        [TestMethod]
        public void Matching_AsmVerTrailing0()
        {
            bool expectedResult = true;
            string assemblyVersion = "1.1.0.0";
            string pluginVersion = "1.1.0";
            string[] logMessages = new string[] { };

            CompareVersions task = new CompareVersions()
            {
                AssemblyVersion = assemblyVersion,
                PluginVersion = pluginVersion
            };

            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(logMessages.Length, mockTaskLogger.LogEntries.Count); for (int i = 0; i < logMessages.Length; i++)
            {
                Assert.AreEqual(logMessages[i], mockTaskLogger.LogEntries[i].ToString());
            }
            Assert.AreEqual(expectedResult, taskResult);
        }

        [TestMethod]
        public void Matching_AsmVerLeading0()
        {
            bool expectedResult = true;
            string assemblyVersion = "0.1.1.0";
            string pluginVersion = "1.1.0";
            string[] logMessages = new string[] { };

            CompareVersions task = new CompareVersions()
            {
                AssemblyVersion = assemblyVersion,
                PluginVersion = pluginVersion
            };

            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(logMessages.Length, mockTaskLogger.LogEntries.Count); for (int i = 0; i < logMessages.Length; i++)
            {
                Assert.AreEqual(logMessages[i], mockTaskLogger.LogEntries[i].ToString());
            }
            Assert.AreEqual(expectedResult, taskResult);
        }

        [TestMethod]
        public void NotMatching()
        {
            bool expectedResult = true;
            string assemblyVersion = "1.1.0";
            string pluginVersion = "1.2.0";
            string[] logMessages = new string[]
            {
                $"PluginVersion {pluginVersion} does not match AssemblyVersion {assemblyVersion}."
            };
            CompareVersions task = new CompareVersions()
            {
                AssemblyVersion = assemblyVersion,
                PluginVersion = pluginVersion
            };

            bool taskResult = task.Execute();
            MockTaskLogger mockTaskLogger = task.Logger as MockTaskLogger;
            foreach (MockLogEntry entry in mockTaskLogger.LogEntries)
            {
                Console.WriteLine(entry);
            }

            Assert.AreEqual(logMessages.Length, mockTaskLogger.LogEntries.Count);
            for(int i = 0; i < logMessages.Length; i++)
            {
                Assert.AreEqual(logMessages[i], mockTaskLogger.LogEntries[i].ToString());
            }
            Assert.AreEqual(expectedResult, taskResult);
        }

    }
}
