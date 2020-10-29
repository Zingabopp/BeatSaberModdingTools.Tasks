# BeatSaberModdingTools.Tasks
A set of MSBuild tasks for Beat Saber mods. Created for the templates in [Beat Saber Modding Tools](https://github.com/Zingabopp/BeatSaberModdingTools).

# Tasks 
**Current as of v1.3.0**
## CompareVersions
Compares an assembly and manifest version string. Logs an error and optionally fails if they don't match.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|PluginVersion|string|Yes|The mod or library's version as reported by the manifest file (must be [SemVer](https://semver.org/)).|
|AssemblyVersion|string|Yes|The mod or library's version as reported by the assembly.|
|ErrorOnMismatch|bool|No|If true, the task will report failure if the versions defined in the assembly and manifest don't match or can't be determined.|

## GetAssemblyInfo
Parses the AssemblyVersion from an AssemblyInfo.cs file.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|AssemblyInfoPath|string|No|Set to use an assembly info path other than `Properties\AssemblyInfo.cs`.|
|FailOnError|bool|No|If true, the task will report failure if the versions defined in the assembly and manifest don't match or can't be determined.|

Outputs:
|Name|Type|Description|
|---|---|---|
|AssemblyVersion|string|The assembly's version as reported by the AssemblyInfo file.|

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
|OriginUrl|string|The URL of the git repository. Empty if it can't be determined.|
|GitUser|string|The GitHub username the repository belongs to (Extracted from OriginUrl).  Empty if it can't be determined.|

## GetManifestInfo
Parses the mod or library's manifest and outputs the game and plugin versions.

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
|GameVersion|string|The Beat Saber game version defined in the manifest file.|
|PluginVersion|string|The mod or library's version as reported by the manifest file.|
|BasePluginVersion|string|The PluginVersion without any prerelease labels (i.e. "1.0.0-beta" -> "1.0.0").|

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
