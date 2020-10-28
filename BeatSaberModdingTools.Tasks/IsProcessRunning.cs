using BeatSaberModdingTools.Tasks.Utilities;
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
                Logger = new LogWrapper(Log);
            else
                Logger = new MockTaskLogger();
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
                if (BuildEngine != null)
                {
                    int line = BuildEngine.LineNumberOfTaskNode;
                    int column = BuildEngine.ColumnNumberOfTaskNode;
                    Logger.LogError(null, errorCode, null, BuildEngine.ProjectFileOfTaskNode, line, column, line, column, $"Error in {GetType().Name}: {ex.Message}");
                }
                else
                {
                    Logger.LogError(null, errorCode, null, null, 0, 0, 0, 0, $"Error in {GetType().Name}: {ex.Message}");
                }
                return false;
            }
        }
    }
}
