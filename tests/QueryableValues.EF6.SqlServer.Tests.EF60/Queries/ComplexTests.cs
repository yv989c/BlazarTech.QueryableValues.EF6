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
        public async Task Complex1(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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
        public async Task Complex2(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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
        public async Task Complex3(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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
        public async Task Complex4(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            using (var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120))
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

        [Theory]
        [MemberData(nameof(Data))]
        public void DocsExamples<T>(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, bool withCount, bool isEmpty)
        {
            IEnumerable<int> values = isEmpty ? TestUtil.ArrayEmptyInt32 : GetSequence(Enumerable.Range(1, 4), withCount);

            using var dbContext = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120);

            {
                var qvQuery = dbContext.AsQueryableValues(values);

                // Example #1 (LINQ method syntax)
                var myQuery1 = dbContext.TestData
                    .Where(e => qvQuery.Contains(e.Id))
                    .Select(i => new
                    {
                        i.Id,
                        i.GuidValue
                    })
                    .ToList();

                AssertUtil.EqualContent(values, myQuery1.Select(i => i.Id));

                // Example #2 (LINQ query syntax)
                var myQuery2 = (
                    from e in dbContext.TestData
                    where qvQuery.Contains(e.Id)
                    select new
                    {
                        e.Id,
                        e.GuidValue
                    })
                    .ToList();

                AssertUtil.EqualContent(values, myQuery2.Select(i => i.Id));
            }

            {
                // Example #1 (LINQ method syntax)
                var myQuery1 = dbContext.TestData
                    .Join(
                        dbContext.AsQueryableValues(values),
                        e => e.Id,
                        v => v,
                        (e, v) => new
                        {
                            e.Id,
                            e.GuidValue
                        }
                    )
                    .ToList();

                AssertUtil.EqualContent(values, myQuery1.Select(i => i.Id));

                // Example #2 (LINQ query syntax)
                var myQuery2 = (
                    from e in dbContext.TestData
                    join v in dbContext.AsQueryableValues(values) on e.Id equals v
                    select new
                    {
                        e.Id,
                        e.GuidValue
                    })
                    .ToList();

                AssertUtil.EqualContent(values, myQuery2.Select(i => i.Id));
            }

            {
                IEnumerable<int> values1 = isEmpty ? TestUtil.ArrayEmptyInt32 : GetSequence(Enumerable.Range(1, 2), withCount);
                IEnumerable<int> values2 = isEmpty ? TestUtil.ArrayEmptyInt32 : GetSequence(Enumerable.Range(3, 2), withCount);

                var qvQuery1 = dbContext.AsQueryableValues(values1);
                var qvQuery2 = dbContext.AsQueryableValues(values2);

                var myQuery = (
                    from e in dbContext.TestData
                    where qvQuery1.Contains(e.Id) || qvQuery2.Contains(e.Id)
                    select new
                    {
                        e.Id,
                        e.GuidValue
                    })
                    .ToList();

                AssertUtil.EqualContent(values1.Concat(values2), myQuery.Select(i => i.Id));
            }
        }
    }
}
