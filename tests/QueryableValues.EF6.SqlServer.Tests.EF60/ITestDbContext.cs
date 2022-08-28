using System.Data.Entity;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public interface ITestDbContext
    {
        DbSet<DatabaseFirst.TestDataEntity> TestData { get; set; }
    }

    public interface ITestDbContextWithSauce : ITestDbContext, IQueryableValuesEnabledDbContext
    {
    }
}
