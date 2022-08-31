using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public static class AssertUtil
    {
        public static void EqualContent<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.Equal(expected.OrderBy(i => i), actual.OrderBy(i => i));
        }
    }
}
