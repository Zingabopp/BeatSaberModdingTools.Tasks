using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using BeatSaberModdingTools.Tasks.Utilties;
using Microsoft.Build.Tasks;

namespace BeatSaberModdingTools.Tasks
{
    /// <summary>
    /// Reads a BSIPA manifest json file, compares the values to the project's assembly version, and outputs information from the manifest.
    /// </summary>
    public class GetManifestInfo : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Default output when a property can't be read.
        /// </summary>
        public const string ErrorString = "E.R.R";
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
        /// Version of the assembly.
        /// </summary>
        [Output]
        public virtual string AssemblyVersion { get; protected set; }

        /// <summary>
        /// Optional: Skip trying to read the assembly version of the project and use this value instead. Useful if the project already has a property with the assembly version.
        /// </summary>
        public virtual string KnownAssemblyVersion { get; set; }
        /// <summary>
        /// Optional: Path to the manifest file. Default is 'manifest.json'.
        /// </summary>
        public virtual string ManifestPath { get; set; }
        /// <summary>
        /// Optional: Path to the file containing the assembly information. Default is 'Properties\AssemblyInfo.cs'.
        /// </summary>
        public virtual string AssemblyInfoPath { get; set; }
        /// <summary>
        /// If enabled, this task will report a failure if the assembly version and manifest version don't match or there was a problem getting the value for either of them.
        /// </summary>
        public virtual bool ErrorOnMismatch { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            GameVersion = ErrorString;
            PluginVersion = ErrorString;
            AssemblyVersion = ErrorString;
            if (this.BuildEngine != null)
                Logger = new LogWrapper(Log);
            else
                Logger = new MockTaskLogger();
            try
            {
                string manifestFile = ManifestPath;
                if (string.IsNullOrEmpty(manifestFile))
                    manifestFile = "manifest.json";
                string manifest_gameVerStart = "\"gameVersion\"";
                string manifest_versionStart = "\"version\"";
                string manifest_gameVerLine = null;
                string manifest_versionLine = null;
                if (!File.Exists(manifestFile))
                {
                    Logger.LogError($"Manifest file not found at {manifestFile}");
                    return false;
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
                        }
                        lineNum++;
                    }
                }
                if (!string.IsNullOrEmpty(manifest_versionLine))
                {
                    PluginVersion = manifest_versionLine.Substring(manifest_versionStart.Length).Replace(":", "").Replace("\"", "").TrimEnd(',').Trim();
                }
                else
                {
                    Logger.LogError("Build", "BSMOD04", "", manifestFile, 0, 0, 0, 0, "PluginVersion not found in {0}", manifestFile);
                    PluginVersion = "E.R.R";
                    if (ErrorOnMismatch)
                        return false;
                }

                if (!string.IsNullOrEmpty(manifest_gameVerLine))
                {
                    GameVersion = manifest_gameVerLine.Substring(manifest_gameVerStart.Length).Replace(":", "").Replace("\"", "").TrimEnd(',').Trim();
                }
                else
                {
                    Logger.LogError("Build", "BSMOD05", "", manifestFile, 0, 0, 0, 0, "GameVersion not found in {0}", manifestFile);
                    GameVersion = "E.R.R";
                    if (ErrorOnMismatch)
                        return false;
                }
                string assemblyFileMsg = "";
                if (!string.IsNullOrEmpty(KnownAssemblyVersion))
                    AssemblyVersion = KnownAssemblyVersion.Trim();
                else
                {
                    string filepath;
                    if (string.IsNullOrEmpty(AssemblyInfoPath))
                        filepath = Path.Combine("Properties", "AssemblyInfo.cs");
                    else
                        filepath = AssemblyInfoPath;
                    try
                    {
                        AssemblyVersion = GetAssemblyVersion(filepath, ErrorOnMismatch);
                    }
                    catch (FileNotFoundException ex)
                    {
                        if (ErrorOnMismatch)
                            throw;
                        else
                            Logger.LogErrorFromException(ex);
                    }
                    assemblyFileMsg = " in " + filepath;
                }
                if (AssemblyVersion == null || AssemblyVersion == "E.R.R" || AssemblyVersion.Length == 0)
                {
                    Logger.LogError("AssemblyVersion could not be determined.");
                    return false;
                }
                if (PluginVersion != "E.R.R" && AssemblyVersion != PluginVersion)
                {
                    if (ErrorOnMismatch)
                    {
                        Logger.LogError("Build", "BSMOD01", "", manifestFile, manifestVersionLineNum, 1, manifestVersionLineNum, 1, "PluginVersion {0} in {1} does not match AssemblyVersion {2}{3}", PluginVersion, manifestFile, AssemblyVersion, assemblyFileMsg);
                        return false;
                    }
                    Logger.LogMessage(MessageImportance.High, "PluginVersion {0} does not match AssemblyVersion {1}", PluginVersion, AssemblyVersion);
                }

                return true;
            }
            catch (ParsingException ex)
            {
                ex.LogErrorFromException(Logger);
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogErrorFromException(ex);
                return false;
            }
        }

        public string GetAssemblyVersion(string assemblyFile, bool errorOnMismatch)
        {
            string assemblyVersionStart = "[assembly: AssemblyVersion(\"";
            string assemblyFileVersionStart = "[assembly: AssemblyFileVersion(\"";
            string assemblyFileVersion;
            string assemblyVersionString = null;
            string assemblyFileVersionString = null;
            int assemblyVersionLineNum = 0;
            int assemblyFileVersionLineNum = 0;
            int startColumn;
            int endColumn;
            string line;
            int currentLine = 1;
            string assemblyVersion = null;

            if (!File.Exists(assemblyFile))
            {
                throw new FileNotFoundException("Could not find AssemblyInfo: " + assemblyFile);
            }
            using (StreamReader assemblyStream = new StreamReader(assemblyFile))
            {
                while ((line = assemblyStream.ReadLine()) != null)
                {
                    if (line.Trim().StartsWith(assemblyVersionStart))
                    {
                        assemblyVersionString = line;
                        assemblyVersionLineNum = currentLine;
                    }
                    if (line.Trim().StartsWith(assemblyFileVersionStart))
                    {
                        assemblyFileVersionString = line;
                        assemblyFileVersionLineNum = currentLine;
                    }
                    currentLine++;
                }
            }
            if (!string.IsNullOrEmpty(assemblyVersionString))
            {
                startColumn = assemblyVersionString.IndexOf('"') + 1;
                endColumn = assemblyVersionString.LastIndexOf('"');
                if (startColumn > 0 && endColumn > 0)
                    assemblyVersion = assemblyVersionString.Substring(startColumn, endColumn - startColumn);
            }
            else
            {
                if (ErrorOnMismatch)
                    throw new ParsingException("Build", "BSMOD03", "", assemblyFile, 0, 0, 0, 0, "Unable to parse the AssemblyVersion from {0}", assemblyFile);
                Logger.LogWarning("Build", "BSMOD03", "", assemblyFile, 0, 0, 0, 0, "Unable to parse the AssemblyVersion from {0}", assemblyFile);
                return ErrorString;
            }

            if (!string.IsNullOrEmpty(assemblyFileVersionString))
            {
                startColumn = assemblyFileVersionString.IndexOf('"') + 1;
                endColumn = assemblyFileVersionString.LastIndexOf('"');
                int lenth = endColumn - startColumn;
                if (startColumn > 0 && endColumn > 0 && lenth > 0)
                {
                    assemblyFileVersion = assemblyFileVersionString.Substring(startColumn, endColumn - startColumn);
                    if (assemblyVersion != assemblyFileVersion)
                    {
                        string message = "AssemblyVersion {0} does not match AssemblyFileVersion {1} in AssemblyInfo.cs";
                        if (errorOnMismatch)
                            throw new ParsingException("Build", "BSMOD02", "", assemblyFile, assemblyFileVersionLineNum, startColumn + 1, assemblyFileVersionLineNum, endColumn + 1, message, assemblyVersion, assemblyFileVersion);
                        Logger.LogWarning("Build", "BSMOD02", "", assemblyFile, assemblyFileVersionLineNum, startColumn + 1, assemblyFileVersionLineNum, endColumn + 1, message, assemblyVersion, assemblyFileVersion);
                    }

                }
                else
                {
                    startColumn = Math.Max(0, startColumn);
                    endColumn = startColumn;
                    string message = "Unable to parse the AssemblyFileVersion from {0}";
                    if (errorOnMismatch)
                        throw new ParsingException("Build", "BSMOD06", "", assemblyFile, assemblyFileVersionLineNum, startColumn, assemblyFileVersionLineNum, endColumn, message, assemblyFile);
                    Logger.LogWarning("Build", "BSMOD06", "", assemblyFile, assemblyFileVersionLineNum, startColumn, assemblyFileVersionLineNum, endColumn, message, assemblyFile);
                }
            }
            return assemblyVersion;
        }
    }

    public static class ParsingExceptionExtensions
    {
        public static void LogErrorFromException(this ParsingException ex, ITaskLogger Log)
        {
            Log.LogError("Build", "BSMOD02", "", ex.File, ex.LineNumber, ex.ColumnNumber, ex.EndLineNumber, ex.EndColumnNumber, ex.Message, ex.MessageArgs);
        }
    }

    public class ParsingException : Exception
    {
        public readonly string SubCategory;
        public readonly string WarningCode;
        public readonly string HelpKeyword;
        public readonly string File;
        public readonly int LineNumber;
        public readonly int ColumnNumber;
        public readonly int EndLineNumber;
        public readonly int EndColumnNumber;
        public readonly object[] MessageArgs;

        public ParsingException(string message) : base(message)
        {
        }

        public ParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ParsingException(string subCategory, string warningCode, string helpKeyword,
            string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber,
            string message, params object[] messageArgs)
            : base(message)
        {
            SubCategory = subCategory;
            WarningCode = warningCode;
            HelpKeyword = helpKeyword;
            File = file;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            EndLineNumber = endLineNumber;
            EndColumnNumber = endColumnNumber;
            MessageArgs = messageArgs;
        }
    }
}