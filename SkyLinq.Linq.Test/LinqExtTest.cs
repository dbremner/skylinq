using System;
using Xunit;

namespace SkyLinq.Linq.Test
{
    public sealed class LinqExtTest
    {
        private readonly int[] _data;

        public LinqExtTest()
        {
            _data = new int[] { 3, 2, 7, 9, 5 };
        }

        [Fact]
        public void TestMaxWithIndex()
        {
            var result = _data.MaxWithIndex();
            Assert.Equal(3, result.Item2);
            Assert.Equal(9, result.Item1);
        }

        [Fact]
        public void TestMinWithIndex()
        {
            var result = _data.MinWithIndex();
            Assert.Equal(1, result.Item2);
            Assert.Equal(2, result.Item1);
        }
    }
}
