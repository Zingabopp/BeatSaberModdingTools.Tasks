using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BeatSaberModdingTools.Tasks
{
    /// <summary>
    /// Replaces a string pattern with a value in a file.
    /// </summary>
    public class ReplaceInFile : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// File path.
        /// </summary>
        [Required]
        public virtual string File { get; set; }
        /// <summary>
        /// Pattern to match (case sensitive).
        /// </summary>
        [Required]
        public virtual string Pattern { get; set; }
        /// <summary>
        /// String to substitute.
        /// </summary>
        [Required]
        public virtual string Substitute { get; set; }

        /// <summary>
        /// Set to true if <see cref="Pattern"/> is a regular expression.
        /// </summary>
        public virtual bool UseRegex { get; set; }
        /// <summary>
        /// Changes '^' and '$ so they match the beginning and end of a line instead of the entire string.
        /// </summary>
        public virtual bool RegexMultilineMode { get; set; }
        /// <summary>
        /// Changes the meaning of '.' so it matches every character except '\n' (newline).
        /// </summary>
        public virtual bool RegexSinglelineMode { get; set; }
        /// <summary>
        /// Escapes the '\' character in Substitute with '\\'. Does not escape '\n'.
        /// </summary>
        public virtual bool EscapeBackslash { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>true if successful</returns>
        public override bool Execute()
        {
            string errorCode = "";
            try
            {
                if (string.IsNullOrEmpty(File))
                {
                    errorCode = MessageCodes.ReplaceInFile.MissingSource;
                    throw new ArgumentNullException(nameof(File), $"{nameof(File)} is null or empty.");
                }
                FileInfo sourceFile = new FileInfo(File);
                if (!sourceFile.Exists)
                {
                    errorCode = MessageCodes.ReplaceInFile.MissingSource;
                    throw new FileNotFoundException($"File '{sourceFile.FullName}' does not exist.");
                }
                if (string.IsNullOrEmpty(Pattern))
                {
                    errorCode = MessageCodes.ReplaceInFile.EmptyPattern;
                    throw new ArgumentNullException(nameof(Pattern), $"{nameof(Pattern)} cannot be null or empty.");
                }
                if (Substitute == null)
                    Substitute = "";
                string fileText = System.IO.File.ReadAllText(sourceFile.FullName);
                if(EscapeBackslash)
                    Substitute = Substitute.Replace(@"\", @"\\");
                Log.LogMessage(MessageImportance.High, $"Replacing '{Pattern}' with '{Substitute}' in {sourceFile.FullName}");
                if (UseRegex)
                {
                    RegexOptions options = RegexOptions.None;
                    if (RegexMultilineMode)
                        options |= RegexOptions.Multiline;
                    if (RegexSinglelineMode)
                        options |= RegexOptions.Singleline;
                    fileText = Regex.Replace(fileText, Pattern, Substitute, options);
                }
                else
                    fileText = fileText.Replace(Pattern, Substitute);
                System.IO.File.WriteAllText(sourceFile.FullName, fileText);
                return true;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(errorCode))
                    errorCode = MessageCodes.ReplaceInFile.ReplaceFailed;
                if (BuildEngine != null)
                {
                    int line = BuildEngine.LineNumberOfTaskNode;
                    int column = BuildEngine.ColumnNumberOfTaskNode;
                    Log.LogError("Compile", errorCode, null, BuildEngine.ProjectFileOfTaskNode, line, column, line, column, $"Error in {GetType().Name}: {ex.Message}");
                }
                else
                {
                    Log.LogError($"Error in {GetType().Name}: {ex.Message}");
                }
                return false;
            }
        }
    }
}
