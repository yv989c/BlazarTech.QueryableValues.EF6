using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public interface ITestDbContext
    {
        DbSet<DatabaseFirst.TestDataEntity> TestData { get; set; }
        Task<int> SaveChangesAsync();
    }

    public interface ITestDbContextWithSauce : ITestDbContext, IQueryableValuesEnabledDbContext, IDisposable
    {
    }
}
