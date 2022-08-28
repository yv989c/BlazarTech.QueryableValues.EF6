using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.Queries
{
    [Collection("DbContext")]
    public class SequenceTests
    {
        private readonly DbContextFixture _dbContextFixture;

        public static IEnumerable<object[]> Data
        {
            get
            {
                var numberOfElementsList = new[] { 0, 6, 1000 };

#if NET5_0_OR_GREATER
                foreach (var numberOfElements in numberOfElementsList)
                {
                    yield return new object[] { false, numberOfElements, false };
                    yield return new object[] { false, numberOfElements, true };
                }
#else
                foreach (var numberOfElements in numberOfElementsList)
                {
                    yield return new object[] { false, numberOfElements, false };
                    yield return new object[] { false, numberOfElements, true };
                }

                foreach (var numberOfElements in numberOfElementsList)
                {
                    yield return new object[] { true, numberOfElements, false };
                    yield return new object[] { true, numberOfElements, true };
                }
#endif
            }
        }

        public SequenceTests(DbContextFixture dbContextFixture)
        {
            _dbContextFixture = dbContextFixture;
        }

        private DbContext GetDb(bool useDatabaseFirst)
        {
            return useDatabaseFirst ? (DbContext)_dbContextFixture.DatabaseFirstDb : _dbContextFixture.CodeFirstDb;
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Byte(bool useDatabaseFirst, int numberOfElements, bool withCount)
        {
            var db = GetDb(useDatabaseFirst);
            var sequence = TestUtil.GetSequenceOfByte(numberOfElements, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int16(bool useDatabaseFirst, int numberOfElements, bool withCount)
        {
            var db = GetDb(useDatabaseFirst);
            var sequence = TestUtil.GetSequenceOfInt16(numberOfElements, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int32(bool useDatabaseFirst, int numberOfElements, bool withCount)
        {
            var db = GetDb(useDatabaseFirst);
            var sequence = TestUtil.GetSequenceOfInt32(numberOfElements, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int64(bool useDatabaseFirst, int numberOfElements, bool withCount)
        {
            var db = GetDb(useDatabaseFirst);
            var sequence = TestUtil.GetSequenceOfInt64(numberOfElements, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task String(bool useDatabaseFirst, int numberOfElements, bool withCount)
        {
            var db = GetDb(useDatabaseFirst);
            var sequence = TestUtil.GetSequenceOfString(numberOfElements, withCount, false);
            var result = await db.AsQueryableValues(sequence, isUnicode: false).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task StringUnicode(bool useDatabaseFirst, int numberOfElements, bool withCount)
        {
            var db = GetDb(useDatabaseFirst);
            var sequence = TestUtil.GetSequenceOfString(numberOfElements, withCount, true);
            var result = await db.AsQueryableValues(sequence, isUnicode: true).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Guid(bool useDatabaseFirst, int numberOfElements, bool withCount)
        {
            var db = GetDb(useDatabaseFirst);
            var sequence = TestUtil.GetSequenceOfGuid(numberOfElements, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }
    }
}
