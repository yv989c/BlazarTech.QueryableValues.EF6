﻿using BlazarTech.QueryableValues.EF6.SqlServer.Tests.CodeFirst;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
{
    public sealed class DbContextFixture : IDisposable, IAsyncLifetime
    {
        public TestDbContext CodeFirstDb { get; }
        public DatabaseFirst.TestDbContext DatabaseFirstDb { get; }
        public TestDbContext CodeFirstDbCompatLevel120 { get; }

        public DbContextFixture()
        {
            CodeFirstDb = TestDbContext.Create();
            DatabaseFirstDb = DatabaseFirst.TestDbContext.Create();
            CodeFirstDbCompatLevel120 = TestDbContext.Create(useCompat120: true);
        }

        public void Dispose()
        {
            CodeFirstDb.Dispose();
            DatabaseFirstDb.Dispose();
            CodeFirstDbCompatLevel120.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task InitializeAsync()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine("Creating DB...");
            CodeFirstDb.Database.Delete();
            CodeFirstDb.Database.Create();
            Console.WriteLine($"DB Created ({sw.ElapsedMilliseconds}ms)");

            sw.Restart();

            Console.WriteLine("Seeding DB...");
            await Seed(CodeFirstDb);
            Console.WriteLine($"DB Seeded ({sw.ElapsedMilliseconds}ms)");

            sw.Restart();

            {
                Console.WriteLine("Creating CompatLevel120 DB...");
                var wasCreated = CodeFirstDbCompatLevel120.Database.CreateIfNotExists();

                if (wasCreated)
                {
                    var databaseName = CodeFirstDbCompatLevel120.Database.Connection.Database;
                    var cm = CodeFirstDbCompatLevel120.Database.Connection.CreateCommand();
                    cm.CommandText = $"ALTER DATABASE [{databaseName}] SET COMPATIBILITY_LEVEL = 120";

                    try
                    {
                        await cm.Connection!.OpenAsync();
                        await cm.ExecuteNonQueryAsync();
                    }
                    finally
                    {
                        cm.Connection!.Close();
                    }

                    Console.WriteLine($"CompatLevel120 DB Created ({sw.ElapsedMilliseconds}ms)");

                    Console.WriteLine("Seeding CompatLevel120 DB...");
                    await Seed(CodeFirstDbCompatLevel120);
                    Console.WriteLine($"CompatLevel120 DB Seeded ({sw.ElapsedMilliseconds}ms)");
                }
                else
                {
                    Console.WriteLine($"CompatLevel120 DB Created Already ({sw.ElapsedMilliseconds}ms)");
                }
            }
        }

        private static async Task Seed(ITestDbContext testDbContext)
        {
            var dateTimeOffset = new DateTimeOffset(1999, 12, 31, 23, 59, 59, 0, TimeSpan.FromHours(5));

            var data = new[]
            {
                // 1
                new DatabaseFirst.TestDataEntity
                {
                    BoolValue = false,
                    ByteValue = byte.MinValue,
                    Int16Value = short.MinValue,
                    Int32Value = int.MinValue,
                    Int64Value = long.MinValue,
                    DecimalValue = -1234567.890123M,
                    SingleValue = -1234567.890123F,
                    DoubleValue = -1234567.890123D,
                    //CharValue = 'A',
                    //CharUnicodeValue = '\u2603',
                    StringValue = "Hola!",
                    StringUnicodeValue = "👋",
                    DateTimeValue = DateTime.MinValue,
                    DateTimeOffsetValue = DateTimeOffset.MinValue,
                    GuidValue = Guid.Empty
                },
                // 2
                new DatabaseFirst.TestDataEntity
                {
                    Int32Value = 0,
                    Int64Value = 0,
                    DecimalValue = 0,
                    SingleValue = 12345.67F,
                    DoubleValue = 0,
                    StringValue = "Hallo!",
                    StringUnicodeValue = "你好！",
                    DateTimeValue = dateTimeOffset.DateTime,
                    DateTimeOffsetValue = dateTimeOffset,
                    GuidValue = new Guid("df2c9bfe-9d83-4331-97ce-2876d5dc6576")
                },
                // 3
                new DatabaseFirst.TestDataEntity
                {
                    BoolValue = true,
                    ByteValue = byte.MaxValue,
                    Int16Value = short.MaxValue,
                    Int32Value = int.MaxValue,
                    Int64Value = long.MaxValue,
                    DecimalValue = 1234567.890123M,
                    SingleValue = 1234567.890123F,
                    DoubleValue = 1234567.890123D,
                    //CharValue = 'c',
                    //CharUnicodeValue = '\u2622',
                    StringValue = "Hi!",
                    StringUnicodeValue = "أهلا",
                    DateTimeValue = DateTime.MaxValue,
                    DateTimeOffsetValue = DateTimeOffset.MaxValue,
                    GuidValue = new Guid("f6379213-750f-42df-91b9-73756f28c4b6")
                },
                // 4
                new DatabaseFirst.TestDataEntity
                {
                    BoolValue = true,
                    ByteValue = 123,
                    Int16Value = 123,
                    Int32Value = 123,
                    Int64Value = 123,
                    DecimalValue = 123M,
                    SingleValue = 123F,
                    DoubleValue = 123D,
                    //CharValue = 'c',
                    //CharUnicodeValue = '\u2622',
                    StringValue = "Olá!",
                    StringUnicodeValue = "أهلا",
                    DateTimeValue = new DateTime(2000, 1, 1),
                    DateTimeOffsetValue = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    GuidValue = new Guid("4ec4f690-a13c-4669-b622-351b3e568e68")
                }
            };

            testDbContext.TestData.AddRange(data);

            await testDbContext.SaveChangesAsync();
        }

        public Task DisposeAsync()
        {
#if NET452
            return Task.Delay(0);
#else
            return Task.CompletedTask;
#endif
        }
    }

    [CollectionDefinition("DbContext", DisableParallelization = true)]
    public class DbContextCollection : ICollectionFixture<DbContextFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
