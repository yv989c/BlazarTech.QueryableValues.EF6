using System.Data.Entity;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public class InvalidDbContext : DbContext
    {
        public InvalidDbContext() : base(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Connection Timeout=190;Database=master;") { }
    }
}
