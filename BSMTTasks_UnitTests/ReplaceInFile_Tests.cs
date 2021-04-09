using BeatSaberModdingTools.Tasks;
using BeatSaberModdingTools.Tasks.Utilities;
using BeatSaberModdingTools.Tasks.Utilities.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace BSMTTasks_UnitTests
{
    [TestClass]
    public class ReplaceInFile_Tests
    {
        public static readonly string DataFolder = Path.Combine("Data", "ReplaceInFile");
        public static readonly string OutputFolder = Path.Combine("Output", "ReplaceInFile");

        [TestMethod]
        public void EmptyFile()
        {
            string file = "";
            string pattern = null;
            string substitute = null;
            bool useRegex = false;
            bool regexMultiline = false;
            bool regexSingleline = false;
            bool escapeBackslash = false;
            ReplaceInFile task = new ReplaceInFile()
            {
                File = file,
                Pattern = pattern,
                Substitute = substitute,
                UseRegex = useRegex,
                RegexMultilineMode = regexMultiline,
                RegexSinglelineMode = regexSingleline,
                EscapeBackslash = escapeBackslash
            };
            bool expectedReturn = false;
            string expectedMessage = $"{task.GetType().Name}: 'File' is null or empty.";
            string expectedMessageCode = MessageCodes.ReplaceInFile.EmptyFile;

            Assert.AreEqual(expectedReturn, task.Execute());
            MockTaskLogger logger = task.Logger as MockTaskLogger;
            MockLogEntry logMessage = logger.LogEntries.First();
            Assert.AreEqual(expectedMessage, logMessage.Message);
            Assert.AreEqual(expectedMessageCode, logMessage.MessageCode);
        }

        [TestMethod]
        public void FileDoesntExist()
        {
            string file = "C:\\DoesntExist.cs";
            string pattern = null;
            string substitute = null;
            bool useRegex = false;
            bool regexMultiline = false;
            bool regexSingleline = false;
            bool escapeBackslash = false;
            ReplaceInFile task = new ReplaceInFile()
            {
                File = file,
                Pattern = pattern,
                Substitute = substitute,
                UseRegex = useRegex,
                RegexMultilineMode = regexMultiline,
                RegexSinglelineMode = regexSingleline,
                EscapeBackslash = escapeBackslash
            };
            bool expectedReturn = false;
            string expectedMessage = @$"{task.GetType().Name}: File 'C:\DoesntExist.cs' does not exist.";

            string expectedMessageCode = MessageCodes.ReplaceInFile.MissingSource;

            Assert.AreEqual(expectedReturn, task.Execute());
            MockTaskLogger logger = task.Logger as MockTaskLogger;
            MockLogEntry logMessage = logger.LogEntries.First();
            Assert.AreEqual(expectedMessage, logMessage.Message.Replace(Environment.CurrentDirectory + '/', ""));
            Assert.AreEqual(expectedMessageCode, logMessage.MessageCode);
        }

        [TestMethod]
        public void PatternEmpty()
        {
            string file = Path.Combine(DataFolder, "ReplaceTest.txt");
            string pattern = null;
            string substitute = null;
            bool useRegex = false;
            bool regexMultiline = false;
            bool regexSingleline = false;
            bool escapeBackslash = false;
            ReplaceInFile task = new ReplaceInFile()
            {
                File = file,
                Pattern = pattern,
                Substitute = substitute,
                UseRegex = useRegex,
                RegexMultilineMode = regexMultiline,
                RegexSinglelineMode = regexSingleline,
                EscapeBackslash = escapeBackslash
            };
            bool expectedReturn = false;
            string expectedMessage = @$"{task.GetType().Name}: Pattern cannot be null or empty.";
            string expectedMessageCode = MessageCodes.ReplaceInFile.EmptyPattern;

            Assert.AreEqual(expectedReturn, task.Execute());
            MockTaskLogger logger = task.Logger as MockTaskLogger;
            MockLogEntry logMessage = logger.LogEntries.First();
            Assert.AreEqual(expectedMessage, logMessage.Message);
            Assert.AreEqual(expectedMessageCode, logMessage.MessageCode);
        }


        [TestMethod]
        public void NormalReplace_NullSubstitute()
        {
            string fileName = "ReplaceTest.txt";
            string sourceFile = Path.Combine(DataFolder, fileName);
            string outDir = Path.Combine(OutputFolder, "NormalReplace_NullSubstitute");
            Directory.CreateDirectory(outDir);
            File.Copy(sourceFile, Path.Combine(outDir, fileName), true);

            string file = Path.Combine(outDir, fileName);
            string pattern = "$test$";
            string substitute = null;
            bool useRegex = false;
            bool regexMultiline = false;
            bool regexSingleline = false;
            bool escapeBackslash = false;
            ReplaceInFile task = new ReplaceInFile()
            {
                File = file,
                Pattern = pattern,
                Substitute = substitute,
                UseRegex = useRegex,
                RegexMultilineMode = regexMultiline,
                RegexSinglelineMode = regexSingleline,
                EscapeBackslash = escapeBackslash
            };
            bool expectedReturn = true;
            string expectedMessage = @$"{task.GetType().Name}: Replacing '$test$' with '' in ";
            string expectedReplacedString = "Left  Right";

            Assert.AreEqual(expectedReturn, task.Execute());
            MockTaskLogger logger = task.Logger as MockTaskLogger;
            MockLogEntry logMessage = logger.LogEntries.First();
            Console.WriteLine(logMessage);
            Assert.IsTrue(logMessage.Message.StartsWith(expectedMessage));
            string replacedText = File.ReadAllText(file);
            Assert.AreEqual(expectedReplacedString, replacedText);
        }
    }
}
