using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using BeatSaberModdingTools.Tasks;
using BeatSaberModdingTools.Tasks.Utilties;
using System.Linq;
using System.IO;
using Microsoft.Build.Tasks;

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
            bool expectedReturn = false;
            string expectedMessage = @"Error in ReplaceInFile: 'File' is null or empty.";
            string expectedMessageCode = MessageCodes.ReplaceInFile.EmptyFile;
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
            Assert.AreEqual(expectedReturn, task.Execute());
            MockTaskLogger logger = task.Logger as MockTaskLogger;
            var logMessage = logger.LogEntries.First();
            Assert.AreEqual(expectedMessage, logMessage.Message);
            Assert.AreEqual(expectedMessageCode, logMessage.MessageCode);
        }

        [TestMethod]
        public void FileDoesntExist()
        {
            bool expectedReturn = false;
            string expectedMessage = @"Error in ReplaceInFile: File 'C:\DoesntExist.cs' does not exist.";

            string expectedMessageCode = MessageCodes.ReplaceInFile.MissingSource;
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
            Assert.AreEqual(expectedReturn, task.Execute());
            MockTaskLogger logger = task.Logger as MockTaskLogger;
            var logMessage = logger.LogEntries.First();
            Assert.AreEqual(expectedMessage, logMessage.Message);
            Assert.AreEqual(expectedMessageCode, logMessage.MessageCode);
        }

        [TestMethod]
        public void PatternEmpty()
        {
            bool expectedReturn = false;
            string expectedMessage = @"Error in ReplaceInFile: Pattern cannot be null or empty.";
            string expectedMessageCode = MessageCodes.ReplaceInFile.EmptyPattern;
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
            Assert.AreEqual(expectedReturn, task.Execute());
            MockTaskLogger logger = task.Logger as MockTaskLogger;
            var logMessage = logger.LogEntries.First();
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

            bool expectedReturn = true;
            string expectedMessage = @"Replacing '$test$' with '' in ";
            string expectedReplacedString = "Left  Right";
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
            Assert.AreEqual(expectedReturn, task.Execute());
            MockTaskLogger logger = task.Logger as MockTaskLogger;
            var logMessage = logger.LogEntries.First();
            Console.WriteLine(logMessage);
            Assert.IsTrue(logMessage.Message.StartsWith(expectedMessage));
            string replacedText = File.ReadAllText(file);
            Assert.AreEqual(expectedReplacedString, replacedText);
        }
    }
}
