namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.DatabaseFirst
{
    public partial class TestDbContext : ITestDbContext, ITestDbContextWithSauce
    {
        public TestDbContext(string connectionString) : base(connectionString)
        {
        }

        public static TestDbContext Create(bool useDatabaseNullSemantics = false)
        {
            return new TestDbContext(DbUtil.GetConnectionString(true))
            {
                Configuration =
                {
                    UseDatabaseNullSemantics = useDatabaseNullSemantics
                }
            };
        }
    }
}
