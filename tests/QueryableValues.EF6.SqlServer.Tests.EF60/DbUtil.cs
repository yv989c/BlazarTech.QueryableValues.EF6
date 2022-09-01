using System.IO;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    internal static class DbUtil
    {
        public static string GetConnectionString(bool useDatabaseFirst)
        {
            var databaseFilePath = Path.Combine(Path.GetTempPath(), $"QueryableValues.EF6.Tests.mdf");
            var connectionString = @$"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Connection Timeout=190;Database=QueryableValues.EF6.Tests;AttachDbFileName={databaseFilePath}";

            if (useDatabaseFirst)
            {
                return $"metadata=res://*/DatabaseFirst.TestModels.csdl|res://*/DatabaseFirst.TestModels.ssdl|res://*/DatabaseFirst.TestModels.msl;provider=System.Data.SqlClient;provider connection string=\"{connectionString}\"";
            }
            else
            {
                return connectionString;
            }
        }

        public static ITestDbContextWithSauce CreateDbContext(bool useDatabaseFirst, bool useDatabaseNullSemantics)
        {
            var dbContext = useDatabaseFirst ?
                (ITestDbContextWithSauce)DatabaseFirst.TestDbContext.Create(useDatabaseNullSemantics) :
                CodeFirst.TestDbContext.Create(useDatabaseNullSemantics);

            return dbContext;
        }
    }
}
