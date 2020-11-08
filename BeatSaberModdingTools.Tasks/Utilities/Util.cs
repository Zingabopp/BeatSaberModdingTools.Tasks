using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BeatSaberModdingTools.Tasks.Utilities
{
    /// <summary>
    /// Utility functions shared by various tasks.
    /// </summary>
    public static class Util
    {
        private static readonly Regex SemVerRegex =
            new Regex(
                @"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$",
                RegexOptions.Compiled);

        /// <summary>
        /// Strips any prerelease version label from a SemVer string.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string StripVersionLabel(string version)
        {
            MatchCollection allMatches = SemVerRegex.Matches(version);

            if (allMatches.Count <= 0 || !allMatches[0].Success)
            {
                throw new ParsingException($"{version} is not a valid SemVer version string.");
            }

            GroupCollection groups = allMatches[0].Groups;
            return $"{groups["major"]}.{groups["minor"]}.{groups["patch"]}";
        }

        /// <summary>
        /// Parses a version string into an integer array. Returns an empty array if there are non-integers in the version.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="minLength"></param>
        /// <returns></returns>
        public static int[] ParseVersionString(string version, int minLength = 0)
        {
            string[] verAry = version.Split('.');
            List<int> e = new List<int>(verAry.Length);
            for (int i = 0; i < verAry.Length; i++)
            {
                if (int.TryParse(verAry[i], out int result))
                    e.Add(result);
                else
                    throw new ParsingException($"Could not parse '{verAry[i]}' in '{version}' to an integer.");
            }
            while (e.Count < minLength)
            {
                e.Add(0);
            }
            return e.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyVersion"></param>
        /// <param name="manifestVersion"></param>
        /// <returns></returns>
        /// <exception cref="ParsingException"></exception>
        /// <exception cref="VersionMatchException"></exception>
        public static bool MatchVersions(string assemblyVersion, string manifestVersion)
        {
            int[] asmAry = Util.ParseVersionString(assemblyVersion, 3);
            string manifestStr = Util.StripVersionLabel(manifestVersion);
            int[] manifestAry = Util.ParseVersionString(manifestStr, 3);
            bool matchFound = false;
            if (manifestAry.Length > asmAry.Length)
                throw new VersionMatchException("manifestVersion cannot have more numbers than the assemblyVersion.");
            if (asmAry.Length > 4)
                throw new VersionMatchException("assemblyVersion cannot have more than 4 version numbers.");
            if (asmAry.Length > 3)
            {
                if (asmAry.Take(3).SequenceEqual(manifestAry) && asmAry.Last() == 0)
                    matchFound = true;
                else if (asmAry.Skip(1).SequenceEqual(manifestAry) && asmAry.First() == 0)
                    matchFound = true;
            }
            else
                matchFound = asmAry.SequenceEqual(manifestAry);

            return matchFound;
        }
    }
}
