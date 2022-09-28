using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.Queries
{
    [Collection("DbContext")]
    public class SequenceTests
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
#if NET5_0_OR_GREATER
                var useDatabaseFirstOptions = new[] { false };
#else
                var useDatabaseFirstOptions = new[] { false, true };
#endif
                var useDatabaseNullSemanticsOptions = new[] { false, true };
                var numberOfElementsOptions = new[] { 0, 6, 1000 };
                var withCountOptions = new[] { false, true };
                var useCompat120DbOptions = new[] { false, true };

                foreach (var useDatabaseFirstOption in useDatabaseFirstOptions)
                {
                    foreach (var useDatabaseNullSemanticsOption in useDatabaseNullSemanticsOptions)
                    {
                        foreach (var numberOfElementsOption in numberOfElementsOptions)
                        {
                            foreach (var withCountOption in withCountOptions)
                            {
                                foreach (var useCompat120DbOption in useCompat120DbOptions)
                                {
                                    if (useCompat120DbOption && useDatabaseFirstOption)
                                    {
                                        continue;
                                    }

                                    yield return new object[] { useCompat120DbOption, useDatabaseFirstOption, useDatabaseNullSemanticsOption, numberOfElementsOption, withCountOption };
                                }
                            }
                        }
                    }
                }
            }
        }

        public SequenceTests(DbContextFixture _)
        {
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Byte(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, int numberOfElements, bool withCount)
        {
            var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120);
            var sequence = TestUtil.GetSequenceOfByte(numberOfElements, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int16(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, int numberOfElements, bool withCount)
        {
            using var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120);
            var sequence = TestUtil.GetSequenceOfInt16(numberOfElements, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int32(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, int numberOfElements, bool withCount)
        {
            using var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120);
            var sequence = TestUtil.GetSequenceOfInt32(numberOfElements, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int64(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, int numberOfElements, bool withCount)
        {
            using var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120);
            var sequence = TestUtil.GetSequenceOfInt64(numberOfElements, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task String(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, int numberOfElements, bool withCount)
        {
            using var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120);
            var sequence = TestUtil.GetSequenceOfString(numberOfElements, withCount, false);
            var result = await db.AsQueryableValues(sequence, isUnicode: false).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task StringUnicode(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, int numberOfElements, bool withCount)
        {
            using var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120);
            var sequence = TestUtil.GetSequenceOfString(numberOfElements, withCount, true);
            var result = await db.AsQueryableValues(sequence, isUnicode: true).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Guid(bool useCompat120, bool useDatabaseFirst, bool useDatabaseNullSemantics, int numberOfElements, bool withCount)
        {
            using var db = DbUtil.CreateDbContext(useDatabaseFirst, useDatabaseNullSemantics, useCompat120: useCompat120);
            var sequence = TestUtil.GetSequenceOfGuid(numberOfElements, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }
    }
}
