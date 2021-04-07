using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace BeatSaberModdingTools.Tasks.Utilities
{
    public static class ParseUtil
    {

        private static readonly char[] DependencySeparators = new char[] { ';' };
        private static readonly char[] VersionSeparators = new char[] { '|' };

        public static string[] GetStringArray(JSONArray node)
        {
            string[] values = node.Linq.Select(p => p.Value.Value).ToArray();
            return values;
        }

        public static void SetStringArray(JSONArray node, IEnumerable<string> ary)
        {
            foreach (var item in ary)
            {
                node.Add(item);
            }
        }
        public static Dictionary<string, string> GetDictionary(JSONNode node)
        {

            if (node != null && node is JSONObject obj && obj.Count > 0)
                return obj.Linq.ToDictionary(p => p.Key, p => p.Value.Value);
            return null;
        }

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

        public static string[] ParseStringArray(string aryStr)
        {
            if (aryStr == null || aryStr.Length == 0)
                return null;
            string[] strs = aryStr.Split(DependencySeparators).Select(s => s.Trim()).ToArray();
            if (strs.Length > 0)
                return strs;
            return null;
        }
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
        /// Sets the values in the DependsOn Dictionary.
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
                                throw new ManifestValidationException(propName, $"{propName} entry '{dep}' is not valid (should be 'ModID{VersionSeparators[0]}Version'");
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
    }
}
