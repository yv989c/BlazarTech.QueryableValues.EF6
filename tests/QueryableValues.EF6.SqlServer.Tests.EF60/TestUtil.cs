using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public static class TestUtil
    {
        public static List<T> ToList<T>(this IEnumerable<T> sequence, int capacity)
        {
            var list = new List<T>(capacity);
            list.AddRange(sequence);
            return list;
        }

        public static IEnumerable<short> GetSequenceOfInt16(int numberOfElements, bool withCount)
        {
            return withCount ? getSequence().ToList(numberOfElements) : getSequence();

            IEnumerable<short> getSequence()
            {
                yield return 0;
                yield return short.MinValue;
                yield return short.MaxValue;

                var random = new Random(1);

                for (var i = 0; i < numberOfElements - 3; i++)
                {
                    yield return (short)random.Next(short.MinValue, short.MaxValue);
                }
            }
        }

        public static IEnumerable<int> GetSequenceOfInt32(int numberOfElements, bool withCount)
        {

            return withCount ? getSequence().ToList(numberOfElements) : getSequence();

            IEnumerable<int> getSequence()
            {
                yield return 0;
                yield return int.MinValue;
                yield return int.MaxValue;

                var random = new Random(1);

                for (var i = 0; i < numberOfElements - 3; i++)
                {
                    yield return random.Next(int.MinValue, int.MaxValue);
                }
            }
        }

        public static IEnumerable<long> GetSequenceOfInt64(int numberOfElements, bool withCount)
        {
            return withCount ? getSequence().ToList(numberOfElements) : getSequence();

            IEnumerable<long> getSequence()
            {
                yield return 0;
                yield return long.MinValue;
                yield return long.MaxValue;

                var random = new Random(1);

                for (var i = 0; i < numberOfElements - 3; i++)
                {
                    yield return random.Next(int.MinValue, int.MaxValue);
                }
            }
        }

    }
}
