using System;

namespace BeatSaberModdingTools.Tasks.Utilities
{
    /// <summary>
    /// Defines a position in a file.
    /// </summary>
    public struct FilePosition
    {
        /// <summary>
        /// Line the <see cref="FilePosition"/> starts on.
        /// </summary>
        public readonly int StartLine;
        /// <summary>
        /// Line the <see cref="FilePosition"/> ends on.
        /// </summary>
        public readonly int EndLine;
        /// <summary>
        /// Column the <see cref="FilePosition"/> starts on.
        /// </summary>
        public readonly int StartColumn;
        /// <summary>
        /// Column the <see cref="FilePosition"/> ends on.
        /// </summary>
        public readonly int EndColumn;

        /// <summary>
        /// Defines a new <see cref="FilePosition"/> that covers a single line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="startColumn"></param>
        /// <param name="endColumn"></param>
        public FilePosition(int line, int startColumn = 0, int endColumn = 0)
        {
            StartLine = Math.Max(0, line);
            EndLine = StartLine;
            StartColumn = Math.Max(0, startColumn);
            EndColumn = Math.Max(StartColumn, endColumn);
        }

        /// <summary>
        /// Defines a new <see cref="FilePosition"/> that may cover multiple lines.
        /// </summary>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <param name="startColumn"></param>
        /// <param name="endColumn"></param>
        public FilePosition(int startLine, int endLine, int startColumn, int endColumn)
        {
            StartLine = Math.Max(0, startLine);
            EndLine = Math.Max(startLine, endLine);
            StartColumn = Math.Max(0, startColumn);
            EndColumn = StartLine == EndLine ? Math.Max(StartColumn, endColumn) : Math.Max(0, endColumn);
        }
    }
}
