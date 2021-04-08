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
        /// <summary>
        /// Create a <see cref="BsipaManifest"/> from a JSON string.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static BsipaManifest FromJson(string json)
        {
            if (JSON.Parse(json) is JSONObject obj)
                return new BsipaManifest(obj);
            return new BsipaManifest();
        }
        private readonly JSONObject json;

        /// <summary>
        /// Convert the <see cref="BsipaManifest"/> to a JSON string.
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return json.ToString(3);
        }

        /// <summary>
        /// Create a new BsipaManifest with an optional base <see cref="JSONObject"/>.
        /// </summary>
        /// <param name="j"></param>
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
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    json["$schema"] = value;
            }
        }

        /// <summary>
        /// modsaber id
        /// </summary>
        [JsonProperty("id")]
        public string Id
        {
            get => GetStringValue("id");
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) json["id"] = value;
            }
        }

        /// <summary>
        /// plugin name
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get => GetStringValue("name");
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) json["name"] = value;
            }
        }

        /// <summary>
        /// plugin author
        /// </summary>
        [JsonProperty("author")]
        public string Author
        {
            get => GetStringValue("author");
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) json["author"] = value;
            }
        }

        /// <summary>
        /// plugin version
        /// </summary>
        [JsonProperty("version")]
        public string Version
        {
            get => GetStringValue("version");
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) json["version"] = value;
            }
        }

        /// <summary>
        /// plugin description
        /// </summary>
        [JsonProperty("description")]
        public string Description
        {
            get => GetStringValue("description");
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    json["description"] = value;
            }
        }

        /// <summary>
        /// compatable game version
        /// </summary>
        [JsonProperty("gameVersion")]
        public string GameVersion
        {
            get => GetStringValue("gameVersion");
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    json["gameVersion"] = value;
            }
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
                if (value == null || value.Length == 0)
                    return;
                JSONArray ary = new JSONArray();
                SetStringArray(ary, value);
                var node = json["files"];
                if (node is JSONArray existing)
                {
                    foreach (var item in ary)
                    {
                        existing.Add(item);
                    }
                }
                else
                    json["files"] = ary;
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
                if (value == null || value.Length == 0)
                    return;
                JSONArray ary = new JSONArray();
                SetStringArray(ary, value);
                var node = json["loadBefore"];
                if (node is JSONArray existing)
                {
                    foreach (var item in ary)
                    {
                        existing.Add(item);
                    }
                }
                else
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
                if (value == null || value.Length == 0)
                    return;
                JSONArray ary = new JSONArray();
                SetStringArray(ary, value);
                var node = json["loadAfter"];
                if(node is JSONArray existing)
                {
                    foreach (var item in ary)
                    {
                        existing.Add(item);
                    }
                }
                else
                    json["loadAfter"] = ary;
            }
        }

        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public JSONObject Features
        {
            get
            {
                var val = json["features"];

                if (val != null && val is JSONObject obj && obj.Count > 0)
                    return obj;
                return null;
            }
            set
            {
                if (value != null && value is JSONObject obj && obj.Count > 0)
                {
                    var existing = json["features"];
                    if (existing != null && existing.Count > 0 && existing is JSONObject eObj)
                    {
                        foreach (var key in obj.Keys)
                        {
                            eObj[key] = obj[key];
                        }
                    }
                    else
                        json["features"] = value;
                }
            }
        }
    }
}
