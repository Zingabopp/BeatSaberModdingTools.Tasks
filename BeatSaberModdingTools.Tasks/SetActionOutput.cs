using System;
using System.IO;
using BeatSaberModdingTools.Tasks.Utilities;
using BeatSaberModdingTools.Tasks.Utilities.Mock;
using Microsoft.Build.Framework;

namespace BeatSaberModdingTools.Tasks
{
    /// <summary>
    /// Writes data to a GitHub Actions environment file
    /// </summary>
    public class SetActionOutput : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// <see cref="ITaskLogger"/> instance used.
        /// </summary>
        public ITaskLogger Logger;

        /// <summary>
        /// The output variable name
        /// </summary>
        [Required]
        public virtual string OutputName { get; set; }

        /// <summary>
        /// The output variable value
        /// </summary>
        [Required]
        public virtual string OutputValue { get; set; }
        /// <summary>
        /// Optional: Path to the file to write to
        /// </summary>
        public virtual string OutputPath { get; set; }
        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>true if successful</returns>
        public override bool Execute()
        {
            string errorCode = null;
            if (this.BuildEngine != null)
                Logger = new LogWrapper(Log, GetType().Name);
            else
                Logger = new MockTaskLogger(GetType().Name);
            try
            {
                string outputPath = null;
                if (!string.IsNullOrWhiteSpace(OutputPath))
                {
                    outputPath = OutputPath;
                }
                else
                {
                    outputPath = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
                }
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    string errorMessage = "'GithubOutputPath' property was not set and/or 'GITHUB_OUTPUT' has no value";
                    
                    if (BuildEngine != null)
                    {
                        int line = BuildEngine.LineNumberOfTaskNode;
                        int column = BuildEngine.ColumnNumberOfTaskNode;
                        Logger.LogError(null, errorCode, null, BuildEngine.ProjectFileOfTaskNode, line, column, line, column, $"Error in {GetType().Name}: {errorMessage}");
                    }
                    else
                    {
                        Logger.LogError(null, errorCode, null, null, 0, 0, 0, 0, $"Error in {GetType().Name}: {errorMessage}");
                    }
                    return false;

                }
                if (string.IsNullOrWhiteSpace(OutputName))
                {
                    if (BuildEngine != null)
                    {
                        int line = BuildEngine.LineNumberOfTaskNode;
                        int column = BuildEngine.ColumnNumberOfTaskNode;
                        Logger.LogError(null, errorCode, null, BuildEngine.ProjectFileOfTaskNode, line, column, line, column, $"Error in {GetType().Name}: OutputName was not set");
                    }
                    else
                    {
                        Logger.LogError(null, errorCode, null, null, 0, 0, 0, 0, $"Error in {GetType().Name}: OutputName was not set");
                    }
                    return false;
                }
                FileInfo outputFile = new FileInfo(outputPath);
                outputFile.Directory.Create();
                File.AppendAllText(outputPath, $"{OutputName}={OutputValue}\n");
                return true;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(errorCode))
                    errorCode = MessageCodes.SetActionOutput.GeneralFailure;
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
