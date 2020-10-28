using BeatSaberModdingTools.Tasks.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSMTTasks_UnitTests.Util_Tests
{
    [TestClass]
    public class MatchVersions_Tests
    {
        [TestMethod]
        public void TrailingZero1()
        {
            string version1 = "0.0.1.0";
            string version2 = "0.0.1";
            Assert.IsTrue(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void TrailingZero2()
        {
            string version1 = "1.0.1.0";
            string version2 = "1.0.1";
            Assert.IsTrue(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void TrailingZero3()
        {
            string version1 = "0.1.1.0";
            string version2 = "0.1.1";
            Assert.IsTrue(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void TrailingZero4()
        {
            string version1 = "0.1.0.0";
            string version2 = "0.1.0";
            Assert.IsTrue(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void TrailingZero5()
        {
            string version1 = "0.0.1.0";
            string version2 = "0.1.0-beta";
            Assert.IsTrue(Util.MatchVersions(version1, version2));
        }

        [TestMethod]
        public void TrailingZero6()
        {
            string version1 = "0.0.0.1";
            string version2 = "0.0.1";
            Assert.IsTrue(Util.MatchVersions(version1, version2));
        }

        [TestMethod]
        public void TrailingZero7()
        {
            string version1 = "0.1.0.1";
            string version2 = "1.0.1";
            Assert.IsTrue(Util.MatchVersions(version1, version2));
        }

        [TestMethod]
        public void Mismatch_TrailingZero1()
        {
            string version1 = "0.0.1.0";
            string version2 = "0.0.2";
            Assert.IsFalse(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void Mismatch_TrailingZero2()
        {
            string version1 = "1.0.1.0";
            string version2 = "1.0.2";
            Assert.IsFalse(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void Mismatch_TrailingZero3()
        {
            string version1 = "0.1.1.0";
            string version2 = "0.1.2";
            Assert.IsFalse(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void Mismatch_TrailingZero4()
        {
            string version1 = "0.1.0.0";
            string version2 = "0.2.0";
            Assert.IsFalse(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void Mismatch_TrailingZero5()
        {
            string version1 = "0.0.1.0";
            string version2 = "0.2.0";
            Assert.IsFalse(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void Mismatch_TrailingOne()
        {
            string version1 = "0.2.0.1";
            string version2 = "0.2.1";
            Assert.IsFalse(Util.MatchVersions(version1, version2));
        }

        [TestMethod]
        public void Mismatch_TrailingOne2()
        {
            string version1 = "0.1.0.1";
            string version2 = "1.0.2";
            Assert.IsFalse(Util.MatchVersions(version1, version2));
        }

        [TestMethod]
        public void Mismatch_TrailingOne3()
        {
            string version1 = "1.0.2.1";
            string version2 = "1.0.2";
            Assert.IsFalse(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void Mismatch_TrailingOne4()
        {
            string version1 = "1.1.1.1";
            string version2 = "1.1.1";
            Assert.IsFalse(Util.MatchVersions(version1, version2));
        }
        [TestMethod]
        public void Mismatch_TrailingOne5()
        {
            string version1 = "1.0.0.1";
            string version2 = "0.1.0";
            Assert.IsFalse(Util.MatchVersions(version1, version2));
        }
    }
}
