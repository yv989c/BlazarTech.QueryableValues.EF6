using System.IO;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Benchmarks;

internal static class DbUtil
{
    public static string GetConnectionString(bool useDatabaseFirst, string databaseNameSuffix = "Tests")
    {
        var databaseName = $"QueryableValues.EF6.{databaseNameSuffix}";
        var databaseFilePath = Path.Combine(Path.GetTempPath(), $"{databaseName}.mdf");
        var connectionString = @$"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Connection Timeout=190;Database={databaseName};AttachDbFileName={databaseFilePath}";

        if (useDatabaseFirst)
        {
            return $"metadata=res://*/DatabaseFirst.TestModels.csdl|res://*/DatabaseFirst.TestModels.ssdl|res://*/DatabaseFirst.TestModels.msl;provider=System.Data.SqlClient;provider connection string=\"{connectionString}\"";
        }
        else
        {
            return connectionString;
        }
    }

    public static string GetConnectionStringMasterDb()
    {
        return @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Connection Timeout=190;Database=master";
    }
}
