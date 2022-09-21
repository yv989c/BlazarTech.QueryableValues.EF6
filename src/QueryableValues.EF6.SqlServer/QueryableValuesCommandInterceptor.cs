using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace BlazarTech.QueryableValues
{
    sealed class QueryableValuesCommandInterceptor : IDbCommandInterceptor
    {
        private static readonly MemoryCache Cache = new MemoryCache(
            "BlazarTech.QueryableValues.EF6",
            new NameValueCollection
            {
                { "CacheMemoryLimitMegabytes", "10" }
            });

        private static readonly Regex Regex1 = new Regex(@"'" + QueryableValuesDbContextExtensions.InternalId + @"(?<DT>[a-z\-]{3,})'\s*=\s*@(?<V>.+?)(?:\)\s*AND\s*\(.+?\))?(?=\s*\))", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex Regex2 = new Regex(@"SELECT\s+TOP\s+\(\s*@(?<T>.+?)\s*\)", RegexOptions.CultureInvariant | RegexOptions.RightToLeft | RegexOptions.Compiled);

        private static readonly ConcurrentDictionary<string, bool> JsonSupportByDb = new ConcurrentDictionary<string, bool>();

        private static bool IsJsonSupported(SqlConnection connection)
        {
#if NET472_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
            return JsonSupportByDb.GetOrAdd(connection.ConnectionString, isJsonSupported, connection);
#else
            return JsonSupportByDb.GetOrAdd(connection.ConnectionString, key => isJsonSupported(key, connection));
#endif

            static bool isJsonSupported(string key, SqlConnection connection)
            {
                var majorVersionNumber = getMajorVersionNumber(connection.ServerVersion);

                // https://learn.microsoft.com/en-us/sql/t-sql/functions/openjson-transact-sql
                if (majorVersionNumber >= 13)
                {
                    try
                    {
                        using var cm = new SqlCommand("SELECT [compatibility_level] FROM [sys].[databases] WHERE [database_id] = DB_ID()", connection);
                        var compatibilityLevel = Convert.ToInt32(cm.ExecuteScalar());
                        return compatibilityLevel >= 130;
                    }
                    catch (Exception ex)
                    {
                        Util.TraceError(nameof(IsJsonSupported), ex);
                    }
                }

                return false;
            }

            static int getMajorVersionNumber(string? serverVersion)
            {
                if (serverVersion != null)
                {
                    var index = serverVersion.IndexOf('.');

                    if (index >= 0)
                    {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                        var substring = serverVersion.AsSpan()[..index];
#else
                        var substring = serverVersion.Substring(0, index);
#endif
                        if (int.TryParse(substring, out int majorVersionNumber))
                        {
                            return majorVersionNumber;
                        }
                    }
                }

                return 0;
            }
        }

        private static IQueryableValuesConfiguration GetConfiguration(DbCommandInterceptionContext interceptionContext)
        {
            if (interceptionContext.DbContexts.FirstOrDefault()?.GetType() is Type dbContextType)
            {
                return QueryableValuesConfigurator.GetConfiguration(dbContextType);
            }
            else
            {
                return QueryableValuesConfigurator.DefaultConfiguration;
            }
        }

        private static void TransformCommand(DbCommand command, DbCommandInterceptionContext interceptionContext)
        {
            var originalCommandText = command.CommandText;

            if (originalCommandText is null || !originalCommandText.Contains(QueryableValuesDbContextExtensions.InternalId))
            {
                return;
            }

            if (!(command is SqlCommand sqlCommand))
            {
                throw new InvalidOperationException("QueryableValues only works with a SQL Server provider.");
            }

            var configuration = GetConfiguration(interceptionContext);

            var useJson = 
                (configuration.JsonOptions == QueryableValuesJsonOptions.Auto && IsJsonSupported(sqlCommand.Connection)) ||
                configuration.JsonOptions == QueryableValuesJsonOptions.Always;

            var entry = (InterceptedCommandData)Cache.Get(originalCommandText);

            if (entry is null)
            {
                var sb = StringBuilderPool.Shared.Get();

                try
                {
                    var xmlParameterNames = new HashSet<string>();
                    var matches = Regex1.Matches(originalCommandText);
                    var lastStartIndex = 0;

                    foreach (Match match in matches)
                    {
                        var match2 = Regex2.Match(originalCommandText, match.Index);
                        if (match2.Success)
                        {
                            var topParameterName = match2.Groups["T"].Value;

                            var valueParameterName = match.Groups["V"].Value;

                            xmlParameterNames.Add(valueParameterName);

                            sb.Append(originalCommandText.Substring(lastStartIndex, match2.Index - lastStartIndex));
                            sb.Append("SELECT ");
                            sb.Append("TOP (@").Append(topParameterName).Append(") ");
                            sb.Append("I.value('. cast as xs:");

                            var dataType = match.Groups["DT"].Value;
                            string xmlType, sqlType;

                            switch (dataType)
                            {
                                case QueryTypeIdentifier.Byte:
                                    xmlType = "unsignedByte";
                                    sqlType = "tinyint";
                                    break;
                                case QueryTypeIdentifier.Short:
                                    xmlType = "short";
                                    sqlType = "smallint";
                                    break;
                                case QueryTypeIdentifier.Int:
                                    xmlType = "integer";
                                    sqlType = "int";
                                    break;
                                case QueryTypeIdentifier.Long:
                                    xmlType = "integer";
                                    sqlType = "bigint";
                                    break;
                                case QueryTypeIdentifier.String:
                                    xmlType = "string";
                                    sqlType = "varchar(max)";
                                    break;
                                case QueryTypeIdentifier.StringUnicode:
                                    xmlType = "string";
                                    sqlType = "nvarchar(max)";
                                    break;
                                case QueryTypeIdentifier.Guid:
                                    xmlType = "string";
                                    sqlType = "uniqueidentifier";
                                    break;
                                default:
                                    throw new NotImplementedException(dataType);
                            }

                            sb.Append(xmlType).Append("?', '").Append(sqlType);
                            sb.Append("') AS [C1] FROM @").Append(valueParameterName).Append(".nodes('/R/V') N(I)");

                            lastStartIndex = match.Index + match.Length;
                        }
                    }

                    sb.Append(originalCommandText.Substring(lastStartIndex));

                    entry = new InterceptedCommandData(sb.ToString(), xmlParameterNames);
                }
                finally
                {
                    StringBuilderPool.Shared.Return(sb);
                }

                var cachePolicy = new CacheItemPolicy
                {
                    SlidingExpiration = TimeSpan.FromHours(12)
                };

                Cache.Add(originalCommandText, entry, cachePolicy);
            }

            foreach (SqlParameter parameter in sqlCommand.Parameters)
            {
                if (entry.XmlParameterNames.Contains(parameter.ParameterName))
                {
                    parameter.SqlDbType = System.Data.SqlDbType.Xml;
                    parameter.Size = -1;
                }
            }

            sqlCommand.CommandText = entry.CommandText;
        }

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            TransformCommand(command, interceptionContext);
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            TransformCommand(command, interceptionContext);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            TransformCommand(command, interceptionContext);
        }

        private class InterceptedCommandData
        {
            public string CommandText { get; }
            public HashSet<string> XmlParameterNames { get; }

            public InterceptedCommandData(string commandText, HashSet<string> xmlParameterNames)
            {
                CommandText = commandText;
                XmlParameterNames = xmlParameterNames;
            }
        }
    }
}
