using System;
using System.Collections.Generic;
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
        public void Int32SequenceEmpty(bool useCount)
        {
            var sequence = TestUtil.GetInt32Sequence(0);

            if (useCount)
            {
                sequence = sequence.ToList();
            }

            using var db = new TestDbContext();
            var result = db.AsQueryableValues(sequence).ToList();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Int32SequenceFew(bool useCount)
        {
            var sequence = TestUtil.GetInt32Sequence(6);

            if (useCount)
            {
                sequence = sequence.ToList();
            }

            using var db = new TestDbContext();
            var result = db.AsQueryableValues(sequence).ToList();
            Assert.Equal(sequence, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Int32SequenceMany(bool useCount)
        {
            var sequence = TestUtil.GetInt32Sequence(1000);

            if (useCount)
            {
                sequence = sequence.ToList();
            }

            using var db = new TestDbContext();
            var result = db.AsQueryableValues(sequence).ToList();
            Assert.Equal(sequence, result);
        }
    }
}
