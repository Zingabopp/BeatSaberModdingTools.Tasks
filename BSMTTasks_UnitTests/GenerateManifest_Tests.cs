using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BeatSaberModdingTools.Tasks;
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
                DependsOn = new string[] { "BSIPA|^4.3.0", "TestDepend1|^2.0.1", "TestDepend2|^1.0.0" },
                RequiresBsipa = true,
                TargetPath = targetPath
            };
            Assert.IsTrue(task.Execute());
        }

        [TestMethod]
        public void Valid_WithBaseManifest_DependsOnSingle()
        {
            Directory.CreateDirectory(OutputPath);
            string basePath = Path.Combine(Data, "manifest.json");
            string targetPath = Path.Combine(OutputPath, nameof(Valid_WithBaseManifest_DependsOnSingle) + ".json");
            var task = new GenerateManifest()
            {
                Id = "TestPlugin",
                Name = "Test Plugin",
                Author = "Zingabopp",
                Version = "1.0.0",
                GameVersion = "1.14.0",
                Description = "Description of a test plugin.",
                DependsOn = new string[] { "TestDepend1|^2.0.1;TestDepend2|^1.0.0" },
                RequiresBsipa = true,
                BaseManifestPath = basePath,
                TargetPath = targetPath
            };
            Assert.IsTrue(task.Execute());
        }
    }
}
