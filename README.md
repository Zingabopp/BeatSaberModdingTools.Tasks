# BeatSaberModdingTools.Tasks
A set of MSBuild tasks for Beat Saber mods. Created for the templates in [Beat Saber Modding Tools](https://github.com/Zingabopp/BeatSaberModdingTools).

# Tasks
## GetManifestInfo
Parses the mod or library's manifest and assembly info to verify they match and outputs the plugin/assembly/game version for use in other tasks.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|ErrorOnMismatch|bool|No|If true, the task will report failure if the versions defined in the assembly and manifest don't match or can't be determined.|
|ManifestPath|string|No|Set to use a manifest file other than `manifest.json` in the project root.|
|AssemblyInfoPath|string|No|Set to use an assembly info path other than `Properties\AssemblyInfo.cs`.|
|KnownAssemblyVersion|string|No|A string to use for the assembly version, use this if your assembly version is already defined in a project property.|

Outputs:
|Name|Type|Description|
|---|---|---|
|PluginVersion|string|The mod or library's version as reported by the manifest file.|
|AssemblyVersion|string|The mod or library's version as reported by the AssemblyInfo file or `KnownAssemblyVersion`.|
|GameVersion|string|The Beat Saber game version defined in the manifest file.|

## GetCommitInfo
Gets information about the git repository and current commit.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|ProjectDir|string|Yes|The directory of the project.|
|HashLength|int|No|The length of the `CommitHash` output. Default is 7.|
|NoGit|bool|No|If true, reads git files manually instead of using the `git` executable. This is faster, but the `Modified` output will be unavailable.|
|SkipStatus|bool|No|If true, does not attempt to check if files have been modified.|

Outputs:
|Name|Type|Description|
|---|---|---|
|CommitHash|string|The first 8 characters of the current commit hash. Outputs `local` if project isn't using git source control.|
|Branch|string|The current branch of the repository.|
|Modified|string|Will be `Modified` if there are uncommitted changes, `Unmodified` if there aren't. Empty if it can't be determined.|

# ZipDir
Creates a zip archive from the given directory.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|SourceDirectory|string|Yes|Directory to zip.|
|DestinationFile|string|Yes|Path of the created zip.|

Outputs:
|Name|Type|Description|
|---|---|---|
|ZipPath|string|Full path to the created zip file. Empty if the file could not be created.|

# IsProcessRunning
Checks if the specified process is running.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|ProcessName|string|Yes|Name of the process to check.|

Outputs:
|Name|Type|Description|
|---|---|---|
|IsRunning|bool|True if the process is running, false otherwise.|

# ReplaceInFile
Replaces text in a file that matches a pattern with a substitute.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|File|string|Yes|Path to target file.|
|Pattern|string|Yes|Pattern to match for replacement (case sensitive).|
|Substitute|string|Yes|String to replace matched patterns with.|
|UseRegex|bool|No|If true, `Pattern` will be treated as a Regular Expression.|
|RegexMultilineMode|bool|No|If true, changes '^' and '$ so they match the beginning and end of a line instead of the entire string.|
|RegexSinglelineMode|bool|No|If true, changes the meaning of '.' so it matches every character except '\n' (newline).|
|EscapeBackslash|bool|No|If true, escapes the `\` character in `Substitute` with `\\`.











