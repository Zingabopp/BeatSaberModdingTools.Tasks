namespace BeatSaberModdingTools.Tasks.Utilties
{
    /// <summary>
    /// Message codes.
    /// </summary>
    public static class MessageCodes
    {
        /// <summary>
        /// Message codes for GetManifestInfo.
        /// </summary>
        public static class GetManifestInfo 
        {
            /// <summary>
            /// Manifest and assembly versions don't match.
            /// </summary>
            public const string VersionMismatch = "BSMT01";
            /// <summary>
            /// Manifest file doesn't exist.
            /// </summary>
            public const string ManifestFileNotFound = "BSMT02";
            /// <summary>
            /// AssemblyInfo file not found and known assembly version not given.
            /// </summary>
            public const string AssemblyInfoNotFound = "BSMT03";
            /// <summary>
            /// AssemblyVersion and AssemblyFileVersion don't match.
            /// </summary>
            public const string AssemblyVersionMismatch = "BSMT04";
            /// <summary>
            /// AssemblyVersion couldn't be parsed from AssemblyInfo file.
            /// </summary>
            public const string AssemblyVersionParseFail = "BSMT05";
            /// <summary>
            /// PluginVersion could not be determined from tthe manifest.
            /// </summary>
            public const string PluginVersionNotFound = "BSMT06";
            /// <summary>
            /// GameVersion could not be determined from the manifest.
            /// </summary>
            public const string GameVersionNotFound = "BSMT07";
            /// <summary>
            /// AssemblyFileVersion couldn't be determined from AssemblyInfo.
            /// </summary>
            public const string AssemblyFileVersionParseFail = "BSMT08";
            /// <summary>
            /// Other error.
            /// </summary>
            public const string GeneralFailure = "BSMT09";
        }
        /// <summary>
        /// Message codes for IsProcessRunning.
        /// </summary>
        public static class IsProcessRunning
        {
            /// <summary>
            /// <see cref="Tasks.IsProcessRunning.ProcessName"/> was given an empty or null value.
            /// </summary>
            public const string EmptyProcessName = "BSMT10";
            /// <summary>
            /// Other error.
            /// </summary>
            public const string GeneralFailure = "BSMT11";
        }
        /// <summary>
        /// Message codes for ZipDir.
        /// </summary>
        public static class ZipDir
        {
            /// <summary>
            /// Other error.
            /// </summary>
            public const string ZipFailed = "BSMT12";
            /// <summary>
            /// <see cref="Tasks.ZipDir.SourceDirectory"/> was given an empty or null value.
            /// </summary>
            public const string ZipEmptySource = "BSMT13";
            /// <summary>
            /// <see cref="Tasks.ZipDir.DestinationFile"/> was given an empty or null value.
            /// </summary>
            public const string ZipEmptyDestination = "BSMT14";
            /// <summary>
            /// Path specified in <see cref="Tasks.ZipDir.SourceDirectory"/> does not exist.
            /// </summary>
            public const string ZipMissingSource = "BSMT15";
        }
        /// <summary>
        /// Message codes for GetCommitHash.
        /// </summary>
        public static class GetCommitHash
        {
            /// <summary>
            /// Other error.
            /// </summary>
            public const string GitFailed = "BSMT16";
            /// <summary>
            /// Project is not in a git repository.
            /// </summary>
            public const string GitNoRepository = "BSMT17";
        }
        /// <summary>
        /// Message codes for ReplaceInFile.
        /// </summary>
        public static class ReplaceInFile
        {
            /// <summary>
            /// <see cref="Tasks.ReplaceInFile.File"/> is null or empty.
            /// </summary>
            public const string EmptyFile = "BSMT18";
            /// <summary>
            /// Path specified in <see cref="Tasks.ReplaceInFile.File"/> does not exist.
            /// </summary>
            public const string MissingSource = "BSMT19";
            /// <summary>
            /// <see cref="Tasks.ReplaceInFile.Pattern"/> is null or empty.
            /// </summary>
            public const string EmptyPattern = "BSMT20";
            /// <summary>
            /// Other error.
            /// </summary>
            public const string ReplaceFailed = "BSMT21";
        }

    }
}
