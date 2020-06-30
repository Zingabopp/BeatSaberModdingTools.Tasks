using BeatSaberModdingTools.Tasks.Utilties;
using Microsoft.Build.Framework;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace BeatSaberModdingTools.Tasks
{
    /// <summary>
    /// Gets the git commit short hash of the project.
    /// </summary>
    public class GetCommitHash : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// The directory of the project.
        /// </summary>
        [Required]
        public virtual string ProjectDir { get; set; }
        /// <summary>
        /// Optional: Number of characters to retrieve from the hash.
        /// Default is 7.
        /// </summary>
        public virtual int HashLength { get; set; } = 7;
        /// <summary>
        /// Optional: If true, do not attempt to use 'git' executable.
        /// </summary>
        public virtual bool NoGit { get; set; }
        /// <summary>
        /// Optional: If true, do not attempt to check if files have been changed.
        /// </summary>
        public virtual bool SkipStatus { get; set; }
        /// <summary>
        /// Commit hash up to the number of characters set by <see cref="HashLength"/>.
        /// </summary>
        [Output]
        public virtual string CommitHash { get; protected set; }
        /// <summary>
        /// 'Modified' if the repository has uncommitted changes, 'Unmodified' if it doesn't. Will be left blank if unsupported (Only works if git bash is installed).
        /// </summary>
        [Output]
        public virtual string Modified { get; protected set; }

        /// <summary>
        /// Name of the current repository branch, if available.
        /// </summary>
        [Output]
        public virtual string Branch { get; protected set; }

        /// <summary>
        /// <see cref="ITaskLogger"/> instance used.
        /// </summary>
        public ITaskLogger Logger;

        /// <summary>
        /// Attempts to retrieve the git commit hash using the 'git' program.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="commitHash"></param>
        /// <returns></returns>
        public static bool TryGetGitCommit(string directory, out string commitHash)
        {
            commitHash = null;
            try
            {
                directory = Path.GetFullPath(directory);
                Process process = new Process();
                string arg = "rev-parse HEAD";
                process.StartInfo = new ProcessStartInfo("git", arg);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = directory;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit(1000);
                string outText = process.StandardOutput.ReadToEnd();
                if (outText.Length > 0)
                {
                    commitHash = outText;
                    return true;
                }
            }
            catch (Win32Exception)
            {
            }
            return false;
        }


        /// <summary>
        /// Attempts to check if the repository has uncommitted changes.
        /// </summary>
        /// <returns></returns>
        public static GitInfo GetGitStatus(string directory)
        {
            GitInfo status = new GitInfo();
            try
            {
                directory = Path.GetFullPath(directory);
                Process process = new Process();
                string arg = "status";
                process.StartInfo = new ProcessStartInfo("git", arg);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = directory;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit(1000);
                string outText = process.StandardOutput.ReadToEnd();
                Regex branchName = new Regex(@"^On branch (.*)$", RegexOptions.Multiline);
                Match match = branchName.Match(outText);
                if (match.Success && match.Groups.Count > 1)
                {
                    string branch = match.Groups[1].Value;
                    status.Branch = branch;
                }
                string unmodified = "NOTHING TO COMMIT";
                if (outText.ToUpper().Contains(unmodified))
                    status.Modified = "Unmodified";
                else
                    status.Modified = "Modified";
            }
            catch (Win32Exception)
            {
            }
            return status;
        }

        /// <summary>
        /// Attempts to retrieve the git commit hash by reading git files.
        /// </summary>
        /// <param name="gitPath"></param>
        /// <param name="gitInfo"></param>
        /// <returns></returns>
        public static bool TryGetCommitManual(string gitPath, out GitInfo gitInfo)
        {
            gitInfo = new GitInfo();
            bool success = false;
            string headContents;
            string headPath = Path.Combine(gitPath, "HEAD");
            if (File.Exists(headPath))
            {
                headContents = File.ReadAllText(headPath);
                if (!string.IsNullOrEmpty(headContents) && headContents.StartsWith("ref:"))
                    headPath = Path.Combine(gitPath, headContents.Replace("ref:", "").Trim());
                gitInfo.Branch = headPath.Substring(headPath.LastIndexOf('/') + 1);
                if (File.Exists(headPath))
                {
                    headContents = File.ReadAllText(headPath);
                    if (headContents.Length >= 0)
                    {
                        gitInfo.CommitHash = headContents.Trim();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Attempts to retrieve the git commit hash by reading git files.
        /// </summary>
        /// <param name="gitPaths"></param>
        /// <param name="gitInfo"></param>
        /// <returns></returns>
        public static bool TryGetCommitManual(string[] gitPaths, out GitInfo gitInfo)
        {
            gitInfo = new GitInfo();
            for (int i = 0; i < gitPaths.Length; i++)
            {
                if(TryGetCommitManual(gitPaths[i], out GitInfo retVal))
                {
                    gitInfo = retVal;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>true if successful</returns>
        public override bool Execute()
        {
            if (this.BuildEngine != null)
                Logger = new LogWrapper(Log);
            else
                Logger = new MockTaskLogger();
            CommitHash = "local";
            string errorCode = null;
            try
            {
                string commitHash = null;
                if (!NoGit && TryGetGitCommit(ProjectDir, out commitHash) && commitHash.Length > 0)
                {
                    CommitHash = commitHash.Substring(0, Math.Min(commitHash.Length, HashLength));
                    if (!SkipStatus)
                    {
                        GitInfo gitStatus = GetGitStatus(ProjectDir);
                        if (!string.IsNullOrEmpty(gitStatus.Branch))
                            Branch = gitStatus.Branch;
                        if (!string.IsNullOrEmpty(gitStatus.Modified))
                            Modified = gitStatus.Modified;
                    }
                }
                else
                {
                    string[] gitPaths = new string[]{
                        Path.GetFullPath(Path.Combine(ProjectDir, ".git")),

                        Path.GetFullPath(Path.Combine(ProjectDir, "..", ".git"))
                    };

                    if (TryGetCommitManual(gitPaths, out GitInfo gitInfo))
                    {
                        commitHash = gitInfo.CommitHash;
                        if (commitHash.Length > 0)
                            CommitHash = commitHash
                                .Substring(0, 
                                  Math.Min(commitHash.Length, HashLength));
                        if (!string.IsNullOrEmpty(gitInfo.Branch))
                            Branch = gitInfo.Branch;
                        if (!string.IsNullOrEmpty(gitInfo.Modified))
                            Modified = gitInfo.Modified;
                    }
                }
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(errorCode))
                    errorCode = MessageCodes.GetCommitHash.GitFailed;
                if (BuildEngine != null)
                {
                    int line = BuildEngine.LineNumberOfTaskNode;
                    int column = BuildEngine.ColumnNumberOfTaskNode;
                    Logger.LogMessage(null, errorCode, null, BuildEngine.ProjectFileOfTaskNode, line, column, line, column,
                        MessageImportance.High, $"Error in {GetType().Name}: {ex.Message}");
                }
                else
                {
                    Logger.LogMessage(null, errorCode, null, null, 0, 0, 0, 0,
                        MessageImportance.High, $"Error in {GetType().Name}: {ex.Message}");
                }
            }
            if (CommitHash == "local")
            {
                if (BuildEngine != null)
                {
                    errorCode = MessageCodes.GetCommitHash.GitNoRepository;
                    int line = BuildEngine.LineNumberOfTaskNode;
                    int column = BuildEngine.ColumnNumberOfTaskNode;
                    Logger.LogMessage(null, errorCode, null, BuildEngine.ProjectFileOfTaskNode, line, column, line, column,
                        MessageImportance.High, "Project does not appear to be in a git repository.");
                }
                else
                    Logger.LogMessage(null, errorCode, null, null, 0, 0, 0, 0,
                        MessageImportance.High, "Project does not appear to be in a git repository.");
            }
            return true;
        }
    }

    /// <summary>
    /// Container for data of a git repository.
    /// </summary>
    public struct GitInfo
    {
        /// <summary>
        /// Current commit hash.
        /// </summary>
        public string CommitHash;
        /// <summary>
        /// Current branch of the repository.
        /// </summary>
        public string Branch;
        /// <summary>
        /// 'Modified' if the repository is modified, 'Unmodified' if it's not.
        /// Null/Empty string if undetermined.
        /// </summary>
        public string Modified;
    }
}
