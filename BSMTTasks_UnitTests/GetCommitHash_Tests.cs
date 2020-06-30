using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using BeatSaberModdingTools.Tasks;
using BeatSaberModdingTools.Tasks.Utilties;
using System.Linq;
using System.IO;
using Microsoft.Build.Tasks;

namespace BSMTTasks_UnitTests
{
    [TestClass]
    public class GetCommitHash_Tests
    {
        public static readonly string DataFolder = Path.Combine("Data", "GetCommitHash");
        public static readonly string OutputFolder = Path.Combine("Output", "GetCommitHash");

#if !NCRUNCH
        [TestMethod]
        public void GetGitStatus_Test()
        {
            string directory = Environment.CurrentDirectory;
            GitStatus status = GetCommitHash.GetGitStatus(directory);
            Assert.IsFalse(string.IsNullOrEmpty(status.Branch));
            Assert.IsFalse(string.IsNullOrEmpty(status.Modified));

        }
        [TestMethod]
        public void TryGetCommitHash_Test()
        {
            string directory = Environment.CurrentDirectory;
            bool success = GetCommitHash.TryGetGitCommit(directory, out string commitHash);
            Assert.IsTrue(success);
        }

#endif
    }
}
