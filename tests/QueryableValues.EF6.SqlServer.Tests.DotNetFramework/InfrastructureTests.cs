using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            Assert.Contains("QueryableValues only works on a DbContext with at least one public DbSet<>.", exception.Message);
        }
    }
}
