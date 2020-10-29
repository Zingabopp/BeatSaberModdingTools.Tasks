using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberModdingTools.Tasks.Utilities
{
    /// <summary>
    /// Interface for an object that retrieves text from the 'git' command.
    /// </summary>
    public interface IGitRunner
    {
        /// <summary>
        /// Runs the given command and returns the output as a string.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="GitRunnerException"></exception>
        string GetTextFromProcess(GitArgument command);
    }

    /// <summary>
    /// Type of git command.
    /// </summary>
    public enum GitArgument
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Runs the 'status' command.
        /// </summary>
        Status = 1,
        /// <summary>
        /// Runs the 'rev-parse HEAD' command.
        /// </summary>
        CommitHash = 2,
        /// <summary>
        /// Runs the 'config --local --get remote.origin.url' command.
        /// </summary>
        OriginUrl = 3
    }

    /// <summary>
    /// An exception that may be thrown by <see cref="IGitRunner"/>.
    /// </summary>
    public class GitRunnerException : Exception
    {
        /// <inheritdoc/>
        public GitRunnerException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public GitRunnerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
