namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.DatabaseFirst
{
    public partial class TestDbContext : ITestDbContext
    {
        public TestDbContext(string connectionString) : base(connectionString)
        {
        }

        public static TestDbContext Create()
        {
            return new TestDbContext(DbUtil.GetConnectionString(true));
        }
    }
}
