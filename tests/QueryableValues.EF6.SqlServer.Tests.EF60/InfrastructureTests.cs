using System;
using System.Linq;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public class InfrastructureTests
    {
        [Fact]
        public void DbSetIsRequired()
        {
            var db = new InvalidDbContext();

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                _ = db.AsQueryableValues(new[] { 1 }).ToList();
            });

            Assert.Contains("QueryableValues only works on a DbContext with at least one public DbSet<> or IDbSet<>.", exception.Message);
        }

        [Fact]
        public void OnlyWorksOnDbContext()
        {
            var db = new NotADbContext();

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                _ = db.AsQueryableValues(new[] { 1 }).ToList();
            });

            Assert.Contains("QueryableValues only works on a System.Data.Entity.DbContext type.", exception.Message);
        }

        class NotADbContext : IQueryableValuesEnabledDbContext
        {
        }
    }
}
