using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{

    public class JoinQueryTests
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
#if NET5_0_OR_GREATER
                yield return new object[] { new CodeFirst.TestDbContext(), false };
                yield return new object[] { new CodeFirst.TestDbContext(), true };
#else
                yield return new object[] { new CodeFirst.TestDbContext(), false };
                yield return new object[] { new CodeFirst.TestDbContext(), true };

                yield return new object[] { new DatabaseFirst.TestDbContext(), false };
                yield return new object[] { new DatabaseFirst.TestDbContext(), true };
#endif
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Int32<T>(T db, bool withCount)
            where T : DbContext, ITestDbContext
        {
            using (db)
            {
                var sequence = withCount ? getSequence().ToList() : getSequence();
                var qv = db.AsQueryableValues(sequence);
                var result = await (
                    from e in db.MyEntity
                    join v in qv on e.MyEntityID equals v
                    select e.MyEntityID
                    )
                    .ToListAsync();

                Assert.Equal(sequence, result);
            }

            IEnumerable<int> getSequence()
            {
                yield return 1;
                yield return 3;
                yield return 2;
            }
        }
    }
}
