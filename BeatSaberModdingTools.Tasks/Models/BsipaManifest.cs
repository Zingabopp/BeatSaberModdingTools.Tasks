using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatSaberModdingTools.Tasks.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BeatSaberModdingTools.Tasks.Models
{
    public partial class BsipaManifest
    {
        private static readonly char[] DependencySeparators = new char[] { ';' };
        private static readonly char[] VersionSeparators = new char[] { '|' };
        /// <summary>
        /// JSON schema.
        /// </summary>
        [JsonProperty("$schema")]
        public string Schema { get; set; } = @"https://raw.githubusercontent.com/bsmg/BSIPA-MetadataFileSchema/master/Schema.json";
        /// <summary>
        /// Throws an exception if the <see cref="BsipaManifest"/> is not valid.
        /// </summary>
        /// <param name="requiresBsipa"></param>
        /// <exception cref="ManifestValidationException"/>
        /// <exception cref="BsipaDependsOnException"/>
        public void Validate(bool requiresBsipa)
        {
            if (string.IsNullOrEmpty(Id))
                throw new ManifestValidationException(nameof(Id));
            if (string.IsNullOrEmpty(Name))
                throw new ManifestValidationException(nameof(Name));
            if (string.IsNullOrEmpty(Author))
                throw new ManifestValidationException(nameof(Author));
            if (string.IsNullOrEmpty(Version))
                throw new ManifestValidationException(nameof(Version));
            if (string.IsNullOrEmpty(GameVersion))
                throw new ManifestValidationException(nameof(GameVersion));
            if (string.IsNullOrEmpty(GetDescription()))
                throw new ManifestValidationException(nameof(Description));
            if (requiresBsipa && !DependsOn.TryGetValue("BSIPA", out _))
                throw new BsipaDependsOnException();
        }

        /// <summary>
        /// Gets the <see cref="Description"/> string.
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            if (!string.IsNullOrWhiteSpace(Description.String))
                return Description.String;
            if (Description.StringArray != null && Description.StringArray.Length > 0)
                return string.Join("\n", Description.StringArray);
            return null;
        }

        /// <summary>
        /// Sets the values in the DependsOn Dictionary.
        /// </summary>
        /// <param name="dependsOnString"></param>
        public void SetDependsOn(string[] dependsOnString)
        {
            foreach (var dependsOnEntry in dependsOnString)
            {
                string[] dependencies = dependsOnEntry.Split(DependencySeparators, StringSplitOptions.RemoveEmptyEntries);
                if (dependencies.Length > 0)
                {
                    if (DependsOn == null)
                        DependsOn = new Dictionary<string, string>();
                    foreach (var dep in dependencies)
                    {
                        string[] parts = dep.Split(VersionSeparators, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                            throw new ManifestValidationException(nameof(DependsOn), $"DependsOn entry '{dep}' is not valid (should be 'ModID{VersionSeparators[0]}Version'");
                        DependsOn[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the values in the ConflictsWith Dictionary.
        /// </summary>
        /// <param name="dependsOnString"></param>
        public void SetConflictsWith(string[] dependsOnString)
        {
            foreach (var dependsOnEntry in dependsOnString)
            {
                string[] dependencies = dependsOnEntry.Split(DependencySeparators, StringSplitOptions.RemoveEmptyEntries);
                if (dependencies.Length > 0)
                {
                    if (ConflictsWith == null)
                        ConflictsWith = new Dictionary<string, string>();
                    foreach (var dep in dependencies)
                    {
                        string[] parts = dep.Split(VersionSeparators, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                            throw new ManifestValidationException(nameof(DependsOn), $"DependsOn entry '{dep}' is not valid (should be 'ModID{VersionSeparators[0]}Version'");
                        DependsOn[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
        }
        /// <summary>
        /// Additional data not deserialized into the object.
        /// </summary>
        [JsonExtensionData(ReadData = true, WriteData = true)]
        protected Dictionary<string, JToken> ExtensionData;
    }
}
