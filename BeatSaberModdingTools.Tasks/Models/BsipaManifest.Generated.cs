/*
using System;
using System.Collections.Generic;

using System.Globalization;
using SimpleJSON;

namespace BeatSaberModdingTools.Tasks.Models
{
    /// <summary>
    /// Models a BSIPA manifest file.
    /// </summary>
    public partial class BsipaManifest
    {
        /// <summary>
        /// modsaber id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// plugin name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// plugin author
        /// </summary>
        [JsonProperty("author")]
        public string Author { get; set; }

        /// <summary>
        /// plugin version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// plugin description
        /// </summary>
        [JsonProperty("description")]
        public Description Description { get; set; }

        /// <summary>
        /// compatable game version
        /// </summary>
        [JsonProperty("gameVersion")]
        public string GameVersion { get; set; }

        /// <summary>
        /// the icon to represent the plugin, as a PNG
        /// </summary>
        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string Icon { get; set; }

        /// <summary>
        /// features to enable for plugin
        /// </summary>
        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Features { get; set; }

        /// <summary>
        /// A list of files that are associated with this mod. If this is a bare manifest, must
        /// include *all* files distributed with the mod. Otherwise, it may exclude the assembly it
        /// is embedded in.
        /// </summary>
        [JsonProperty("files", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Files { get; set; }

        /// <summary>
        /// dependencies
        /// </summary>
        [JsonProperty("dependsOn", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> DependsOn { get; set; }

        /// <summary>
        /// incompatabilities
        /// </summary>
        [JsonProperty("conflictsWith", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> ConflictsWith { get; set; }

        /// <summary>
        /// various links associated with the mod
        /// </summary>
        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public Links Links { get; set; }

        /// <summary>
        /// plugins to load after this one
        /// </summary>
        [JsonProperty("loadBefore", NullValueHandling = NullValueHandling.Ignore)]
        public string[] LoadBefore { get; set; }

        /// <summary>
        /// plugins to load before this one
        /// </summary>
        [JsonProperty("loadAfter", NullValueHandling = NullValueHandling.Ignore)]
        public string[] LoadAfter { get; set; }

        /// <summary>
        /// miscellaneous properties
        /// </summary>
        [JsonProperty("misc", NullValueHandling = NullValueHandling.Ignore)]
        public Misc Misc { get; set; }


    }

    /// <summary>
    /// various links associated with the mod
    /// </summary>
    public partial class Links
    {
        /// <summary>
        /// a link to a donations page
        /// </summary>
        [JsonProperty("donate", NullValueHandling = NullValueHandling.Ignore)]
        public string Donate { get; set; }

        /// <summary>
        /// a link to the project home page. if not specified, same as project-source
        /// </summary>
        [JsonProperty("project-home", NullValueHandling = NullValueHandling.Ignore)]
        public string ProjectHome { get; set; }

        /// <summary>
        /// a link to the project source. if not specified, same as project-home.
        /// </summary>
        [JsonProperty("project-source", NullValueHandling = NullValueHandling.Ignore)]
        public string ProjectSource { get; set; }
    }

    /// <summary>
    /// miscellaneous properties
    /// </summary>
    public partial class Misc
    {
        /// <summary>
        /// a hint for the loader for where to find the plugin type
        /// </summary>
        [JsonProperty("plugin-hint", NullValueHandling = NullValueHandling.Ignore)]
        public string PluginHint { get; set; }
    }

    /// <summary>
    /// plugin description
    /// </summary>
    public partial struct Description
    {
        /// <summary>
        /// Description string
        /// </summary>
        public string String;
        /// <summary>
        /// Description string array...
        /// </summary>
        public string[] StringArray;

        /// <summary>
        /// Create a <see cref="Description"/> from a string.
        /// </summary>
        /// <param name="String"></param>
        public static implicit operator Description(string String) => new Description { String = String };
        /// <summary>
        /// Create a <see cref="Description"/> from a string array.
        /// </summary>
        /// <param name="StringArray"></param>
        public static implicit operator Description(string[] StringArray) => new Description { StringArray = StringArray };
    }

    public partial class BsipaManifest
    {
        /// <summary>
        /// Create a <see cref="BsipaManifest"/> from a JSON string.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static BsipaManifest FromJson(string json)
            => JsonConvert.DeserializeObject<BsipaManifest>(json, Converter.Settings);
    }

    /// <summary>
    /// Extension class for serializing.
    /// </summary>
    public static class Serialize
    {
        /// <summary>
        /// Convert a <see cref="BsipaManifest"/> to a JSON string.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToJson(this BsipaManifest self)
            => JsonConvert.SerializeObject(self, Converter.Settings);
    }
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.Indented,
            Converters =
            {
                DescriptionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class DescriptionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Description) || t == typeof(Description?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new Description { String = stringValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<string[]>(reader);
                    return new Description { StringArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type Description");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Description)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.StringArray != null)
            {
                serializer.Serialize(writer, value.StringArray);
                return;
            }
            throw new Exception("Cannot marshal type Description");
        }

        public static readonly DescriptionConverter Singleton = new DescriptionConverter();
    }
   
}
*/
