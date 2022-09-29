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
                foreach (var option in TestUtil.GetTestDataOptions())
                {
                    yield return new object[] { option.UseCompat120, option.UseDatabaseFirst, option.UseDatabaseNullSemantics, option.WithCount, option.IsEmpty };
                }
            }
        }

        private static IEnumerable<T> GetSequence<T>(IEnumerable<T> sequence, bool withCount)
        {
            return withCount ? sequence.ToList() : sequence;
        }

        private static int[] GetExpected(bool isEmpty, params int[] expected)
        {
            return isEmpty ? TestUtil.ArrayEmptyInt32 : expected;
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Byte(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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
        public async Task Int16(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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
        public async Task Int32(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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
        public async Task Int64(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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
        public async Task String(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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
        public async Task StringUnicode(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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
        public async Task Guid(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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
