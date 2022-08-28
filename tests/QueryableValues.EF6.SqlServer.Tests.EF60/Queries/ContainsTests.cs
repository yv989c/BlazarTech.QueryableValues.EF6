using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.Queries
{
    [Collection("DbContext")]
    public class ContainsTests
    {
        public ContainsTests(DbContextFixture _)
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
        public async Task Byte<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequence = GetSequence(getSequence(), withCount);
                var qv = db.AsQueryableValues(sequence);
                var result = await (
                    from e in db.TestData
                    where qv.Contains(e.ByteValue)
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 1, 2, 3);

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
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int16<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequence = GetSequence(getSequence(), withCount);
                var qv = db.AsQueryableValues(sequence);
                var result = await (
                    from e in db.TestData
                    where qv.Contains(e.Int16Value)
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 3);

                AssertUtil.EqualContent(expected, result);
            }

            IEnumerable<short> getSequence()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return short.MaxValue;
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int32<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequence = GetSequence(getSequence(), withCount);
                var qv = db.AsQueryableValues(sequence);
                var result = await (
                    from e in db.TestData
                    where qv.Contains(e.Int32Value)
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 2, 3, 4);

                AssertUtil.EqualContent(expected, result);
            }

            IEnumerable<int> getSequence()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return 0;
                yield return int.MaxValue;
                yield return 123;
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int64<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequence = GetSequence(getSequence(), withCount);
                var qv = db.AsQueryableValues(sequence);
                var result = await (
                    from e in db.TestData
                    where qv.Contains(e.Int64Value)
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 1, 3);

                AssertUtil.EqualContent(expected, result);
            }

            IEnumerable<long> getSequence()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return long.MinValue;
                yield return long.MaxValue;
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task String<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequence = GetSequence(getSequence(), withCount);
                var qv = db.AsQueryableValues(sequence, false);
                var result = await (
                    from e in db.TestData
                    where qv.Contains(e.StringValue)
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 3, 4);

                AssertUtil.EqualContent(expected, result);
            }

            IEnumerable<string> getSequence()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return "Olá!";
                yield return "Hi!";
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task StringUnicode<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequence = GetSequence(getSequence(), withCount);
                var qv = db.AsQueryableValues(sequence, true);
                var result = await (
                    from e in db.TestData
                    where qv.Contains(e.StringUnicodeValue)
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 1, 2);

                AssertUtil.EqualContent(expected, result);
            }

            IEnumerable<string> getSequence()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return "你好！";
                yield return "👋";
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Guid<T>(T db, bool withCount, bool isEmpty)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequence = GetSequence(getSequence(), withCount);
                var qv = db.AsQueryableValues(sequence);
                var result = await (
                    from e in db.TestData
                    where qv.Contains(e.GuidValue)
                    select e.Id
                    )
                    .ToListAsync();

                var expected = GetExpected(isEmpty, 1, 4);

                AssertUtil.EqualContent(expected, result);
            }

            IEnumerable<Guid> getSequence()
            {
                if (isEmpty)
                {
                    yield break;
                }

                yield return System.Guid.Empty;
                yield return new Guid("4ec4f690-a13c-4669-b622-351b3e568e68");
            }
        }
    }
}
