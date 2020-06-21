using Microsoft.Build.Framework;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

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
        /// Executes the task.
        /// </summary>
        /// <returns>true if successful</returns>
        public override bool Execute()
        {
            try
            {
                FileInfo zipFile = new FileInfo(DestinationFile);
                DirectoryInfo zipDir = zipFile.Directory;
                
                zipDir.Create();
                zipDir.Refresh();
                if (zipFile.Exists)
                    zipFile.Delete();
                Log.LogMessage(MessageImportance.High, "Zipping Directory \"{0}\" to \"{1}\"", SourceDirectory, DestinationFile);
                ZipFile.CreateFromDirectory(SourceDirectory, DestinationFile);
                ZipPath = zipFile.FullName;
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
