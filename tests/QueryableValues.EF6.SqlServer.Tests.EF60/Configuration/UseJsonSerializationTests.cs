using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.Configuration
{
    [Collection("DbContext")]
    public class UseJsonSerializationTests
    {
        private readonly DbContextFixture _dbContextFixture;

        public UseJsonSerializationTests(DbContextFixture dbContextFixture)
        {
            // Ensure that QV's DbInterception gets registered first.
            _ = QueryableValuesDbContextExtensions.AsQueryableValues(dbContextFixture.CodeFirstDb, TestUtil.ArrayEmptyInt32);

            _dbContextFixture = dbContextFixture;
        }

        private async Task SequenceAssertion(ITestDbContextWithSauce testDbContext)
        {
            var sequence = new[] { 1, 2, 3 };
            var result = await testDbContext.AsQueryableValues(sequence)
                .ToListAsync();

            Assert.Equal(sequence, result);
        }

        [Fact]
        public async Task UsesXmlSerializationAuto()
        {
            var interceptor = new TestInterceptor(expectsXml: true);

            try
            {
                DbInterception.Add(interceptor);
                await SequenceAssertion(_dbContextFixture.CodeFirstDbCompatLevel120);
            }
            finally
            {
                DbInterception.Remove(interceptor);
            }
        }

        [Fact]
        public async Task UsesXmlSerializationAlways()
        {
            var interceptor = new TestInterceptor(expectsXml: true);

            try
            {
                QueryableValuesConfigurator
                    .Configure<CodeFirst.TestDbContext>()
                    .UseJsonSerialization(QueryableValuesJsonSerializationOptions.Never);

                DbInterception.Add(interceptor);
                await SequenceAssertion(_dbContextFixture.CodeFirstDb);
            }
            finally
            {
                DbInterception.Remove(interceptor);

                QueryableValuesConfigurator
                    .Configure<CodeFirst.TestDbContext>()
                    .UseJsonSerialization(QueryableValuesJsonSerializationOptions.Auto);
            }
        }

        [Fact]
        public async Task UsesJsonSerializationAuto()
        {
            var interceptor = new TestInterceptor(expectsJson: true);

            try
            {
                DbInterception.Add(interceptor);
                await SequenceAssertion(_dbContextFixture.CodeFirstDb);
            }
            finally
            {
                DbInterception.Remove(interceptor);
            }
        }

        [Fact]
        public async Task UsesJsonSerializationAlways()
        {
            var interceptor = new TestInterceptor(expectsJson: true);

            try
            {
                QueryableValuesConfigurator
                    .Configure<CodeFirst.TestDbContext>()
                    .UseJsonSerialization(QueryableValuesJsonSerializationOptions.Always);

                DbInterception.Add(interceptor);
                await SequenceAssertion(_dbContextFixture.CodeFirstDb);
            }
            finally
            {
                DbInterception.Remove(interceptor);

                QueryableValuesConfigurator
                    .Configure<CodeFirst.TestDbContext>()
                    .UseJsonSerialization(QueryableValuesJsonSerializationOptions.Auto);
            }
        }

        [Fact]
        public async Task UsesJsonSerializationAlwaysOnUnsupportedEnvironment()
        {
            var interceptor = new TestInterceptor(expectsJson: true);

            try
            {
                QueryableValuesConfigurator
                    .Configure<CodeFirst.TestDbContext>()
                    .UseJsonSerialization(QueryableValuesJsonSerializationOptions.Always);

                DbInterception.Add(interceptor);

                var entityCommandExecutionException = await Assert.ThrowsAnyAsync<EntityCommandExecutionException>(async () =>
                {
                    await SequenceAssertion(_dbContextFixture.CodeFirstDbCompatLevel120);
                });

                Assert.IsType<SqlException>(entityCommandExecutionException.InnerException);
            }
            finally
            {
                DbInterception.Remove(interceptor);

                QueryableValuesConfigurator
                    .Configure<CodeFirst.TestDbContext>()
                    .UseJsonSerialization(QueryableValuesJsonSerializationOptions.Auto);
            }
        }

        class TestInterceptor : IDbCommandInterceptor
        {
            private readonly bool _expectsXml;
            private readonly bool _expectsJson;

            public TestInterceptor(bool expectsXml = false, bool expectsJson = false)
            {
                _expectsXml = expectsXml;
                _expectsJson = expectsJson;
            }

            public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
            {
                throw new NotImplementedException();
            }

            public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
            {
                throw new NotImplementedException();
            }

            private void AssertCommandText(DbCommand command)
            {
                if (_expectsXml)
                {
                    Assert.Contains(".nodes('/R/V')", command.CommandText);
                }

                if (_expectsJson)
                {
                    Assert.Contains("OPENJSON", command.CommandText);
                }
            }

            public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
            {
                AssertCommandText(command);
            }

            public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
            {
                AssertCommandText(command);
            }

            public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
            {
                throw new NotImplementedException();
            }

            public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}
