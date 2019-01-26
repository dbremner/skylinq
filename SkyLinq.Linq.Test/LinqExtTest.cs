using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SkyLinq.Linq.Test
{
    [TestClass]
    public sealed class LinqExtTest
    {
        int[] _data;
        [TestInitialize]
        public void SetUp()
        {
            _data = new int[] { 3, 2, 7, 9, 5 };
        }

        [TestMethod]
        public void TestMaxWithIndex()
        {
            var result = _data.MaxWithIndex();
            Assert.AreEqual(3, result.Item2);
            Assert.AreEqual(9, result.Item1);
        }

        [TestMethod]
        public void TestMinWithIndex()
        {
            var result = _data.MinWithIndex();
            Assert.AreEqual(1, result.Item2);
            Assert.AreEqual(2, result.Item1);
        }
    }
}
