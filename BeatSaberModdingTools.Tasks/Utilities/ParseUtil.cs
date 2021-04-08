using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using Microsoft.Build.Framework;

namespace BeatSaberModdingTools.Tasks.Utilities
{
    /// <summary>
    /// Utilities for parsing things.
    /// </summary>
    public static class ParseUtil
    {

        private static readonly char[] DependencySeparators = new char[] { ';' };
        private static readonly char[] VersionSeparators = new char[] { '|' };

        /// <summary>
        /// Gets a string array from a <see cref="JSONArray"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string[] GetStringArray(JSONArray node)
        {
            string[] values = node.Linq.Select(p => p.Value.Value).ToArray();
            return values;
        }

        /// <summary>
        /// Adds the values of <paramref name="ary"/> into <paramref name="node"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ary"></param>
        public static void SetStringArray(JSONArray node, IEnumerable<string> ary)
        {
            if (ary == null)
                return;
            foreach (var item in ary)
            {
                node.Add(item);
            }
        }

        /// <summary>
        /// Gets a dictionary of properties and values from the given <see cref="JSONNode"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDictionary(JSONNode node)
        {

            if (node != null && node is JSONObject obj && obj.Count > 0)
                return obj.Linq.ToDictionary(p => p.Key, p => p.Value.Value);
            return null;
        }

        /// <summary>
        /// Creates a new <see cref="JSONObject"/> from a dictionary.
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static JSONObject CreateObjFromDict(Dictionary<string, string> dict)
        {
            if (dict == null)
                return null;
            JSONObject obj = new JSONObject();
            foreach (var item in dict)
            {
                obj.Add(item.Key, item.Value);
            }
            return obj;
        }

        /// <summary>
        /// Parses a ';' delimited string into an array. Returns null if the length is 0.
        /// </summary>
        /// <param name="aryStr"></param>
        /// <returns></returns>
        public static string[] ParseStringArray(string aryStr)
        {
            if (aryStr == null || aryStr.Length == 0)
                return null;
            string[] strs = aryStr.Split(DependencySeparators).Select(s => s.Trim()).ToArray();
            if (strs.Length > 0)
                return strs;
            return null;
        }
        /// <summary>
        /// Parses an array of ';' delimited strings into an array. Returns null if the length is 0.
        /// </summary>
        /// <param name="aryStrs"></param>
        /// <returns></returns>
        public static string[] ParseStringArray(string[] aryStrs)
        {
            if (aryStrs == null || aryStrs.Length == 0)
                return null;
            List<string> strList = new List<string>();
            foreach (var aryStr in aryStrs)
            {
                string[] strs = ParseStringArray(aryStr);
                if (strs != null)
                    strList.AddRange(strs);
            }
            if (strList.Count > 0)
                return strList.ToArray();
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictStrings"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseDictString(string[] dictStrings, string propName)
            => ParseDictString(dictStrings, null, propName);
        /// <summary>
        /// Parses a string into a dictionary. Entries are delimited by ';', then KEY|VALUE.
        /// </summary>
        /// <param name="dictStrings"></param>
        /// <param name="existing"></param>
        /// <param name="propName"></param>
        public static Dictionary<string, string> ParseDictString(string[] dictStrings, Dictionary<string, string> existing, string propName)
        {
            var dict = existing ?? new Dictionary<string, string>();
            if (dictStrings != null && dictStrings.Length > 0)
            {
                foreach (var dependsOnEntry in dictStrings)
                {
                    string[] dependencies = dependsOnEntry.Split(DependencySeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (dependencies.Length > 0)
                    {
                        foreach (var dep in dependencies)
                        {
                            string[] parts = dep.Split(VersionSeparators, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                                throw new ManifestValidationException(propName, $"{propName} entry '{dep}' is not valid (should be 'ModID{VersionSeparators[0]}Version')");
                            dict[parts[0].Trim()] = parts[1].Trim();
                        }
                    }
                }
            }
            if (dict.Count > 0)
                return dict;
            else
                return null;
        }

        /// <summary>
        /// Parses an <see cref="ITaskItem"/> into (Include, Version).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool TryParseTaskItem(ITaskItem item, out KeyValuePair<string, string> data)
        {
            data = default;
            string name = item.ToString();
            string version = item.GetMetadata("Version");
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(version))
                return false;
            data = new KeyValuePair<string, string>(name, version);
            return true;
        }
        /// <summary>
        /// Parses a collection of <see cref="ITaskItem"/> into a dictionary of (Include, Version).
        /// </summary>
        /// <param name="items"></param>
        /// <param name="existing"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseTaskItems(
            IEnumerable<ITaskItem> items, Dictionary<string, string> existing, string propName = null)
        {
            if (items == null || items.Count() == 0)
                return null;
            var dict = existing ?? new Dictionary<string, string>(items.Count());
            foreach (var item in items)
            {
                if (item == null)
                    continue;
                if (TryParseTaskItem(item, out KeyValuePair<string, string> data))
                    dict[data.Key] = data.Value;
                else
                    throw new ManifestValidationException(propName, $"{propName} entry '{item}' is not valid (example: '<PropertyName Include=ModID Version=^1.2.3' />)");
            }
            return dict;
        }
    }
}
