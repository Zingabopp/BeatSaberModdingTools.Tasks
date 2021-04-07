using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatSaberModdingTools.Tasks.Utilities;
using SimpleJSON;

namespace BeatSaberModdingTools.Tasks.Models
{
    public partial class BsipaManifest
    {
        /// <summary>
        /// Throws an exception if the <see cref="BsipaManifest"/> is not valid.
        /// </summary>
        /// <param name="requiresBsipa"></param>
        /// <exception cref="ManifestValidationException"/>
        /// <exception cref="BsipaDependsOnException"/>
        public void Validate(bool requiresBsipa)
        {
            var nullProps = json.Linq.Where(n => 
            n.Value == null || (n.Value.ToString() == string.Empty)).Select(n => n.Key).ToArray();
            foreach (var prop in nullProps)
            {
                json.Remove(prop);
            }
            if (string.IsNullOrWhiteSpace(Id))
                throw new ManifestValidationException(nameof(Id));
            if (string.IsNullOrWhiteSpace(Name))
                throw new ManifestValidationException(nameof(Name));
            if (string.IsNullOrWhiteSpace(Author))
                throw new ManifestValidationException(nameof(Author));
            if (string.IsNullOrWhiteSpace(Version))
                throw new ManifestValidationException(nameof(Version));
            if (string.IsNullOrWhiteSpace(GameVersion))
                throw new ManifestValidationException(nameof(GameVersion));
            if (string.IsNullOrWhiteSpace(GetDescription()))
                throw new ManifestValidationException(nameof(Description));
            if (requiresBsipa && !(DependsOn?.TryGetValue("BSIPA", out _) ?? false))
                throw new BsipaDependsOnException();
        }

        /// <summary>
        /// Gets the <see cref="Description"/> string.
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            if (!string.IsNullOrWhiteSpace(Description))
                return Description;
            //if (Description.StringArray != null && Description.StringArray.Length > 0)
            //    return string.Join("\n", Description.StringArray);
            return null;
        }
    }
}
