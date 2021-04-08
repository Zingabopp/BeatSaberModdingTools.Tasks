using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BeatSaberModdingTools.Tasks.Models;
using BeatSaberModdingTools.Tasks.Utilities;
using BeatSaberModdingTools.Tasks.Utilities.Mock;
using Microsoft.Build.Framework;

namespace BeatSaberModdingTools.Tasks
{
    /// <summary>
    /// Generates a BSIPA manifest file.
    /// </summary>
    public class GenerateManifest : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// <see cref="ITaskLogger"/> instance used.
        /// </summary>
        public ITaskLogger Logger;

        #region Inputs
        #region Required If Not in BaseManifest
        /// <summary>
        /// The mod's ID.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The mod's name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The mod's author.
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// The mod's version.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// The Beat Saber game version the mod was built for.
        /// </summary>
        public string GameVersion { get; set; }
        /// <summary>
        /// The mod's description.
        /// </summary>
        public string Description { get; set; }
        #endregion

        /// <summary>
        /// Path to the mod's icon.
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// String array of mods to include in DependsOn
        /// </summary>
        public ITaskItem[] DependsOn { get; set; }
        /// <summary>
        /// String array of mods to include in ConflictsWith
        /// </summary>
        public ITaskItem[] ConflictsWith { get; set; }
        /// <summary>
        /// List of files required for the mod to run.
        /// </summary>
        public string[] Files { get; set; }
        /// <summary>
        /// List of mods this mod needs to load before.
        /// </summary>
        public string[] LoadBefore { get; set; }
        /// <summary>
        /// List of mods that need to load before this one.
        /// </summary>
        public string[] LoadAfter { get; set; }

        /// <summary>
        /// Link to the mod's source repository.
        /// </summary>
        public string ProjectSource { get; set; }
        /// <summary>
        /// Link to the project's home page.
        /// </summary>
        public string ProjectHome { get; set; }
        /// <summary>
        /// Link to the author's donation page.
        /// </summary>
        public string Donate { get; set; }

        /*
        public string Features { get; set; }
        public string Links { get; set; }
        public string Misc { get; set; }
        */
        /// <summary>
        /// Path to an existing manifest file.
        /// </summary>
        public string BaseManifestPath { get; set; }
        /// <summary>
        /// Target path to write the manifest file (including file name).
        /// </summary>
        public string TargetPath { get; set; }
        /// <summary>
        /// If true, manifest validation will fail if BSIPA isn't listed as a DependsOn.
        /// </summary>
        public bool RequiresBsipa { get; set; } = true;
        #endregion
        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            if (this.BuildEngine != null)
                Logger = new LogWrapper(Log);
            else
                Logger = new MockTaskLogger();
            try
            {
                var manifest = MakeManifest();
                manifest.Validate(RequiresBsipa);
                WriteManifest(manifest, TargetPath);
                return true;
            }
            catch (ManifestValidationException ex)
            {
                // Generated manifest not valid.
                Logger.LogErrorFromException(ex);
            }
            catch (ArgumentException ex)
            {
                // Base manifest specified but doesn't exist.
                Logger.LogErrorFromException(ex);
            }
            catch (IOException ex)
            {
                // Base JSON read failed or write failed.
                Logger.LogErrorFromException(ex);
            }
            catch (Exception ex)
            {
                Logger.LogErrorFromException(ex);
            }
            return false;
        }

        private void WriteManifest(BsipaManifest manifest, string path)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                path = fileInfo.FullName;
                if (!fileInfo.Directory.Exists)
                    Logger.LogMessage(MessageImportance.High, $"Creating manifest target directory '${fileInfo.Directory.FullName}'...");
                fileInfo.Directory.Create();
                File.WriteAllText(path, manifest.ToJson());
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to write manifest to '${path}': {ex.Message}");
            }
        }

        private BsipaManifest MakeManifest()
        {
            BsipaManifest manifest;
            if (!string.IsNullOrWhiteSpace(BaseManifestPath))
            {
                string manifestPath = Path.GetFullPath(BaseManifestPath);
                if (File.Exists(manifestPath))
                {
                    try
                    {
                        manifest = BsipaManifest.FromJson(File.ReadAllText(manifestPath));
                    }
                    catch (Exception ex)
                    {
                        throw new IOException($"Failed to read JSON at '${manifestPath}': {ex.Message}");
                    }
                }
                else
                    throw new ArgumentException($"A BaseManifestFile '${manifestPath}' does not exist."
                                                , nameof(BaseManifestPath));
            }
            else
                manifest = new BsipaManifest();
            if (string.IsNullOrWhiteSpace(TargetPath))
                TargetPath = "manifest.json";
            SetRequiredProperties(manifest);
            SetOptionalProperties(manifest);
            return manifest;
        }

        private void SetRequiredProperties(BsipaManifest manifest)
        {
            if (!string.IsNullOrWhiteSpace(Id))
            {
                manifest.Id = Id;
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                manifest.Name = Name;
            }
            if (!string.IsNullOrWhiteSpace(Author))
            {
                manifest.Author = Author;
            }
            if (!string.IsNullOrWhiteSpace(Version))
            {
                manifest.Version = Version;
            }
            if (!string.IsNullOrWhiteSpace(GameVersion))
            {
                manifest.GameVersion = GameVersion;
            }
            if (!string.IsNullOrWhiteSpace(Description))
            {
                manifest.Description = Description;
            }
        }

        private void SetOptionalProperties(BsipaManifest manifest)
        {
            manifest.Files = ParseUtil.ParseStringArray(Files);
            manifest.DependsOn = ParseUtil.ParseTaskItems(DependsOn, manifest.DependsOn, "DependsOn");
            manifest.ConflictsWith = ParseUtil.ParseTaskItems(ConflictsWith, manifest.ConflictsWith, "ConflictsWith");
            manifest.LoadBefore = ParseUtil.ParseStringArray(LoadBefore);
            manifest.LoadAfter = ParseUtil.ParseStringArray(LoadAfter);
            if (!string.IsNullOrWhiteSpace(Icon))
                manifest.Icon = Icon;
            manifest.ProjectHome = ProjectHome;
            manifest.ProjectSource = ProjectSource;
            manifest.Donate = Donate;
        }



    }
}
