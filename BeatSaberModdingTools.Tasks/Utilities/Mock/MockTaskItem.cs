using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;

namespace BeatSaberModdingTools.Tasks.Utilities.Mock
{
    /// <summary>
    /// A mock <see cref="ITaskItem"/> for testing.
    /// </summary>
    public class MockTaskItem : ITaskItem
    {
        /// <summary>
        /// Create a new array of <see cref="MockTaskItem"/> from an array of dictionary strings.
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static MockTaskItem[] FromDictString(string propName, params string[] args)
        {
            List<MockTaskItem> items = new List<MockTaskItem>();
            var dict = ParseUtil.ParseDictString(args, propName);
            if (dict == null)
                return null;
            foreach (var pair in dict)
            {
                items.Add(new MockTaskItem(pair.Key, pair.Value));
            }
            return items.ToArray();
        }
        /// <inheritdoc/>
        public string ItemSpec { get => ToString(); set => Data["Include"] = value; }
        private Dictionary<string, string> Data;
        /// <summary>
        /// Create a new <see cref="MockTaskItem"/> from a dictionary of metadata and their values.
        /// </summary>
        /// <param name="data"></param>
        public MockTaskItem(Dictionary<string, string> data)
        {
            Data = data?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<string, string>();
        }
        /// <summary>
        /// Create a new <see cref="MockTaskItem"/> with the metadata Include and Version.
        /// </summary>
        /// <param name="include"></param>
        /// <param name="version"></param>
        public MockTaskItem(string include, string version)
        {
            Data = new Dictionary<string, string>(2);
            Data["Include"] = include;
            Data["Version"] = version;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Data.TryGetValue("Include", out string value))
                return value;
            return Data?.FirstOrDefault().Value ?? "<NULL>";
        }

        /// <inheritdoc/>
        public ICollection MetadataNames => Data.Keys.ToArray();

        /// <inheritdoc/>
        public int MetadataCount => Data.Count;

        /// <inheritdoc/>
        public IDictionary CloneCustomMetadata()
        {
            return Data.ToDictionary(p => p.Key, p => p.Value);
        }
        /// <inheritdoc/>
        public void CopyMetadataTo(ITaskItem destinationItem)
        {
            foreach (var pair in Data)
            {
                destinationItem.SetMetadata(pair.Key, pair.Value);
            }
        }

        /// <inheritdoc/>
        public string GetMetadata(string metadataName)
        {
            if (Data.TryGetValue(metadataName, out string value))
                return value;
            return null;
        }

        /// <inheritdoc/>
        public void RemoveMetadata(string metadataName)
        {
            if (metadataName != null)
                Data.Remove(metadataName);
        }

        /// <inheritdoc/>
        public void SetMetadata(string metadataName, string metadataValue)
        {
            Data[metadataName] = metadataValue;
        }
    }
}
