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
        /// Executes the task.
        /// </summary>
        /// <returns>true if successful</returns>
        public override bool Execute()
        {
            try
            {
                if (string.IsNullOrEmpty(ProcessName))
                {
                    return false;
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
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}
