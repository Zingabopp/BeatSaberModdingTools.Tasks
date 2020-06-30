using BeatSaberModdingTools.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSMTTasks_UnitTests
{
    public class MockGetCommitHash : GetCommitInfo
    {
        public MockGetCommitHash(string gitDirectory)
            :base()
        {
            GitDirectory = gitDirectory;
        }
    }
}
