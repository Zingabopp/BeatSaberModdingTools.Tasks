using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using SimpleJSON;
using static BeatSaberModdingTools.Tasks.Utilities.ParseUtil;

namespace BeatSaberModdingTools.Tasks.Models
{
    /// <summary>
    /// Models a BSIPA manifest file.
    /// </summary>
    public partial class BsipaManifest
    {
        public static BsipaManifest FromJson(string json)
        {
            if (JSON.Parse(json) is JSONObject obj)
                return new BsipaManifest(obj);
            return new BsipaManifest();
        }
        private readonly JSONObject json;

        public string ToJson()
        {
            return json.ToString(3);
        }

        public BsipaManifest(JSONObject j = null)
        {
            json = j ?? new JSONObject();
            Schema = @"https://raw.githubusercontent.com/bsmg/BSIPA-MetadataFileSchema/master/Schema.json";
        }

        private string GetStringValue(string key, JSONNode startNode = null)
        {
            if (startNode == null)
                startNode = json;
            string val = startNode[key]?.Value;
            if (string.IsNullOrEmpty(val))
                return null;
            return val;
        }

        /// <summary>
        /// JSON schema.
        /// </summary>
        [JsonProperty("$schema")]
        public string Schema
        {
            get => GetStringValue("$schema");
            set => json["$schema"] = value;
        }

        /// <summary>
        /// modsaber id
        /// </summary>
        [JsonProperty("id")]
        public string Id
        {
            get => GetStringValue("id");
            set => json["id"] = value;
        }

        /// <summary>
        /// plugin name
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get => GetStringValue("name");
            set => json["name"] = value;
        }

        /// <summary>
        /// plugin author
        /// </summary>
        [JsonProperty("author")]
        public string Author
        {
            get => GetStringValue("author");
            set => json["author"] = value;
        }

        /// <summary>
        /// plugin version
        /// </summary>
        [JsonProperty("version")]
        public string Version
        {
            get => GetStringValue("version");
            set => json["version"] = value;
        }

        /// <summary>
        /// plugin description
        /// </summary>
        [JsonProperty("description")]
        public string Description
        {
            get => GetStringValue("description");
            set => json["description"] = value;
        }

        /// <summary>
        /// compatable game version
        /// </summary>
        [JsonProperty("gameVersion")]
        public string GameVersion
        {
            get => GetStringValue("gameVersion");
            set => json["gameVersion"] = value;
        }

        /// <summary>
        /// the icon to represent the plugin, as a PNG
        /// </summary>
        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string Icon
        {
            get => GetStringValue("icon");
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) 
                    json["icon"] = value;
                else
                    json.Remove("icon");
            }
        }

        /// <summary>
        /// the icon to represent the plugin, as a PNG
        /// </summary>
        [JsonProperty("project-home", NullValueHandling = NullValueHandling.Ignore)]
        public string ProjectHome
        {
            get => GetStringValue("project-home", json["links"]);
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) 
                    json["links"]["project-home"] = value;
                else
                    json["links"].Remove("project-home");
            }
        }
        /// <summary>
        /// the icon to represent the plugin, as a PNG
        /// </summary>
        [JsonProperty("project-source", NullValueHandling = NullValueHandling.Ignore)]
        public string ProjectSource
        {
            get => GetStringValue("project-source", json["links"]);
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) 
                    json["links"]["project-source"] = value;
                else
                    json["links"].Remove("project-source");
            }
        }
        /// <summary>
        /// the icon to represent the plugin, as a PNG
        /// </summary>
        [JsonProperty("donate", NullValueHandling = NullValueHandling.Ignore)]
        public string Donate
        {
            get => GetStringValue("donate", json["links"]);
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    json["links"]["donate"] = value;
                else
                    json["links"].Remove("donate");
            }
        }

        /*
        /// <summary>
        /// features to enable for plugin
        /// </summary>
        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Features { get; set; }
        */

        /// <summary>
        /// A list of files that are associated with this mod. If this is a bare manifest, must
        /// include *all* files distributed with the mod. Otherwise, it may exclude the assembly it
        /// is embedded in.
        /// </summary>
        [JsonProperty("files", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Files
        {
            get
            {
                var val = json["files"];
                if (val != null && val is JSONArray ary && ary.Count > 0)
                    return GetStringArray(ary);
                return null;
            }
            set
            {
                if (value != null)
                {
                    JSONArray ary = new JSONArray();
                    SetStringArray(ary, value);
                    json["files"] = ary;
                }
                else
                    json.Remove("files");
            }
        }

        /// <summary>
        /// dependencies
        /// </summary>
        [JsonProperty("dependsOn", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> DependsOn
        {
            get
            {
                var node = json["dependsOn"];
                return GetDictionary(node);
            }
            set
            {
                if (value != null && value.Count > 0)
                    json["dependsOn"] = CreateObjFromDict(value);
                else
                    json.Remove("dependsOn");
            }
        }

        /// <summary>
        /// incompatabilities
        /// </summary>
        [JsonProperty("conflictsWith", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> ConflictsWith
        {
            get
            {
                var node = json["conflictsWith"];
                return GetDictionary(node);
            }
            set
            {
                if (value != null && value.Count > 0)
                    json["conflictsWith"] = CreateObjFromDict(value);
                else
                    json.Remove("conflictsWith");
            }
        }
        /// <summary>
        /// plugins to load after this one
        /// </summary>
        [JsonProperty("loadBefore", NullValueHandling = NullValueHandling.Ignore)]
        public string[] LoadBefore
        {
            get
            {
                var val = json["loadBefore"];
                if (val != null && val is JSONArray ary && ary.Count > 0)
                    return GetStringArray(ary);
                return null;
            }
            set
            {
                JSONArray ary = new JSONArray();
                SetStringArray(ary, value);
                json["loadBefore"] = ary;
            }
        }

        /// <summary>
        /// plugins to load before this one
        /// </summary>
        [JsonProperty("loadAfter", NullValueHandling = NullValueHandling.Ignore)]
        public string[] LoadAfter
        {
            get
            {
                var val = json["loadAfter"];
                if (val != null && val is JSONArray ary && ary.Count > 0)
                    return GetStringArray(ary);
                return null;
            }
            set
            {
                JSONArray ary = new JSONArray();
                SetStringArray(ary, value);
                json["loadAfter"] = ary;
            }
        }
    }
}
