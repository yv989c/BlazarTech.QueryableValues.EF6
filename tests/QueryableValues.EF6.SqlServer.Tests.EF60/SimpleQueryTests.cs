using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public class SimpleQueryTests
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                var numberOfElementsList = new[] { 0, 6, 1000 };


#if NET5_0_OR_GREATER
                foreach (var numberOfElements in numberOfElementsList)
                {
                    yield return new object[] { new CodeFirst.TestDbContext(), numberOfElements, false };
                    yield return new object[] { new CodeFirst.TestDbContext(), numberOfElements, true };
                }
#else
                foreach (var numberOfElements in numberOfElementsList)
                {
                    yield return new object[] { new CodeFirst.TestDbContext(), numberOfElements, false };
                    yield return new object[] { new CodeFirst.TestDbContext(), numberOfElements, true };
                }

                foreach (var numberOfElements in numberOfElementsList)
                {
                    yield return new object[] { new DatabaseFirst.TestDbContext(), numberOfElements, false };
                    yield return new object[] { new DatabaseFirst.TestDbContext(), numberOfElements, true };
                }
#endif
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int16(DbContext db, int numberOfElements, bool withCount)
        {
            using (db)
            {
                var sequence = TestUtil.GetSequenceOfInt16(numberOfElements, withCount);
                var result = await db.AsQueryableValues(sequence).ToListAsync();
                Assert.Equal(sequence, result);
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int32(DbContext db, int numberOfElements, bool withCount)
        {
            using (db)
            {
                var sequence = TestUtil.GetSequenceOfInt32(numberOfElements, withCount);
                var result = await db.AsQueryableValues(sequence).ToListAsync();
                Assert.Equal(sequence, result);
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int64(DbContext db, int numberOfElements, bool withCount)
        {
            using (db)
            {
                var sequence = TestUtil.GetSequenceOfInt64(numberOfElements, withCount);
                var result = await db.AsQueryableValues(sequence).ToListAsync();
                Assert.Equal(sequence, result);
            }
        }
    }
}
