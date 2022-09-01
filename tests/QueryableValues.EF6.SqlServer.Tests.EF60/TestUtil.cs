using System;
using System.Collections.Generic;
using System.Text;

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

        public static IEnumerable<byte> GetSequenceOfByte(int numberOfElements, bool withCount)
        {
            return withCount ? getSequence().ToList(numberOfElements) : getSequence();

            IEnumerable<byte> getSequence()
            {
                yield return 0;
                yield return byte.MinValue;
                yield return byte.MaxValue;

                var random = new Random(1);

                for (var i = 0; i < numberOfElements - 3; i++)
                {
                    yield return (byte)random.Next(byte.MinValue, byte.MaxValue);
                }
            }
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

        public static IEnumerable<string> GetSequenceOfString(int numberOfElements, bool withCount, bool unicode)
        {
            return withCount ? getSequence().ToList(numberOfElements) : getSequence();

            IEnumerable<string> getSequence()
            {
                var random = new Random(1);
                var sb = new StringBuilder();

                for (var i = 0; i < numberOfElements; i++)
                {
                    sb.Clear();
                    var wordLength = random.Next(0, 50);

                    for (var x = 0; x < wordLength; x++)
                    {
                        var c = (char)random.Next(32, unicode ? short.MaxValue : 255);

                        if (char.IsControl(c))
                        {
                            continue;
                        }

                        sb.Append(c);
                    }

                    yield return sb.ToString();
                }
            }
        }

        public static IEnumerable<Guid> GetSequenceOfGuid(int numberOfElements, bool withCount)
        {
            return withCount ? getSequence().ToList(numberOfElements) : getSequence();

            IEnumerable<Guid> getSequence()
            {
                yield return Guid.Empty;

                var random = new Random(1);
                var bytes = new byte[16];

                for (var i = 0; i < numberOfElements - 1; i++)
                {
                    random.NextBytes(bytes);
                    yield return new Guid(bytes);
                }
            }
        }

        public static IEnumerable<(bool UseDatabaseFirst, bool UseDatabaseNullSemantics, bool IsEmpty, bool WithCount)> GetTestDataOptions()
        {
#if NET5_0_OR_GREATER
            var useDatabaseFirstOptions = new[] { false };
#else
            var useDatabaseFirstOptions = new[] { false, true };
#endif
            var useDatabaseNullSemanticsOptions = new[] { false, true };
            var isEmptyOptions = new[] { false, true };
            var withCountOptions = new[] { false, true };

            foreach (var useDatabaseFirstOption in useDatabaseFirstOptions)
            {
                foreach (var useDatabaseNullSemanticsOption in useDatabaseNullSemanticsOptions)
                {
                    foreach (var isEmptyOption in isEmptyOptions)
                    {
                        foreach (var withCountOption in withCountOptions)
                        {
                            yield return (useDatabaseFirstOption, useDatabaseNullSemanticsOption, isEmptyOption, withCountOption);
                        }
                    }
                }
            }
        }
    }
}
