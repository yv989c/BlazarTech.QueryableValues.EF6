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
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Int32Empty(bool withCount)
        {
            var sequence = TestUtil.GetSequenceOfInt32(0, withCount);
            using var db = new TestDbContext();
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Int32Some(bool withCount)
        {
            var sequence = TestUtil.GetSequenceOfInt32(0, withCount);
            using var db = new TestDbContext();
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Int32Many(bool withCount)
        {
            var sequence = TestUtil.GetSequenceOfInt32(0, withCount);
            using var db = new TestDbContext();
            var result = await db.AsQueryableValues(sequence).ToListAsync();
            Assert.Equal(sequence, result);
        }


    }
}
