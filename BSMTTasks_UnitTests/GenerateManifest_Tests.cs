using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BeatSaberModdingTools.Tasks;
using BeatSaberModdingTools.Tasks.Models;
using BeatSaberModdingTools.Tasks.Utilities;
using BeatSaberModdingTools.Tasks.Utilities.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BSMTTasks_UnitTests
{
    [TestClass]
    public class GenerateManifest_Tests
    {
        public static readonly string OutputPath = Path.Combine("Output", "GenerateManifests");
        public static readonly string Data = Path.Combine("Manifests");

        [TestMethod]
        public void Valid_FullyGenerated_DependsOnArray()
        {
            Directory.CreateDirectory(OutputPath);
            string targetPath = Path.Combine(OutputPath, nameof(Valid_FullyGenerated_DependsOnArray) + ".json");
            var task = new GenerateManifest()
            {
                Id = "TestPlugin",
                Name = "Test Plugin",
                Author = "Zingabopp",
                Version = "1.0.0",
                GameVersion = "1.14.0",
                Description = "Description of a test plugin.",
                DependsOn = 
                    MockTaskItem.FromDictString("DependsOn", "BSIPA|^4.3.0", "TestDepend1|^2.0.1", "TestDepend2|^1.0.0" ),
                RequiresBsipa = true,
                TargetPath = targetPath
            };
            Assert.IsTrue(task.Execute());
            BsipaManifest manifest = BsipaManifest.FromJson(File.ReadAllText(targetPath));
            TestManifest(task, manifest);
        }

        [TestMethod]
        public void Valid_FullyGenerated_FilesStr()
        {
            Directory.CreateDirectory(OutputPath);
            string targetPath = Path.Combine(OutputPath, nameof(Valid_FullyGenerated_FilesStr) + ".json");
            var task = new GenerateManifest()
            {
                Id = "TestPlugin",
                Name = "Test Plugin",
                Author = "Zingabopp",
                Version = "1.0.0",
                GameVersion = "1.14.0",
                Description = "Description of a test plugin.",
                Files = new string[] { "Libs/TestLib1.dll;Libs/TestLib2.dll" },
                RequiresBsipa = false,
                TargetPath = targetPath
            };
            Assert.IsTrue(task.Execute());
            BsipaManifest manifest = BsipaManifest.FromJson(File.ReadAllText(targetPath));
            TestManifest(task, manifest);
        }

        [TestMethod]
        public void Valid_FullyGenerated_FilesAry()
        {
            Directory.CreateDirectory(OutputPath);
            string targetPath = Path.Combine(OutputPath, nameof(Valid_FullyGenerated_FilesAry) + ".json");
            var task = new GenerateManifest()
            {
                Id = "TestPlugin",
                Name = "Test Plugin",
                Author = "Zingabopp",
                Version = "1.0.0",
                GameVersion = "1.14.0",
                Description = "Description of a test plugin.",
                DependsOn =
                    MockTaskItem.FromDictString("DependsOn", "BSIPA|^4.3.0", "TestDepend1|^2.0.1", "TestDepend2|^1.0.0"),
                ConflictsWith =
                    MockTaskItem.FromDictString("ConflictsWith", "TestConflict1|^2.0.1", "TestConflict2|^1.0.0"),
                Files = new string[] { "Libs/TestLib1.dll", "Libs/TestLib2.dll" },
                Donate = "http://donate.com",
                ProjectHome = "http://project.home",
                ProjectSource = "http://project.source",
                PluginHint = "Namespace.Plugin.Type",
                RequiresBsipa = false,
                TargetPath = targetPath
            };
            Assert.IsTrue(task.Execute());
            BsipaManifest manifest = BsipaManifest.FromJson(File.ReadAllText(targetPath));
            TestManifest(task, manifest);
            Assert.AreEqual(task.Donate, manifest.Donate);
            Assert.AreEqual(task.ProjectHome, manifest.ProjectHome);
            Assert.AreEqual(task.ProjectSource, manifest.ProjectSource);
            Assert.AreEqual(task.PluginHint, manifest.PluginHint);
        }

        [TestMethod]
        public void Valid_WithBaseManifest_DependsOnSingle()
        {
            Directory.CreateDirectory(OutputPath);
            string basePath = Path.Combine(Data, "manifest.json");
            BsipaManifest baseManifest = BsipaManifest.FromJson(File.ReadAllText(basePath));
            Assert.IsFalse(string.IsNullOrWhiteSpace(baseManifest.GameVersion));
            Assert.IsTrue(File.Exists(basePath));
            int baseDepends = 2;
            int baseConflicts = 0;
            string targetPath = Path.Combine(OutputPath, nameof(Valid_WithBaseManifest_DependsOnSingle) + ".json");            
            var task = new GenerateManifest()
            {
                Id = "TestPlugin",
                Name = "Test Plugin",
                Author = "Zingabopp",
                Version = "1.0.0",
                //GameVersion = "1.14.0",
                Description = "Description of a test plugin.",
                DependsOn =
                    MockTaskItem.FromDictString("DependsOn", "BSIPA|^4.3.0", "TestDepend1|^2.0.1", "TestDepend2|^1.0.0"),
                ConflictsWith =
                    MockTaskItem.FromDictString("ConflictsWith", "TestConflict1|^2.0.1", "TestConflict2|^1.0.0"),
                RequiresBsipa = true,
                BaseManifestPath = basePath,
                TargetPath = targetPath
            };
            Assert.IsTrue(task.Execute());
            BsipaManifest manifest = BsipaManifest.FromJson(File.ReadAllText(targetPath));
           
            TestManifest(task, manifest, baseDepends, baseConflicts);
        }

        [TestMethod]
        public void AllBase_NoOverwrite()
        {
            Directory.CreateDirectory(OutputPath);
            string basePath = Path.Combine(Data, "Original-all.json");
            string targetPath = Path.Combine(OutputPath, nameof(AllBase_NoOverwrite) + ".json");
            Console.WriteLine($"Writing output to '{Path.GetFullPath(targetPath)}'");
            var task = new GenerateManifest()
            {
                BaseManifestPath = basePath,
                TargetPath = targetPath
            };
            Assert.IsTrue(task.Execute());
        }
        [TestMethod]
        public void MergeAllTest()
        {
            Directory.CreateDirectory(OutputPath);
            string basePath = Path.Combine(Data, "overwrite.json");
            string targetPath = Path.Combine(OutputPath, nameof(MergeAllTest) + ".json");
            Console.WriteLine($"Writing output to '{Path.GetFullPath(targetPath)}'");
            var task = new GenerateManifest()
            {
                Id = "New-ID",
                Name = "New-Name",
                Author = "New-Author",
                Version = "1.0.0-New",
                GameVersion = "1.14.0-New",
                Description = "New-Description",
                Files = new string[] { "Libs/New-File1.dll", "Libs/New-File2.dll" },
                DependsOn =
                    MockTaskItem.FromDictString("DependsOn", "BSIPA|^NewVersion", "New-Depend|^2.0.1", "TestDepend2|^1.0.0"),
                ConflictsWith =
                    MockTaskItem.FromDictString("ConflictsWith", "BeatSaberPlus|^NewVersion", "New-Conflict|^1.0.0"),

                LoadBefore = new string[] { "Libs/New-LoadBefore1.dll", "Libs/New-LoadBefore2.dll" },
                LoadAfter = new string[] { "Libs/New-LoadAfter1.dll", "Libs/New-LoadAfter2.dll" },
                Icon = "New-Icon.png",
                Donate = "http://New-donate.com",
                ProjectHome = "http://New-project.home",
                ProjectSource = "http://New-project.source",
                Features = "{ \"New-Feature\" : { \"name\" : \"New-feature-name\" } }",
                Misc = "{ \"Other-Misc\" : { \"name\" : \"New-Misc-name\" } }",
                PluginHint = "Namespace.Plugin.Type",
                RequiresBsipa = true,
                BaseManifestPath = basePath,
                TargetPath = targetPath
            };
            Assert.IsTrue(task.Execute());
        }

        [TestMethod]
        public void SimpleJSONTest()
        {
            Directory.CreateDirectory(OutputPath);
            string targetPath = Path.Combine(OutputPath, nameof(SimpleJSONTest) + ".json");
            BsipaManifest manifest = new BsipaManifest()
            {
                Id = "TestPlugin",
                Name = "Test Plugin",
                Author = "Zingabopp",
                Version = "1.0.0",
                GameVersion = "1.14.0",
                Description = "Description of a test plugin."
            };
            manifest.DependsOn = ParseUtil.ParseDictString(new string[] { "TestDepend1|^2.0.1;TestDepend2|^1.0.0" }, "DependsOn");
            manifest.Validate(false);
            File.WriteAllText(targetPath, manifest.ToJson());
        }


        [TestMethod]
        public void InvalidProperties_ThrowsException()
        {
            Directory.CreateDirectory(OutputPath);
            string targetPath = Path.Combine(OutputPath, nameof(InvalidProperties_ThrowsException) + ".json");
            MockTaskLogger logger = new MockTaskLogger(nameof(GenerateManifest));
            var task = new GenerateManifest()
            {
                RequiresBsipa = false,
                TargetPath = targetPath,
                Logger = logger
            };
            Assert.IsFalse(task.Execute());
            var logEntry = logger.LogEntries.First();
            Console.Write(logEntry.Message);
            var exception = logEntry.Exception as ManifestValidationException;
            Assert.IsNotNull(exception);
            var props = exception.InvalidProperties;
            Assert.IsTrue(props.Any(p => p == nameof(GenerateManifest.Id)));
            Assert.IsTrue(props.Any(p => p == nameof(GenerateManifest.Name)));
            Assert.IsTrue(props.Any(p => p == nameof(GenerateManifest.Author)));
            Assert.IsTrue(props.Any(p => p == nameof(GenerateManifest.Version)));
            Assert.IsTrue(props.Any(p => p == nameof(GenerateManifest.GameVersion)));
            Assert.IsTrue(props.Any(p => p == nameof(GenerateManifest.Description)));
        }




        public void TestManifest(GenerateManifest task, BsipaManifest manifest, int baseDepends = 0, int baseConflicts = 0)
        {
            Assert.AreEqual(task.Id, manifest.Id);
            Assert.AreEqual(task.Name , manifest.Name);
            Assert.AreEqual(task.Author , manifest.Author);
            Assert.AreEqual(task.Version , manifest.Version);
            Assert.AreEqual(task.GameVersion , manifest.GameVersion);
            Assert.AreEqual(task.Description , manifest.Description);
            Assert.AreEqual(task.Icon , manifest.Icon);
            CompareDictionary(ParseUtil.ParseModIds(task.DependsOn, null, "DependsOn"), manifest.DependsOn, baseDepends);
            CompareDictionary(ParseUtil.ParseModIds(task.ConflictsWith, null, "ConflictsWith"), manifest.ConflictsWith, baseConflicts);
            CompareStringArrays(ParseUtil.ParseStringArray(task.Files), manifest.Files, 0);
        }

        public void CompareDictionary(Dictionary<string, string> expected, Dictionary<string, string> actual, int baseEntries)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }
            Assert.AreEqual(expected.Count + baseEntries, actual.Count);
            foreach (var key in expected.Keys)
            {
                Assert.IsTrue(expected[key] == actual[key]);
            }
        }

        public void CompareStringArrays(string[] expected, string[] actual, int baseEntries)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }
            else
                Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Length + baseEntries, actual.Length);
            foreach (var exp in expected)
            {
                Assert.IsTrue(actual.Any(e => exp.Equals(e)));
            }
        }
    }
}
