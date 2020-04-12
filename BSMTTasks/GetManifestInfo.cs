using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace BSMTTasks
{
    public class GetManifestInfo : Microsoft.Build.Utilities.Task
    {
        [Output]
        public virtual string GameVersion { get; protected set; }
        [Output]
        public virtual string PluginVersion { get; protected set; }
        [Output]
        public virtual string AssemblyVersion { get; protected set; }
        public virtual bool ErrorOnMismatch { get; set; }

        public override bool Execute()
        {
            try
            {
                string manifestFile = "manifest.json";
                string assemblyFile = "Properties\\AssemblyInfo.cs";
                string manifest_gameVerStart = "\"gameVersion\"";
                string manifest_versionStart = "\"version\"";
                string manifest_gameVerLine = null;
                string manifest_versionLine = null;
                string startString = "[assembly: AssemblyVersion(\"";
                string secondStartString = "[assembly: AssemblyFileVersion(\"";
                string assemblyFileVersion = null;
                string firstLineStr = null;
                string endLineStr = null;
                bool badParse = false;
                int startLine = 1;
                int endLine = 0;
                int startColumn = 0;
                int endColumn = 0;
                if (!File.Exists(manifestFile))
                {
                    throw new FileNotFoundException("Could not find manifest: " + Path.GetFullPath(manifestFile));
                }
                if (!File.Exists(assemblyFile))
                {
                    throw new FileNotFoundException("Could not find AssemblyInfo: " + Path.GetFullPath(assemblyFile));
                }
                string line;
                using (StreamReader manifestStream = new StreamReader(manifestFile))
                {
                    while ((line = manifestStream.ReadLine()) != null && (manifest_versionLine == null || manifest_gameVerLine == null))
                    {
                        line = line.Trim();
                        if (line.StartsWith(manifest_gameVerStart))
                        {
                            manifest_gameVerLine = line;
                        }
                        else if (line.StartsWith(manifest_versionStart))
                        {
                            manifest_versionLine = line;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(manifest_versionLine))
                {
                    PluginVersion = manifest_versionLine.Substring(manifest_versionStart.Length).Replace(":", "").Replace("\"", "").TrimEnd(',').Trim();
                }
                else
                {
                    Log.LogError("Build", "BSMOD04", "", manifestFile, 0, 0, 0, 0, "PluginVersion not found in manifest.json");
                    PluginVersion = "E.R.R";
                    if (ErrorOnMismatch)
                        return false;
                }

                if (!string.IsNullOrEmpty(manifest_gameVerLine))
                {
                    GameVersion = manifest_gameVerLine.Substring(manifest_gameVerStart.Length).Replace(":", "").Replace("\"", "").TrimEnd(',').Trim();
                }
                else
                {
                    Log.LogError("Build", "BSMOD05", "", manifestFile, 0, 0, 0, 0, "GameVersion not found in manifest.json");
                    GameVersion = "E.R.R";
                    if (ErrorOnMismatch)
                        return false;
                }

                line = null;
                using (StreamReader assemblyStream = new StreamReader(assemblyFile))
                {
                    while ((line = assemblyStream.ReadLine()) != null)
                    {
                        if (line.Trim().StartsWith(startString))
                        {
                            firstLineStr = line;
                            break;
                        }
                        startLine++;
                        endLine = startLine + 1;
                    }
                    while ((line = assemblyStream.ReadLine()) != null)
                    {
                        if (line.Trim().StartsWith(secondStartString))
                        {
                            endLineStr = line;
                            break;
                        }
                        endLine++;
                    }
                }
                if (!string.IsNullOrEmpty(firstLineStr))
                {
                    startColumn = firstLineStr.IndexOf('"') + 1;
                    endColumn = firstLineStr.LastIndexOf('"');
                    if (startColumn > 0 && endColumn > 0)
                        AssemblyVersion = firstLineStr.Substring(startColumn, endColumn - startColumn);
                    else
                        badParse = true;
                }
                else
                    badParse = true;
                if (badParse)
                {
                    Log.LogError("Build", "BSMOD03", "", assemblyFile, 0, 0, 0, 0, "Unable to parse the AssemblyVersion from {0}", assemblyFile);
                    if (ErrorOnMismatch)
                        return false;
                    badParse = false;
                }

                if (PluginVersion != "E.R.R" && AssemblyVersion != PluginVersion)
                {
                    Log.LogError("Build", "BSMOD01", "", assemblyFile, startLine, startColumn + 1, startLine, endColumn + 1, "PluginVersion {0} in manifest.json does not match AssemblyVersion {1} in AssemblyInfo.cs", PluginVersion, AssemblyVersion, assemblyFile);
                    Log.LogMessage(MessageImportance.High, "PluginVersion {0} does not match AssemblyVersion {1}", PluginVersion, AssemblyVersion);
                    if (ErrorOnMismatch)
                        return false;
                }
                if (!string.IsNullOrEmpty(endLineStr))
                {
                    startColumn = endLineStr.IndexOf('"') + 1;
                    endColumn = endLineStr.LastIndexOf('"');
                    if (startColumn > 0 && endColumn > 0)
                    {
                        assemblyFileVersion = endLineStr.Substring(startColumn, endColumn - startColumn);
                        if (AssemblyVersion != assemblyFileVersion)
                        {
                            Log.LogWarning("Build", "BSMOD02", "", assemblyFile, endLine, startColumn + 1, endLine, endColumn + 1, "AssemblyVersion {0} does not match AssemblyFileVersion {1} in AssemblyInfo.cs", AssemblyVersion, assemblyFileVersion);
                            if (ErrorOnMismatch)
                                return false;
                        }

                    }
                    else
                    {
                        Log.LogWarning("Build", "BSMOD06", "", assemblyFile, 0, 0, 0, 0, "Unable to parse the AssemblyFileVersion from {0}", assemblyFile);
                        if (ErrorOnMismatch)
                            return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}
