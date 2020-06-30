using BeatSaberModdingTools.Tasks;

namespace BSMTTasks_UnitTests
{
    public class MockGetCommitHash : GetCommitInfo
    {
        public MockGetCommitHash(string gitDirectory)
            : base()
        {
            GitDirectory = gitDirectory;
        }
    }
}
