using System;
using System.Diagnostics;

namespace BeatSaberModdingTools.Tasks.Utilities
{
    /// <summary>
    /// Runs and retrieves output from the 'git' command.
    /// </summary>
    public class GitCommandRunner : IGitRunner
    {
        private const string Command = "git";
        /// <summary>
        /// Working directory to run the command in.
        /// </summary>
        public readonly string WorkingDirectory;
        /// <summary>
        /// Logger for the command runner.
        /// </summary>
        protected ITaskLogger Logger;
        /// <summary>
        /// Creates a new <see cref="GitCommandRunner"/> with an optional <see cref="ITaskLogger"/>.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="logger"></param>
        public GitCommandRunner(string workingDirectory, ITaskLogger logger = null)
        {
            WorkingDirectory = workingDirectory;
            Logger = logger;
        }
        /// <inheritdoc/>
        public string GetTextFromProcess(GitArgument command)
        {
            string outText = null;
            string arg = GetArg(command);
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo(Command, arg)
                    {
                        UseShellExecute = false,
                        WorkingDirectory = this.WorkingDirectory,
                        RedirectStandardOutput = true
                    };
                    process.Start();
                    bool exited = process.WaitForExit(1000);
                    if (!exited || !process.HasExited)
                    {
                        Logger?.LogWarning($"Process '{command} {arg}' timed out, killing.");
                        process.Kill();
                    }
                    outText = process.StandardOutput.ReadToEnd()?.Trim();
                }
            }
            catch (Exception ex)
            {
                throw new GitRunnerException(ex.Message, ex);
            }
            return outText;
        }

        private static string GetArg(GitArgument command)
        {
            switch (command)
            {
                case GitArgument.None:
                    return string.Empty;
                case GitArgument.Status:
                    return "status";
                case GitArgument.CommitHash:
                    return "rev-parse HEAD";
                case GitArgument.OriginUrl:
                    return "config --local --get remote.origin.url";
                default:
                    break;
            }
            return string.Empty;
        }
    }
}
