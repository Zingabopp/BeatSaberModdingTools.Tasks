using BeatSaberModdingTools.Tasks.Utilities;
using Microsoft.Build.Framework;
using System;
using System.IO;
using System.Linq;
using static BeatSaberModdingTools.Tasks.Utilities.MessageCodes;

namespace BeatSaberModdingTools.Tasks
{
    /// <summary>
    /// Verifies the plugin manifest and assembly metadata.
    /// </summary>
    public class VerifyManifest : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// <see cref="ITaskLogger"/> instance used.
        /// </summary>
        public ITaskLogger Logger;

        /// <summary>
        /// Version of the assembly.
        /// </summary>
        [Output]
        public virtual string AssemblyVersion { get; protected set; }

        /// <summary>
        /// The mod or lib's version as reported by the manifest.
        /// </summary>
        public virtual string PluginVersion { get; set; }

        /// <summary>
        /// Optional: Skip trying to read the assembly version of the project and use this value instead.
        /// Useful if the project already has a property with the assembly version.
        /// </summary>
        public virtual string KnownAssemblyVersion { get; set; }

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
        /// <returns>true if successful</returns>
        public override bool Execute()
        {
            string errorCode = null;
            LogMessageLevel errorLevel = ErrorOnMismatch ? LogMessageLevel.Error : LogMessageLevel.Warning;
            AssemblyInfoData asmInfo = default;
            string assemblyInfoPath = null;
            AssemblyVersion = MessageCodes.ErrorString;
            if (this.BuildEngine != null)
                Logger = new LogWrapper(Log);
            else
                Logger = new MockTaskLogger();
            try
            {
                string assemblyFileMsg = "";
                if (!string.IsNullOrWhiteSpace(KnownAssemblyVersion))
                    AssemblyVersion = KnownAssemblyVersion.Trim();
                else
                {
                    if (string.IsNullOrWhiteSpace(AssemblyInfoPath))
                        assemblyInfoPath = Path.Combine("Properties", "AssemblyInfo.cs");
                    else
                        assemblyInfoPath = AssemblyInfoPath;
                    try
                    {
                        asmInfo = ParseAssembly(assemblyInfoPath, ErrorOnMismatch);
                        AssemblyVersion = asmInfo.AssemblyVersion;
                    }
                    catch (FileNotFoundException ex)
                    {
                        if (ErrorOnMismatch)
                        {
                            errorCode = MessageCodes.VerifyManifest.AssemblyInfoNotFound;
                            throw;
                        }
                        else
                            Logger.LogErrorFromException(ex);
                    }
                    assemblyFileMsg = " in " + assemblyInfoPath;
                }
                if (AssemblyVersion == null || AssemblyVersion == ErrorString || AssemblyVersion.Length == 0)
                {
                    Logger.LogError("AssemblyVersion could not be determined.");
                    return false;
                }
                if (PluginVersion != ErrorString && AssemblyVersion != PluginVersion)
                {
                    Logger.Log(null, MessageCodes.VerifyManifest.VersionMismatch, "",
                        assemblyInfoPath, asmInfo.AssemblyVersionPosition, errorLevel,
                        "PluginVersion {0} does not match AssemblyVersion {1}{2}", PluginVersion,
                        AssemblyVersion, assemblyFileMsg);
                    if (ErrorOnMismatch)
                        return false;
                }

                return true;
            }
            catch(ParsingException ex)
            {
                if (string.IsNullOrEmpty(errorCode))
                    errorCode = MessageCodes.VerifyManifest.GeneralFailure;
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
                    errorCode = MessageCodes.VerifyManifest.GeneralFailure;
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

        /// <summary>
        /// Parses the assembly version from the given file.
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="errorOnMismatch"></param>
        /// <returns></returns>
        public AssemblyInfoData ParseAssembly(string assemblyFile, bool errorOnMismatch)
        {
            string assemblyVersionStart = "[assembly: AssemblyVersion(\"";
            string assemblyFileVersionStart = "[assembly: AssemblyFileVersion(\"";
            string assemblyVersionString = null;
            string assemblyFileVersionString = null;
            int asmVerLineNum = 0;
            int asmFileVerLineNum = 0;
            int asmVerStartColumn;
            int asmVerEndColumn;
            int asmFileVerStartColumn;
            int asmFileVerEndColumn;
            Position asmVerPosition = default;
            Position asmFileVerPosition = default;
            string line;
            int currentLine = 1;
            string assemblyVersion = null;
            string assemblyFileVersion = null;

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
                        asmVerLineNum = currentLine;
                    }
                    if (line.Trim().StartsWith(assemblyFileVersionStart))
                    {
                        assemblyFileVersionString = line;
                        asmFileVerLineNum = currentLine;
                    }
                    currentLine++;
                }
            }
            if (!string.IsNullOrEmpty(assemblyVersionString))
            {
                asmVerStartColumn = assemblyVersionString.IndexOf('"') + 1;
                asmVerEndColumn = assemblyVersionString.LastIndexOf('"');
                asmVerPosition = new Position(asmVerLineNum, asmVerStartColumn + 1, asmVerEndColumn + 1);
                if (asmVerStartColumn > 0 && asmVerEndColumn > 0)
                    assemblyVersion = assemblyVersionString.Substring(asmVerStartColumn, asmVerEndColumn - asmVerStartColumn);
            }
            else
            {
                if (ErrorOnMismatch)
                    throw new ParsingException(null, MessageCodes.VerifyManifest.AssemblyFileVersionParseFail,
                        "", assemblyFile, 0, 0, 0, 0, "Unable to parse the AssemblyVersion from {0}", assemblyFile);
                Logger.LogWarning(null, MessageCodes.VerifyManifest.AssemblyFileVersionParseFail,
                    "", assemblyFile, 0, 0, 0, 0, "Unable to parse the AssemblyVersion from {0}", assemblyFile);
                return AssemblyInfoData.AssemblyVersionError();
            }

            if (!string.IsNullOrEmpty(assemblyFileVersionString))
            {
                asmFileVerStartColumn = assemblyFileVersionString.IndexOf('"') + 1;
                asmFileVerEndColumn = assemblyFileVersionString.LastIndexOf('"');
                int lenth = asmFileVerEndColumn - asmFileVerStartColumn;
                if (asmFileVerStartColumn > 0 && asmFileVerEndColumn > 0 && lenth > 0)
                {
                    assemblyFileVersion = assemblyFileVersionString.Substring(asmFileVerStartColumn, asmFileVerEndColumn - asmFileVerStartColumn);
                    if (assemblyVersion != assemblyFileVersion)
                    {
                        string message = "AssemblyVersion {0} does not match AssemblyFileVersion {1} in {2}";
                        if (errorOnMismatch)
                            throw new ParsingException(null, MessageCodes.VerifyManifest.AssemblyVersionMismatch,
                                "", assemblyFile, asmFileVerLineNum, asmFileVerStartColumn + 1, asmFileVerLineNum,
                                asmFileVerEndColumn + 1, message, assemblyVersion, assemblyFileVersion, assemblyFile);
                        Logger.LogWarning(null, MessageCodes.VerifyManifest.AssemblyVersionMismatch,
                            "", assemblyFile, asmFileVerLineNum, asmFileVerStartColumn + 1, asmFileVerLineNum,
                            asmFileVerEndColumn + 1, message, assemblyVersion, assemblyFileVersion, assemblyFile);
                    }

                }
                else
                {
                    asmFileVerStartColumn = Math.Max(0, asmFileVerStartColumn);
                    asmFileVerEndColumn = asmFileVerStartColumn;
                    string message = "Unable to parse the AssemblyFileVersion from {0}";
                    if (errorOnMismatch)
                        throw new ParsingException(null, MessageCodes.VerifyManifest.AssemblyFileVersionParseFail,
                            "", assemblyFile, asmFileVerLineNum, asmFileVerStartColumn,
                            asmFileVerLineNum, asmFileVerEndColumn, message, assemblyFile);
                    Logger.LogWarning(null, MessageCodes.VerifyManifest.AssemblyFileVersionParseFail,
                        "", assemblyFile, asmFileVerLineNum, asmFileVerStartColumn, asmFileVerLineNum,
                        asmFileVerEndColumn, message, assemblyFile);
                }
                asmFileVerPosition = new Position(asmFileVerLineNum, asmFileVerStartColumn, asmFileVerEndColumn);
            }
            return new AssemblyInfoData(assemblyVersion, assemblyFileVersion, asmVerPosition, asmFileVerPosition);
        }
    }

    /// <summary>
    /// Contains data read from the AssemblyInfo file.
    /// </summary>
    public struct AssemblyInfoData
    {
        /// <summary>
        /// Returns an <see cref="AssemblyInfoData"/> where the AssemblyVersion could not be parsed.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static AssemblyInfoData AssemblyVersionError(Position position = default)
        {
            return new AssemblyInfoData(MessageCodes.ErrorString, MessageCodes.ErrorString, position, default);
        }

        /// <summary>
        /// AssemblyVersion parsed from the file. The value will be <see cref="Utilities.MessageCodes.ErrorString"/> if it could not be parsed.
        /// </summary>
        public readonly string AssemblyVersion;
        /// <summary>
        /// AssemblyFileVersion parsed from the file. The value will be <see cref="Utilities.MessageCodes.ErrorString"/> if it could not be parsed.
        /// </summary>
        public readonly string AssemblyFileVersion;
        /// <summary>
        /// Position of the AssemblyVersion string in the file.
        /// </summary>
        public readonly Position AssemblyVersionPosition;
        /// <summary>
        /// Position of the AssemblyFileVersion string in the file.
        /// </summary>
        public readonly Position AssemblyFileVersionPosition;

        /// <summary>
        /// Creates a new <see cref="AssemblyInfoData"/>.
        /// </summary>
        /// <param name="assemblyVersion"></param>
        /// <param name="assemblyFileVersion"></param>
        /// <param name="assemblyVersionPosition"></param>
        /// <param name="assemblyFileVersionPosition"></param>
        public AssemblyInfoData(string assemblyVersion, string assemblyFileVersion, Position assemblyVersionPosition, Position assemblyFileVersionPosition)
        {
            AssemblyVersion = assemblyVersion;
            AssemblyFileVersion = assemblyFileVersion;
            AssemblyVersionPosition = assemblyVersionPosition;
            AssemblyFileVersionPosition = assemblyFileVersionPosition;
        }
    }

    /// <summary>
    /// Defines a position in a file.
    /// </summary>
    public struct Position
    {
        /// <summary>
        /// Line the <see cref="Position"/> starts on.
        /// </summary>
        public readonly int StartLine;
        /// <summary>
        /// Line the <see cref="Position"/> ends on.
        /// </summary>
        public readonly int EndLine;
        /// <summary>
        /// Column the <see cref="Position"/> starts on.
        /// </summary>
        public readonly int StartColumn;
        /// <summary>
        /// Column the <see cref="Position"/> ends on.
        /// </summary>
        public readonly int EndColumn;

        /// <summary>
        /// Defines a new <see cref="Position"/> that covers a single line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="startColumn"></param>
        /// <param name="endColumn"></param>
        public Position(int line, int startColumn = 0, int endColumn = 0)
        {
            StartLine = Math.Max(0, line);
            EndLine = StartLine;
            StartColumn = Math.Max(0, startColumn);
            EndColumn = Math.Max(StartColumn, endColumn);
        }

        /// <summary>
        /// Defines a new <see cref="Position"/> that may cover multiple lines.
        /// </summary>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <param name="startColumn"></param>
        /// <param name="endColumn"></param>
        public Position(int startLine, int endLine, int startColumn, int endColumn)
        {
            StartLine = Math.Max(0, startLine);
            EndLine = Math.Max(startLine, endLine);
            StartColumn = Math.Max(0, startColumn);
            EndColumn = StartLine == EndLine ? Math.Max(StartColumn, endColumn) : Math.Max(0, endColumn);
        }
    }
}
