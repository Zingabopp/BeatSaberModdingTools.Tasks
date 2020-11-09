using BeatSaberModdingTools.Tasks;
using BeatSaberModdingTools.Tasks.Utilities;
using BSMTTasks_UnitTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace BSMTTasks_UnitTests
{
    [TestClass]
    public class GetCommitInfo_Tests
    {
        public static readonly string DataFolder = Path.Combine("Data");
        public static readonly string OutputFolder = Path.Combine("Output", "GetCommitInfo");

        [TestMethod]
        public void TryGetCommitManual_Test()
        {
            string directory = Path.Combine(DataFolder, "GitTests", "Test.git");
            string expectedBranch = "master";
            string expectedHash = "4197466ed7682542b4669e98fd962a3925ccaadf";
            string expectedUrl = @"https://github.com/Zingabopp/BeatSaberModdingTools.Tasks";
            string expectedUser = "Zingabopp";
            Assert.IsTrue(GetCommitInfo.TryGetCommitManual(directory, out GitInfo gitInfo));
            Assert.AreEqual(expectedBranch, gitInfo.Branch);
            Assert.AreEqual(expectedHash, gitInfo.CommitHash);
            Assert.AreEqual(expectedUrl, gitInfo.OriginUrl);
            Assert.AreEqual(expectedUser, gitInfo.GitUser);
        }

        [TestMethod]
        public void GetGitHubUsername()
        {
            string url = @"https://github.com/Zingabopp/BeatSaberModdingTools.Tasks";
            string expectedUser = "Zingabopp";
            string actualUser = GetCommitInfo.GetGitHubUser(url);
            Assert.AreEqual(expectedUser, actualUser);
        }

        [TestMethod]
        public void GetGitHubUsername_NoHttp()
        {
            string url = @"github.com/Zingabopp/BeatSaberModdingTools.Tasks";
            string expectedUser = "Zingabopp";
            string actualUser = GetCommitInfo.GetGitHubUser(url);
            Assert.AreEqual(expectedUser, actualUser);
        }

        [TestMethod]
        public void GetGitHubUsername_NotGithub()
        {
            string url = @"https://gitlab.com/Zingabopp/BeatSaberModdingTools.Tasks";
            string expectedUser = null;
            string actualUser = GetCommitInfo.GetGitHubUser(url);
            Assert.AreEqual(expectedUser, actualUser);
        }

        [TestMethod]
        public void GetGitHubUsername_NotAUrl()
        {
            string url = @"asdfasdf";
            string expectedUser = null;
            string actualUser = GetCommitInfo.GetGitHubUser(url);
            Assert.AreEqual(expectedUser, actualUser);
        }

        [TestMethod]
        public void GetGitHubUsername_NoUser()
        {
            string url = @"https://github.com/";
            string expectedUser = null;
            string actualUser = GetCommitInfo.GetGitHubUser(url);
            Assert.AreEqual(expectedUser, actualUser);
        }

        #region Execute Tests
        [TestMethod]
        public void NoGit()
        {
            string directory = Path.Combine(DataFolder, "GitTests");
            string expectedBranch = "master";
            int hashLength = 7;
            string expectedHash = "4197466ed7682542b4669e98fd962a3925ccaadf".Substring(0, hashLength);
            string expectedUrl = @"https://github.com/Zingabopp/BeatSaberModdingTools.Tasks";
            GetCommitInfo task = new MockGetCommitHash("Test.git")
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
            Assert.AreEqual(expectedUrl, task.OriginUrl);
        }

        [TestMethod]
        public void DefaultHashLength_Manual()
        {
            string directory = Path.Combine(DataFolder, "GitTests");
            string expectedBranch = "master";
            int hashLength = 7;
            string expectedHash = "4197466ed7682542b4669e98fd962a3925ccaadf".Substring(0, hashLength);
            GetCommitInfo task = new MockGetCommitHash("Test.git")
            {
                ProjectDir = directory,
                NoGit = true
            };
            Assert.IsTrue(task.Execute());
            Console.WriteLine($"Branch: {task.Branch}");
            Assert.AreEqual(expectedBranch, task.Branch);
            Console.WriteLine($"Hash: {task.CommitHash}");
            Assert.AreEqual(expectedHash, task.CommitHash);
            Assert.AreEqual(hashLength, task.CommitHash.Length);
        }

        [TestMethod]
        public void NormalStatus_Unmodified()
        {
            string directory = Path.Combine(DataFolder, "GitTests");
            string statusStr = "On branch master\nYour branch is up to date with 'origin/master'.\n\nnothing to commit, working tree clean";
            string originStr = @"https://github.com/Zingabopp/BeatSaberModdingTools";
            string hash = "aadfa8f8af8a8f8af8a8fa";

            string expectedOrigin = originStr;
            string expectedHash = hash.Substring(0, 7);
            string expectedModified = "Unmodified";

            IGitRunner gitRunner = new MockGitRunner
                (
                (GitArgument.Status, statusStr),
                (GitArgument.OriginUrl, originStr),
                (GitArgument.CommitHash, hash))
                ;
            GetCommitInfo task = new MockGetCommitHash("Test.git")
            {
                GitRunner = gitRunner,
                ProjectDir = directory
            };
            task.Execute();
            Assert.AreEqual("master", task.Branch);
            Assert.AreEqual(expectedHash, task.CommitHash);
            Assert.AreEqual(expectedOrigin, task.OriginUrl);
            Assert.AreEqual(expectedModified, task.Modified);
        }

        [TestMethod]
        public void UntrackedFiles()
        {
            string directory = Path.Combine(DataFolder, "GitTests");
            string statusStr = "On branch master\nYour branch is up to date with 'origin/master'.\n\nUntracked files:\n  (use \"git add <file>...\" to include in what will be committed)\n	        Refs / Beat Saber_Data / Managed / IPA.Injector.dll\n        Refs / Libs / Mono.Cecil.Mdb.dll\n        Refs / Libs / Mono.Cecil.Pdb.dll\n        Refs / Libs / Mono.Cecil.Rocks.dll\n        Refs / Libs / Mono.Cecil.dll\n        bsfiles.zip\n\nnothing added to commit but untracked files present(use \"git add\" to track)";

            string originStr = @"https://github.com/Zingabopp/BeatSaberModdingTools";
            string hash = "aadfa8f8af8a8f8af8a8fa";

            string expectedOrigin = originStr;
            string expectedHash = hash.Substring(0, 7);
            string expectedModified = "Unmodified";

            IGitRunner gitRunner = new MockGitRunner
                (
                (GitArgument.Status, statusStr),
                (GitArgument.OriginUrl, originStr),
                (GitArgument.CommitHash, hash))
                ;
            GetCommitInfo task = new MockGetCommitHash("Test.git")
            {
                GitRunner = gitRunner,
                ProjectDir = directory
            };
            task.Execute();
            Assert.AreEqual("master", task.Branch);
            Assert.AreEqual(expectedHash, task.CommitHash);
            Assert.AreEqual(expectedOrigin, task.OriginUrl);
            Assert.AreEqual(expectedModified, task.Modified);
        }

        [TestMethod]
        public void PullRequestStatus()
        {
            string directory = Path.Combine(DataFolder, "GitTests");
            string statusStr = "HEAD detached at pull/11/merge\nnothing to commit, working tree clean";
            string originStr = @"https://github.com/Zingabopp/BeatSaberModdingTools";
            string hash = "aadfa8f8af8a8f8af8a8fa";

            string expectedOrigin = originStr;
            string expectedHash = hash.Substring(0, 7);
            string expectedModified = "Unmodified";

            IGitRunner gitRunner = new MockGitRunner
                (
                (GitArgument.Status, statusStr),
                (GitArgument.OriginUrl, originStr),
                (GitArgument.CommitHash, hash))
                ;
            GetCommitInfo task = new MockGetCommitHash("Test.git")
            {
                GitRunner = gitRunner,
                ProjectDir = directory
            };
            task.Execute();
            Assert.AreEqual("master", task.Branch);
            Assert.AreEqual(expectedHash, task.CommitHash);
            Assert.AreEqual(expectedOrigin, task.OriginUrl);
            Assert.AreEqual(expectedModified, task.Modified);
        }

#if !NCRUNCH
        [TestMethod]
        public void GetGitStatus_Test()
        {
            string directory = Environment.CurrentDirectory;
            IGitRunner gitRunner = new GitCommandRunner(directory);
            string expectedUser = "Zingabopp";
            MockTaskLogger logger = new MockTaskLogger();
            GetCommitInfo.ExtendedLogging = true;
            GitInfo status = GetCommitInfo.GetGitStatus(gitRunner, logger);
            Assert.IsFalse(string.IsNullOrEmpty(status.Branch), $"Branch should not be null/empty.\n{string.Join('\n', logger.LogEntries.Select(e => e.ToString()))}");
            Assert.IsFalse(string.IsNullOrEmpty(status.Modified));
            Assert.IsTrue(status.Modified == "Unmodified" || status.Modified == "Modified");
            Assert.IsFalse(string.IsNullOrWhiteSpace(status.GitUser));
            //Assert.AreEqual(expectedUser, status.GitUser);
        }
        [TestMethod]
        public void TryGetCommitHash_Test()
        {
            string directory = Environment.CurrentDirectory;
            IGitRunner gitRunner = new GitCommandRunner(directory);
            bool success = GetCommitInfo.TryGetGitCommit(gitRunner, null, out string commitHash);
            Assert.IsTrue(success);
            Assert.IsTrue(commitHash.Length > 0);
        }
#endif
        #endregion
    }
}
