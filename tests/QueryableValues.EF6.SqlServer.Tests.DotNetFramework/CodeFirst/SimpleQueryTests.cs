using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.CodeFirst
{
    public class SimpleQueryTests
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                yield return new object[] { new DatabaseFirst.TestDbContext(), false };
                yield return new object[] { new DatabaseFirst.TestDbContext(), true };
                yield return new object[] { new CodeFirst.TestDbContext(), false };
                yield return new object[] { new CodeFirst.TestDbContext(), true };
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int32Empty(DbContext db, bool withCount)
        {
            var sequence = TestUtil.GetSequenceOfInt32(0, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int32Some(DbContext db, bool withCount)
        {
            var sequence = TestUtil.GetSequenceOfInt32(0, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int32Many(DbContext db, bool withCount)
        {
            var sequence = TestUtil.GetSequenceOfInt32(0, withCount);
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }


    }
}
