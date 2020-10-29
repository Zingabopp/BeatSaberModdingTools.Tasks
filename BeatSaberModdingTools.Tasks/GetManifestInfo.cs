using BeatSaberModdingTools.Tasks.Utilities;
using Microsoft.Build.Framework;
using System;
using System.IO;
using static BeatSaberModdingTools.Tasks.Utilities.MessageCodes;

namespace BeatSaberModdingTools.Tasks
{
    /// <summary>
    /// Reads a BSIPA manifest json file and outputs information from the manifest.
    /// </summary>
    public class GetManifestInfo : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// <see cref="ITaskLogger"/> instance used.
        /// </summary>
        public ITaskLogger Logger;
        /// <summary>
        /// Beat Saber game version the mod or lib is compatible with, as reported by the manifest.
        /// </summary>
        [Output]
        public virtual string GameVersion { get; protected set; }
        /// <summary>
        /// The mod or lib's version as reported by the manifest.
        /// </summary>
        [Output]
        public virtual string PluginVersion { get; protected set; }
        /// <summary>
        /// The mod or lib's version as reported by the manifest with prerelease labels stripped.
        /// Use this for comparing to the AssemblyVersion metadata.
        /// </summary>
        [Output]
        public virtual string BasePluginVersion { get; protected set; }

        /// <summary>
        /// Optional: Path to the manifest file. Default is 'manifest.json'.
        /// </summary>
        public virtual string ManifestPath { get; set; }
        /// <summary>
        /// If enabled, this task will report a failure if it cannot parse the Plugin version or Game version.
        /// </summary>
        public virtual bool FailOnError { get; set; }

        #region Removed
        /// <summary>
        /// Moved to <see cref="CompareVersions"/>.
        /// Version of the assembly.
        /// </summary>
        [Output]
        public virtual string AssemblyVersion
        {
            get => throw new NotSupportedException(GetUnsupported(nameof(AssemblyVersion)));
            set => throw new NotSupportedException(GetUnsupported(nameof(AssemblyVersion)));
        }
        /// <summary>
        /// Moved to <see cref="CompareVersions"/>. 
        /// Optional: Skip trying to read the assembly version of the project and use this value instead. 
        /// Useful if the project already has a property with the assembly version.
        /// </summary>
        public virtual string KnownAssemblyVersion
        {
            get => throw new NotSupportedException(GetUnsupported(nameof(KnownAssemblyVersion)));
            set => throw new NotSupportedException(GetUnsupported(nameof(KnownAssemblyVersion)));
        }
        /// <summary>
        /// Moved to <see cref="CompareVersions"/>. 
        /// Optional: Path to the file containing the assembly information. Default is 'Properties\AssemblyInfo.cs'.
        /// </summary>
        public virtual string AssemblyInfoPath
        {
            get => throw new NotSupportedException(GetUnsupported(nameof(AssemblyInfoPath)));
            set => throw new NotSupportedException(GetUnsupported(nameof(AssemblyInfoPath)));
        }
        /// <summary>
        /// Moved to <see cref="CompareVersions"/>. 
        /// If enabled, this task will report a failure if the assembly version and manifest version don't match or there was a problem getting the value for either of them.
        /// </summary>
        public virtual bool ErrorOnMismatch
        {
            get => throw new NotSupportedException(GetUnsupported(nameof(ErrorOnMismatch)));
            set => throw new NotSupportedException(GetUnsupported(nameof(ErrorOnMismatch)));
        }

        private static string GetUnsupported(string memberName)
        {
            return $"{memberName} is not supported by this version of {MessageCodes.Name}, use {nameof(CompareVersions)}.";
        }
        #endregion
        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>true if successful</returns>
        public override bool Execute()
        {
            GameVersion = ErrorString;
            PluginVersion = ErrorString;
            string errorCode = null;
            FilePosition versionPosition = default;
            string manifestFile = ManifestPath;
            if (this.BuildEngine != null)
                Logger = new LogWrapper(Log);
            else
                Logger = new MockTaskLogger();
            try
            {
                if (string.IsNullOrEmpty(manifestFile))
                    manifestFile = "manifest.json";
                string manifest_gameVerStart = "\"gameVersion\"";
                string manifest_versionStart = "\"version\"";
                string manifest_gameVerLine = null;
                string manifest_versionLine = null;
                if (!File.Exists(manifestFile))
                {
                    errorCode = MessageCodes.GetManifestInfo.ManifestFileNotFound;
                    throw new FileNotFoundException($"Manifest file not found at {manifestFile}");

                }
                string line;
                int manifestVersionLineNum = 1;
                int lineNum = 1;
                using (StreamReader manifestStream = new StreamReader(manifestFile))
                {
                    while ((line = manifestStream.ReadLine()) != null && (manifest_versionLine == null || manifest_gameVerLine == null))
                    {
                        line = line.Trim();
                        if (line.StartsWith(manifest_gameVerStart))
                        {
                            manifest_gameVerLine = line;
                        }
                        else if (line.StartsWith(manifest_versionStart))
                        {
                            manifest_versionLine = line;
                            manifestVersionLineNum = lineNum;
                            versionPosition = new FilePosition(lineNum);
                        }
                        lineNum++;
                    }
                }
                if (!string.IsNullOrEmpty(manifest_versionLine))
                {
                    try
                    {
                        string versionStr = manifest_versionLine.Substring(manifest_versionStart.Length).Replace(":", "").Replace("\"", "").TrimEnd(',').Trim();
                        PluginVersion = string.Join(".", Util.ParseVersionString(versionStr));
                        BasePluginVersion = Util.StripVersionLabel(PluginVersion);
                    }
                    catch (ParsingException ex)
                    {
                        Logger.LogError(null, MessageCodes.GetManifestInfo.VersionParseFail, "", manifestFile, versionPosition,
                            $"Error reading version in manifest: {ex.Message}");
                        return false;
                    }
                }
                else
                {
                    Logger.LogError(null, MessageCodes.GetManifestInfo.PluginVersionNotFound, "",
                        manifestFile, default(FilePosition), "PluginVersion not found in {0}", manifestFile);
                    PluginVersion = ErrorString;
                    if (FailOnError)
                        return false;
                }

                if (!string.IsNullOrEmpty(manifest_gameVerLine))
                {
                    GameVersion = manifest_gameVerLine.Substring(manifest_gameVerStart.Length).Replace(":", "").Replace("\"", "").TrimEnd(',').Trim();
                }
                else
                {
                    Logger.LogError(null, MessageCodes.GetManifestInfo.GameVersionNotFound, "",
                        manifestFile, 0, 0, 0, 0, "GameVersion not found in {0}", manifestFile);
                    GameVersion = ErrorString;
                    if (FailOnError)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(errorCode))
                    errorCode = MessageCodes.GetManifestInfo.GeneralFailure;
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