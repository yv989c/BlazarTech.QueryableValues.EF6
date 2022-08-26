using System.Data.Entity;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public interface ITestDbContext
    {
        DbSet<DatabaseFirst.MyEntity> MyEntity { get; set; }
    }
}
