using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using BeatSaberModdingTools.Tasks;
using BeatSaberModdingTools.Tasks.Utilties;
using System.Linq;
using System.IO;
using Microsoft.Build.Tasks;
using System.Net.NetworkInformation;

namespace BSMTTasks_UnitTests
{
    [TestClass]
    public class GetCommitHash_Tests
    {
        public static readonly string DataFolder = Path.Combine("Data");
        public static readonly string OutputFolder = Path.Combine("Output", "GetCommitHash");

#if !NCRUNCH
        [TestMethod]
        public void GetGitStatus_Test()
        {
            string directory = Environment.CurrentDirectory;
            GitInfo status = GetCommitHash.GetGitStatus(directory);
            Assert.IsFalse(string.IsNullOrEmpty(status.Branch));
            Assert.IsFalse(string.IsNullOrEmpty(status.Modified));
            Assert.IsTrue(status.Modified == "Unmodified" || status.Modified == "Modified");
        }
        [TestMethod]
        public void TryGetCommitHash_Test()
        {
            string directory = Environment.CurrentDirectory;
            bool success = GetCommitHash.TryGetGitCommit(directory, out string commitHash);
            Assert.IsTrue(success);
        }
#endif
        [TestMethod]
        public void TryGetCommitManual_Test()
        {
            string directory = Path.Combine(DataFolder, "GitData", ".git");
            string expectedBranch = "master";
            string expectedHash = "4197466ed7682542b4669e98fd962a3925ccaadf";
            Assert.IsTrue(GetCommitHash.TryGetCommitManual(directory, out GitInfo gitInfo));
            Assert.AreEqual(expectedBranch, gitInfo.Branch);
            Assert.AreEqual(expectedHash, gitInfo.CommitHash);
        }
        #region Execute Tests
        [TestMethod]
        public void NoGit()
        {
            string directory = Path.Combine(DataFolder, "GitData");
            string expectedBranch = "master";
            int hashLength = 7;
            string expectedHash = "4197466ed7682542b4669e98fd962a3925ccaadf".Substring(0, hashLength);
            GetCommitHash task = new GetCommitHash()
            {
                ProjectDir = directory,
                NoGit = true,
                HashLength = hashLength
            };
            Assert.IsTrue(task.Execute());
            Console.WriteLine($"Branch: {task.Branch}");
            Assert.AreEqual(expectedBranch, task.Branch);
            Console.WriteLine($"Hash: {task.CommitHash}");
            Assert.AreEqual(expectedHash, task.CommitHash);
            Assert.AreEqual(hashLength, task.CommitHash.Length);
        }
        [TestMethod]
        public void DefaultHashLength()
        {
            string directory = Path.Combine(DataFolder, "GitData");
            string expectedBranch = "master";
            int hashLength = 7;
            string expectedHash = "4197466ed7682542b4669e98fd962a3925ccaadf".Substring(0, hashLength);
            GetCommitHash task = new GetCommitHash()
            {
                ProjectDir = directory
            };
            Assert.IsTrue(task.Execute());
            Console.WriteLine($"Branch: {task.Branch}");
            Assert.AreEqual(expectedBranch, task.Branch);
            Console.WriteLine($"Hash: {task.CommitHash}");
            Assert.AreEqual(expectedHash, task.CommitHash);
            Assert.AreEqual(hashLength, task.CommitHash.Length);
        }
        #endregion
    }
}
