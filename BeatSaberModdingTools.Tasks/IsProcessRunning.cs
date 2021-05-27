using BeatSaberModdingTools.Tasks.Utilities;
using BeatSaberModdingTools.Tasks.Utilities.Mock;
using Microsoft.Build.Framework;
using System;
using System.Diagnostics;

namespace BeatSaberModdingTools.Tasks
{
    /// <summary>
    /// Checks if a process is currently running.
    /// </summary>
    public class IsProcessRunning : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Name of the process.
        /// </summary>
        [Required]
        public virtual string ProcessName { get; set; }

        /// <summary>
        /// Return this value if IsProcessRunning fails.
        /// Defaults to true.
        /// </summary>
        public virtual bool Fallback { get; set; } = true;

        /// <summary>
        /// True if the process is running, false otherwise.
        /// </summary>
        [Output]
        public virtual bool IsRunning { get; set; }


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
                Logger = new LogWrapper(Log, GetType().Name);
            else
                Logger = new MockTaskLogger(GetType().Name);
            PlatformID platform = Environment.OSVersion.Platform;
            if (platform != PlatformID.Win32NT)
            {
                Logger.LogMessage(MessageImportance.High, $"This task isn't supported on platform: '{platform}'");
                IsRunning = Fallback;
                return true;
            }
            string errorCode = null;
            try
            {
                if (string.IsNullOrEmpty(ProcessName))
                {
                    errorCode = MessageCodes.IsProcessRunning.EmptyProcessName;
                    throw new ArgumentNullException(nameof(ProcessName), $"{nameof(ProcessName)} cannot be empty.");
                }
                foreach (Process proc in Process.GetProcesses())
                {
                    if (proc.ProcessName.Contains(ProcessName))
                    {
                        IsRunning = true;
                        break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(errorCode))
                    errorCode = MessageCodes.IsProcessRunning.GeneralFailure;
                int line = 0;
                int column = 0;
                string projectFile = null;
                if (BuildEngine != null)
                {
                    line = BuildEngine.LineNumberOfTaskNode;
                    column = BuildEngine.ColumnNumberOfTaskNode;
                    projectFile = BuildEngine.ProjectFileOfTaskNode;
                }
                Logger.LogError(null, errorCode, null, projectFile, line, column, line, column, $"Error in {GetType().Name}: {ex.Message}");
                IsRunning = Fallback;
                return true;
            }
        }
    }
}
