using BeatSaberModdingTools.Tasks.Utilities;
using BeatSaberModdingTools.Tasks.Utilities.Mock;
using Microsoft.Build.Framework;
using System;

namespace BeatSaberModdingTools.Tasks
{
    /// <summary>
    /// Compares an assembly and manifest version string, logs an error and optionally fails if they don't match.
    /// </summary>
    public class CompareVersions : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// <see cref="ITaskLogger"/> instance used.
        /// </summary>
        public ITaskLogger Logger;

        /// <summary>
        /// The mod or lib's version as reported by the manifest.
        /// </summary>
        [Required]
        public virtual string PluginVersion { get; set; }

        /// <summary>
        /// The mod or library's version as reported by the assembly.
        /// </summary>
        [Required]
        public virtual string AssemblyVersion { get; set; }

        /// <summary>
        /// If enabled, this task will report a failure if the assembly version and manifest version don't match or there was a problem getting the value for either of them.
        /// </summary>
        public virtual bool ErrorOnMismatch { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>true if successful</returns>
        public override bool Execute()
        {
            string errorCode = null;
            LogMessageLevel errorLevel = ErrorOnMismatch ? LogMessageLevel.Error : LogMessageLevel.Warning;
            AssemblyInfoData asmInfo = default;
            string assemblyInfoPath = null;
            if (this.BuildEngine != null)
                Logger = new LogWrapper(Log, GetType().Name);
            else
                Logger = new MockTaskLogger(GetType().Name);
            try
            {
                if (!Util.MatchVersions(AssemblyVersion, PluginVersion))
                {
                    Logger.Log(null, MessageCodes.CompareVersions.VersionMismatch, "",
                        assemblyInfoPath, asmInfo.AssemblyVersionPosition, errorLevel,
                        "PluginVersion {0} does not match AssemblyVersion {1}.", PluginVersion,
                        AssemblyVersion);
                    if (ErrorOnMismatch)
                        return false;
                }

                return true;
            }
            catch (VersionMatchException ex)
            {
                if (BuildEngine != null)
                {
                    int line = BuildEngine.LineNumberOfTaskNode;
                    int column = BuildEngine.ColumnNumberOfTaskNode;
                    Logger.LogError(null, MessageCodes.CompareVersions.InvalidVersion, null, BuildEngine.ProjectFileOfTaskNode, line, column, line, column, ex.Message);
                }
                else
                {
                    Logger.LogError(null, MessageCodes.CompareVersions.InvalidVersion, null, null, 0, 0, 0, 0, ex.Message);
                }
                return false;
            }
            catch (ParsingException ex)
            {
                if (string.IsNullOrEmpty(errorCode))
                    errorCode = MessageCodes.CompareVersions.GeneralFailure;
                if (BuildEngine != null)
                {
                    int line = BuildEngine.LineNumberOfTaskNode;
                    int column = BuildEngine.ColumnNumberOfTaskNode;
                    Logger.LogError(null, errorCode, null, BuildEngine.ProjectFileOfTaskNode, line, column, line, column, ex.Message);
                }
                else
                {
                    Logger.LogError(null, errorCode, null, null, ex.LineNumber, ex.ColumnNumber, ex.EndLineNumber, ex.EndColumnNumber, ex.Message);
                }
                return false;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(errorCode))
                    errorCode = MessageCodes.CompareVersions.GeneralFailure;
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
