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

## GetCommitHash
Gets the first 8 characters of the current commit hash for projects using git source control.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|ProjectDir|string|Yes|The directory of the project.|

Outputs:
|Name|Type|Description|
|---|---|---|
|CommitShortHash|string|The first 8 characters of the current commit hash. Outputs `local` if project isn't using git source control.|

# ZipDir
Creates a zip archive from the given directory.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|SourceDirectory|string|Yes|Directory to zip.|
|DestinationFile|string|Yes|Path of the created zip.|

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










