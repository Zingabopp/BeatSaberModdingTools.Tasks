using Microsoft.Build.Framework;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace BSMTTasks
{
    public class GetCommitHash : Microsoft.Build.Utilities.Task
    {
        [Required]
        public virtual string ProjectDir { get; set; }
        [Output]
        public virtual string CommitShortHash { get; protected set; }

        public override bool Execute()
        {
            CommitShortHash = "local";
            bool noGitFound = false;
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
                noGitFound = true;

            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return true;
            }
            try
            {
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
            catch { }
            if (CommitShortHash == "local")
            {
                if (noGitFound)
                    Log.LogMessage(MessageImportance.High, "   'git' command not found, unable to retrieve current commit hash.");
                else
                    Log.LogMessage(MessageImportance.High, "   Unable to retrieve current commit hash.");
            }
            return true;
        }
    }
}
