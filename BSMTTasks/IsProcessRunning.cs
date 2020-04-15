using Microsoft.Build.Framework;
using System;
using System.Diagnostics;

namespace BSMTTasks
{
    public class IsProcessRunning : Microsoft.Build.Utilities.Task
    {
        [Required]
        public virtual string ProcessName { get; set; }
        [Output]
        public virtual bool IsRunning { get; set; }

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
