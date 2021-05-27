# BeatSaberModdingTools.Tasks
A set of MSBuild tasks for Beat Saber mods. Created for the templates in [Beat Saber Modding Tools](https://github.com/Zingabopp/BeatSaberModdingTools).

# Tasks 
**Current as of v1.3.2**
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
|FailOnError|bool|No|If true, the task will report failure if it cannot find or parse the AssemblyInfo file.|

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
|IsPullRequest|bool|True if the current branch appears to be a pull request.|
|Modified|string|Will be `Modified` if there are uncommitted changes, `Unmodified` if there aren't. Empty if it can't be determined.|
|OriginUrl|string|The URL of the git repository. Empty if it can't be determined.|
|GitUser|string|The GitHub username the repository belongs to (Extracted from OriginUrl).  Empty if it can't be determined.|

## GetManifestInfo
Parses the mod or library's manifest and outputs the game and plugin versions.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|ManifestPath|string|No|Set to use a manifest file other than `manifest.json` in the project root.|
|FailOnError|bool|No|If true, the task will report failure if it cannot find or parse the manifest file.|

Outputs:
|Name|Type|Description|
|---|---|---|
|GameVersion|string|The Beat Saber game version defined in the manifest file.|
|PluginVersion|string|The mod or library's version as reported by the manifest file.|
|BasePluginVersion|string|The PluginVersion without any prerelease labels (i.e. "1.0.0-beta" -> "1.0.0").|

## IsProcessRunning
Checks if the specified process is running.

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|ProcessName|string|Yes|Name of the process to check. Case sensitive, do not include extension.|
|Fallback|bool|No|Return this value if IsProcessRunning fails. Defaults to true.|

Outputs:
|Name|Type|Description|
|---|---|---|
|IsRunning|bool|True if the process is running, false otherwise.|

## ReplaceInFile
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
## ZipDir
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
## GenerateManifest
<small>Added in 1.4.1</small><br>
Generates a BSIPA manifest file.<br>

Inputs:
|Name|Type|Required?|Description|
|---|---|---|---|
|Id|string|Yes*|ID for the mod. Use this as the `Mod Name` on BeatMods (because BeatMods).|
|Name|string|Yes*|A friendly name for the mod, usually the same or similar to the mod ID.|
|Author|string|Yes*|The mod author.|
|Version|string|Yes*|Mod version, should be in [SemVer](https://semver.org/) spec.|
|GameVersion|string|Yes*|Beat Saber version the mod was built for.|
|Description|string|Yes*|Description of what the mod does.|
|Icon|string|No|Resource path to the icon.|
|DependsOn|[Mod Identifier](#mod-identifier)|Yes**|Mods that need to be loaded for this mod to function.|
|ConflictsWith|[Mod Identifier](#mod-identifier)|No|Mods cannot be loaded for this mod to function.|
|Files|[string[]](#string-arrays)|No|External files required by the mod or library (usually only used for libraries).|
|LoadBefore|[string[]](#string-arrays)]|No|List of mod IDs for mods that this mod should load before.|
|LoadAfter|[string[]](#string-arrays)|No|List of mod IDs that need to be loaded before this mod (this is implicit for mods in the `DependsOn` list.|
|ProjectSource|string|No|Link to the mod's source repository.|
|ProjectHome|string|No|Link to the mod's project web site.|
|Donate|string|No|Donation link for the mod.|
|Features|[JSON Object String](#json-object-string)|No|A JSON object string to utilize BSIPA's `Features` architecture.|
|Misc|[JSON Object String](#json-object-string)|No|A JSON object string for miscellaneous properties.<sup>1.4.2</sup>|
|PluginHint|string|No|A hint for the loader for where to find the plugin type.<sup>1.4.2</sup>|
|BaseManifestPath|string|No|Path to a manifest file you want to use as a base. GenerateManifest will merge into this manifest.|
|TargetPath|string|No|Output path (including filename) for the generated manifest.|
|RequiresBsipa|bool|No|If true (default), GenerateManifest will error if you don't have BSIPA listed in `DependsOn`.|

*Properties are not required by the task if you are using a base manifest file (specified in `BaseManifestPath`) that already has those properties.<br>
**If a mod references BSIPA and uses its `Plugin` architecture, you *must* have a `DependsOn` for BSIPA.
## Special Types
### Mod Identifier
* Mod identifiers need to have an `Include` attribute (Mod ID from their manifest) and `Version` attribute (SemVer)
* They are defined in one or more `<ItemGroup>`s. 
* Pass them to GenerateManifest using `TaskParameter="@(CollectionName)"`
Example:
```xml
<Project>
    <ItemGroup>
        <Dependency Include="BSIPA" Version="^4.4.0"/>
        <Dependency Include="SongCore" Version="^1.8.0"/>
    </ItemGroup>
    <Target Name="RunGenerateManifest" BeforeTargets="Build">
        <GenerateManifest 
            <!-- Other Parameters -->
            DependsOn="@(Dependency)"
            <!-- Other Parameters -->
        />
    </Target>
</Project>
```
### String Arrays
* String arrays are similar to `Mod Identifiers` except you only need the `Include` attribute.
* If you only need one item, you can pass a single string defined in a `PropertyGroup` as well.
Example:
```xml
<Project>
    <ItemGroup>
        <RequiredFile Include="Libs/RequiredLibFile.dll"/>
        <RequiredFile Include="Libs/OtherRequiredLibFile.dll"/>
    </ItemGroup>
    <Target Name="RunGenerateManifest" BeforeTargets="Build">
        <GenerateManifest 
            <!-- Other Parameters -->
            Files="@(RequiredFile)"
            <!-- Other Parameters -->
        />
    </Target>
</Project>
```
### JSON Object String
* A JSON string that defines an object.
* The string can be put into a `PropertyGroup` property. It seems you don't have to worry about escaping any characters.
Example:
```xml
<Project>
    <PropertyGroup>
        <GameVersion>1.14.0</GameVersion>
        <Description>Description...</Description>
        <Features>
            {
                "CountersPlus.CustomCounter": {
                    "Name": "Nightscout Counter",
                    "Description": "Reads blood sugar from a nightscout site.",
                    "CounterLocation": "NightscoutCounter.Counters.NightscoutCounterCountersPlus",
                    "ConfigDefaults": {
                        "Enabled": true,
                        "Position": "AboveMultiplier",
                        "Distance": 0
                    },
                    "BSML": {
                        "Resource": "NightscoutCounter.UI.Views.NightscoutSettings.bsml",
                        "Host": "NightscoutCounter.UI.NightscoutSettingsHandler",
                        "Icon": "NightscoutCounter.UI.Images.nightscout-counter.png"
                    }
                }
            }
        </Features>
    </PropertyGroup>
    <Target Name="RunGenerateManifest" BeforeTargets="Build">
        <GenerateManifest 
            <!-- Other Parameters -->
            Features="$(Features)"
            <!-- Other Parameters -->
        />
    </Target>
</Project>
```
