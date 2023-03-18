using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberModdingTools.Tasks;
using BeatSaberModdingTools.Tasks.Utilities.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BSMTTasks_UnitTests;
[TestClass]
public class SetActionOutput_Tests
{
    public static readonly string OutputFolder = Path.Combine("Output", "SetActionOutput");

    [TestMethod]
    public void ExistingDefaultPathVariable()
    {
        string outputPath = Path.Combine(OutputFolder, $"{nameof(ExistingDefaultPathVariable)}.txt");
        string outputName = "test1";
        string outputValue = "test_value1";
        Environment.SetEnvironmentVariable("GITHUB_OUTPUT", outputPath);
        var task = new SetActionOutput()
        {
            OutputName = outputName,
            OutputValue = outputValue
        };
        var result = task.Execute();
        MockTaskLogger logger = task.Logger as MockTaskLogger;
        
        Assert.IsTrue(result);
        string[] output = File.ReadAllLines(outputPath);
        Assert.AreEqual(1, output.Length);
        Assert.AreEqual($"{outputName}={outputValue}", output[0]);
    }
}
