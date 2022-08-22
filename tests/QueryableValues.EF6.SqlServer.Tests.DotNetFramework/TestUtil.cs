using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public static class TestUtil
    {
        public static IEnumerable<int> GetSequenceOfInt32(int n, bool withCount)
        {
            var random = new Random(1);

            if (withCount)
            {
                var result = new List<int>(n);

                for (var i = 0; i < n; i++)
                {
                    result.Add(random.Next());
                }

                return result;
            }
            else
            {
                return getSequence();
            }

            IEnumerable<int> getSequence()
            {
                for (var i = 0; i < n; i++)
                {
                    yield return random.Next();
                }
            }
        }
    }
}
