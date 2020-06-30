using BeatSaberModdingTools.Tasks.Utilties;
using Microsoft.Build.Framework;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

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
        /// First 8 characters of the current commit hash.
        /// </summary>
        [Output]
        public virtual string CommitShortHash { get; protected set; }

        /// <summary>
        /// <see cref="ITaskLogger"/> instance used.
        /// </summary>
        public ITaskLogger Logger;

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
            CommitShortHash = "local";
            string errorCode = null;
            try
            {
                try
                {
                    ProjectDir = Path.GetFullPath(ProjectDir);
                    Process process = new Process();
                    string arg = "rev-parse HEAD";
                    process.StartInfo = new ProcessStartInfo("git", arg);
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WorkingDirectory = ProjectDir;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
                    string outText = process.StandardOutput.ReadToEnd();
                    if (outText.Length >= 7)
                    {
                        CommitShortHash = outText.Substring(0, 7);
                        return true;
                    }
                }
                catch (Win32Exception)
                {
                }

                string gitPath = Path.GetFullPath(Path.Combine(ProjectDir, ".git"));
                string headPath = Path.Combine(gitPath, "HEAD");
                string headContents = null;
                if (File.Exists(headPath))
                    headContents = File.ReadAllText(headPath);
                else
                {
                    gitPath = Path.GetFullPath(Path.Combine(ProjectDir, "..", ".git"));
                    headPath = Path.Combine(gitPath, "HEAD");
                    if (File.Exists(headPath))
                        headContents = File.ReadAllText(headPath);
                }
                headPath = null;
                if (!string.IsNullOrEmpty(headContents) && headContents.StartsWith("ref:"))
                    headPath = Path.Combine(gitPath, headContents.Replace("ref:", "").Trim());
                if (File.Exists(headPath))
                {
                    headContents = File.ReadAllText(headPath);
                    if (headContents.Length >= 7)
                        CommitShortHash = headContents.Substring(0, 7);
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
                    Logger.LogMessage("Build", errorCode, null, BuildEngine.ProjectFileOfTaskNode, line, column, line, column,
                        MessageImportance.High, $"Error in {GetType().Name}: {ex.Message}");
                }
                else
                {
                    Logger.LogMessage("Build", errorCode, null, null, 0, 0, 0, 0,
                        MessageImportance.High, $"Error in {GetType().Name}: {ex.Message}");
                }
            }
            if (CommitShortHash == "local")
            {
                if (BuildEngine != null)
                {
                    errorCode = MessageCodes.GetCommitHash.GitNoRepository;
                    int line = BuildEngine.LineNumberOfTaskNode;
                    int column = BuildEngine.ColumnNumberOfTaskNode;
                    Logger.LogMessage("Build", errorCode, null, BuildEngine.ProjectFileOfTaskNode, line, column, line, column,
                        MessageImportance.High, "Project does not appear to be in a git repository.");
                }
                else
                    Logger.LogMessage("Build", errorCode, null, null, 0, 0, 0, 0,
                        MessageImportance.High, "Project does not appear to be in a git repository.");
            }
            return true;
        }
    }
}
