using BeatSaberModdingTools.Tasks.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSMTTasks_UnitTests.Mocks
{
    public class MockGitRunner : IGitRunner
    {
        private Dictionary<GitArgument, string> Results = new Dictionary<GitArgument, string>();
        public MockGitRunner(params KeyValuePair<GitArgument, string>[] results)
        {
            foreach (var pair in results)
            {
                Results[pair.Key] = pair.Value;
            }
        }
        public MockGitRunner(params (GitArgument, string)[] results)
        {
            foreach (var pair in results)
            {
                Results[pair.Item1] = pair.Item2;
            }
        }
        public MockGitRunner(IEnumerable<KeyValuePair<GitArgument, string>> results)
        {
            foreach (var pair in results)
            {
                Results[pair.Key] = pair.Value;
            }
        }

        public void SetResult(GitArgument arg, string result)
        {
            Results[arg] = result;
        }

        public string GetTextFromProcess(GitArgument command)
        {
            if (Results.TryGetValue(command, out string result))
                return result;
            throw new InvalidOperationException($"Result for command '{command}' not found.");
        }
    }
}
