using Microsoft.Build.Framework;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace BeatSaberModdingTools.Tasks
{
    public class ZipDir : Microsoft.Build.Utilities.Task
    {
        [Required]
        public virtual string DirectoryName { get; set; }
        [Required]
        public virtual string ZipFileName { get; set; }

        public override bool Execute()
        {
            try
            {
                DirectoryInfo zipDir = new DirectoryInfo(Path.GetDirectoryName(Path.GetFullPath(ZipFileName)));
                if (zipDir.Exists)
                    zipDir.Delete(true);
                zipDir.Create();
                zipDir.Refresh();
                int tries = 0;
                while (!zipDir.Exists || tries < 10) // Prevents breaking when Explorer is in the folder.
                {
                    tries++;
                    Thread.Sleep(50);
                    zipDir.Create();
                    zipDir.Refresh();
                }

                if (File.Exists(ZipFileName))
                    File.Delete(ZipFileName);
                Log.LogMessage(MessageImportance.High, "Zipping Directory \"{0}\" to \"{1}\"", DirectoryName, ZipFileName);
                ZipFile.CreateFromDirectory(DirectoryName, ZipFileName);
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
