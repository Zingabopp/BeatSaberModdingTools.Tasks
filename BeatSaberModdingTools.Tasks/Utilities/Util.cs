﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatSaberModdingTools.Tasks.Utilities
{
    /// <summary>
    /// Utility functions shared by various tasks.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Strips any prerelease version label from a SemVer string.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string StripVersionLabel(string version)
        {
            string[] ary = version.Split('.', '-');
            return string.Join(".", ary.Take(3));
        }

        /// <summary>
        /// Parses a version string into an integer array. Returns an empty array if there are non-integers in the version.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static int[] ParseVersionString(string version, int minLength = 0)
        {
            string[] verAry = version.Split('.');
            List<int> e = new List<int>(verAry.Length);
            for(int i = 0; i < verAry.Length; i++)
            {
                if (int.TryParse(verAry[i], out int result))
                    e.Add(result);
                else
                    throw new ParsingException($"Could not parse '{verAry[i]}' in '{version}' to an integer.");
            }
            while(e.Count < minLength)
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
                else if(asmAry.Skip(1).SequenceEqual(manifestAry) && asmAry.First() == 0)
                    matchFound = true;
            }
            else
                matchFound = asmAry.SequenceEqual(manifestAry);

            return matchFound;
        }
    }
}