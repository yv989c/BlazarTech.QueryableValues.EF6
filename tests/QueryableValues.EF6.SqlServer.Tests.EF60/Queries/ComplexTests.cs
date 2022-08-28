using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.Queries
{
    [Collection("DbContext")]
    public class ComplexTests
    {
        public ComplexTests(DbContextFixture _)
        {
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                var options = new (bool WithCount, bool IsEmpty)[]
                {
                    (false, false),
                    (false, true),
                    (true, false),
                    (true, true)
                };

#if NET5_0_OR_GREATER
                foreach (var option in options)
                {
                    yield return new object[] { CodeFirst.TestDbContext.Create(), option.WithCount, option.IsEmpty };
                }
#else
                foreach (var option in options)
                {
                    yield return new object[] { CodeFirst.TestDbContext.Create(), option.WithCount, option.IsEmpty };
                }

                foreach (var option in options)
                {
                    yield return new object[] { DatabaseFirst.TestDbContext.Create(), option.WithCount, option.IsEmpty };
                }
#endif
            }
        }

        private static IEnumerable<T> GetSequence<T>(IEnumerable<T> sequence, bool withCount)
        {
            return withCount ? sequence.ToList() : sequence;
        }

        private static int[] GetExpected(bool isEmpty, params int[] expected)
        {
            return isEmpty ? Array.Empty<int>() : expected;
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Complex1<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequence = GetSequence(getSequence(), withCount);

                var sequenceInt16 = GetSequence(getSequenceInt16(), withCount);
                var qvInt16 = db.AsQueryableValues(sequenceInt16);

                var result = await (
                    from e in db.TestData
                    join v in db.AsQueryableValues(sequence) on e.ByteValue equals v
                    where qvInt16.Contains(e.Int16Value)
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 3);

                AssertUtil.EqualContent(expected, result);
            }

            IEnumerable<byte> getSequence()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return byte.MinValue;
                yield return byte.MaxValue;
            }

            IEnumerable<short> getSequenceInt16()
            {
                yield return short.MaxValue;
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Complex2<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequence = GetSequence(getSequence(), withCount);

                var sequenceInt16 = GetSequence(getSequenceInt16(), withCount);
                var qvInt16 = db.AsQueryableValues(sequenceInt16);

                var sequenceInt32 = GetSequence(getSequenceInt32(), withCount);
                var qvInt32 = db.AsQueryableValues(sequenceInt32);

                var result = await (
                    from e in db.TestData
                    join v in db.AsQueryableValues(sequence) on e.ByteValue equals v
                    where qvInt16.Contains(e.Int16Value) && qvInt32.Contains(e.Int32Value)
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 3);

                AssertUtil.EqualContent(expected, result);
            }

            IEnumerable<byte> getSequence()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return byte.MinValue;
                yield return byte.MaxValue;
            }

            IEnumerable<short> getSequenceInt16()
            {
                yield return short.MaxValue;
            }

            IEnumerable<int> getSequenceInt32()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return int.MaxValue;
                yield return 123;
            }
        }


        [Theory]
        [MemberData(nameof(Data))]
        public async Task Complex3<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequenceStringUnicode = GetSequence(getSequenceString(), withCount);
                var qvStringUnicode = db.AsQueryableValues(sequenceStringUnicode, isUnicode: true);

                var sequenceInt32 = GetSequence(getSequenceInt32(), withCount);
                var qvInt32 = db.AsQueryableValues(sequenceInt32);

                var result = await (
                    from e in db.TestData
                    where qvStringUnicode.Contains(e.StringUnicodeValue) || qvInt32.Contains(e.Int32Value)
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 1, 2, 4);

                AssertUtil.EqualContent(expected, result);
            }

            IEnumerable<string> getSequenceString()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return "你好！";
                yield return "👋";
            }

            IEnumerable<int> getSequenceInt32()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return 123;
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Complex4<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequenceStringUnicode = GetSequence(getSequenceString(), withCount);
                var qvStringUnicode = db.AsQueryableValues(sequenceStringUnicode, isUnicode: true);

                var sequenceInt32 = GetSequence(getSequenceInt32(), withCount);
                var qvInt32 = db.AsQueryableValues(sequenceInt32);

                var ids = (
                    from e in db.TestData
                    where qvStringUnicode.Contains(e.StringUnicodeValue) || qvInt32.Contains(e.Int32Value)
                    select e.Id
                    )
                    .Take(2);

                var result = await (
                    from e in db.TestData
                    join id in ids on e.Id equals id
                    orderby e.Id descending
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 4, 2);

                Assert.Equal(expected, result);
            }

            IEnumerable<string> getSequenceString()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return "你好！";
                yield return "👋";
            }

            IEnumerable<int> getSequenceInt32()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return 123;
            }
        }
    }
}
