using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public static class TestUtil
    {
        public static IEnumerable<int> GetInt32Sequence(int n)
        {
            var random = new Random(1);

            for (int i = 0; i < n; i++)
            {
                yield return random.Next();
            }
        }
    }
}
