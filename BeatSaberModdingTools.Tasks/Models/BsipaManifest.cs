using System.Collections.Generic;
using System.Linq;
using BeatSaberModdingTools.Tasks.Utilities;

namespace BeatSaberModdingTools.Tasks.Models
{
    public partial class BsipaManifest
    {
        /// <summary>
        /// URL for the JSON schema.
        /// </summary>
        public const string SchemaUrl = @"https://raw.githubusercontent.com/bsmg/BSIPA-MetadataFileSchema/master/Schema.json";
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
            List<string> invalidProperties = new List<string>();
            if (string.IsNullOrWhiteSpace(Id))
                invalidProperties.Add(nameof(Id));
            if (string.IsNullOrWhiteSpace(Name))
                invalidProperties.Add(nameof(Name));
            if (string.IsNullOrWhiteSpace(Author))
                invalidProperties.Add(nameof(Author));
            if (string.IsNullOrWhiteSpace(Version))
                invalidProperties.Add(nameof(Version));
            if (string.IsNullOrWhiteSpace(GameVersion))
                invalidProperties.Add(nameof(GameVersion));
            if (string.IsNullOrWhiteSpace(GetDescription()))
                invalidProperties.Add(nameof(Description));
            if (requiresBsipa && !(DependsOn?.TryGetValue("BSIPA", out _) ?? false))
                throw new BsipaDependsOnException();
            if (invalidProperties.Count > 0)
            {
                string message;
                if (invalidProperties.Count == 1)
                    message = $"The property '{invalidProperties.First()}' cannot be empty.";
                else
                    message = $"The properties '{string.Join(", ", invalidProperties)}' cannot be empty.";
                throw new ManifestValidationException(message, invalidProperties);
            }
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
