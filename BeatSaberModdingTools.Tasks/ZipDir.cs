using BeatSaberModdingTools.Tasks.Utilties;
using Microsoft.Build.Framework;
using System;
using System.IO;
using System.IO.Compression;

namespace BeatSaberModdingTools.Tasks
{
    /// <summary>
    /// Zips the contents of a directory.
    /// </summary>
    public class ZipDir : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Name of the directory to zip.
        /// </summary>
        [Required]
        public virtual string SourceDirectory { get; set; }
        /// <summary>
        /// Name of the created zip file.
        /// </summary>
        [Required]
        public virtual string DestinationFile { get; set; }
        /// <summary>
        /// Full path to the created zip file.
        /// </summary>
        [Output]
        public virtual string ZipPath { get; set; }

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
                FileInfo zipFile = new FileInfo(DestinationFile);
                DirectoryInfo zipDir = zipFile.Directory;

                zipDir.Create();
                zipDir.Refresh();
                if (zipFile.Exists)
                    zipFile.Delete();
                if (string.IsNullOrEmpty(SourceDirectory))
                {
                    errorCode = MessageCodes.ZipDir.ZipEmptySource;
                    throw new ArgumentNullException($"{nameof(SourceDirectory)} cannot be null or empty.");
                }
                if (string.IsNullOrEmpty(DestinationFile))
                {
                    errorCode = MessageCodes.ZipDir.ZipEmptyDestination;
                    throw new ArgumentNullException($"{nameof(DestinationFile)} cannot be null or empty.");
                }
                if (!Directory.Exists(SourceDirectory))
                {
                    errorCode = MessageCodes.ZipDir.ZipMissingSource;
                    throw new DirectoryNotFoundException($"{nameof(SourceDirectory)} '{SourceDirectory}' not found.");
                }
                Logger.LogMessage(MessageImportance.High, "Zipping Directory \"{0}\" to \"{1}\"", SourceDirectory, DestinationFile);
                ZipFile.CreateFromDirectory(SourceDirectory, DestinationFile);
                ZipPath = zipFile.FullName;
                return true;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(errorCode))
                    errorCode = MessageCodes.ZipDir.ZipFailed;
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
