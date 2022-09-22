using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazarTech.QueryableValues
{
    sealed class QueryableValuesCommandInterceptor : IDbCommandInterceptor
    {
        private static readonly MemoryCache Cache = new(
            "BlazarTech.QueryableValues.EF6",
            new NameValueCollection
            {
                { "CacheMemoryLimitMegabytes", "10" }
            });

        private static readonly Regex Regex1 = new(@"'" + QueryableValuesDbContextExtensions.InternalId + @"(?<DT>[a-z\-]{3,})'\s*=\s*@(?<V>.+?)(?:\)\s*AND\s*\(.+?\))?(?=\s*\))", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex Regex2 = new(@"SELECT\s+TOP\s+\(\s*@(?<T>.+?)\s*\)", RegexOptions.CultureInvariant | RegexOptions.RightToLeft | RegexOptions.Compiled);

        private static readonly ConcurrentDictionary<string, bool> JsonSupportByDb = new();

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

        private static void BuildSqlQueryFragmentForXml(StringBuilder sb, string dataType, string valueParameterName)
        {
            sb.Append("I.value('. cast as xs:");

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
        }

        private static void BuildSqlQueryFragmentForJson(StringBuilder sb, string dataType, string valueParameterName)
        {
            sb.Append("[C1] FROM OPENJSON(@").Append(valueParameterName).Append(") WITH ([C1] ");

            var sqlType = dataType switch
            {
                QueryTypeIdentifier.Byte => "tinyint",
                QueryTypeIdentifier.Short => "smallint",
                QueryTypeIdentifier.Int => "int",
                QueryTypeIdentifier.Long => "bigint",
                QueryTypeIdentifier.String => "varchar(max)",
                QueryTypeIdentifier.StringUnicode => "nvarchar(max)",
                QueryTypeIdentifier.Guid => "uniqueidentifier",
                _ => throw new NotImplementedException(dataType),
            };

            sb.Append(sqlType).Append(" '$')");
        }

        private static void TransformCommand(DbCommand command, DbCommandInterceptionContext interceptionContext)
        {
            var originalCommandText = command.CommandText;

            if (originalCommandText is null || !originalCommandText.Contains(QueryableValuesDbContextExtensions.InternalId))
            {
                return;
            }

            if (command is not SqlCommand sqlCommand)
            {
                throw new InvalidOperationException("QueryableValues only works with a SQL Server provider.");
            }

            var entry = (InterceptedCommandData)Cache.Get(originalCommandText);

            if (entry is null)
            {
                // todo:
                // - The first execution will be used to detect if json is supported and will use XML
                // - Next execution will use Json if it's supported (MUST drop previosuly cached SQL and recreate for Json)
                // - Get Json supported flag per connection string (bool? IsJsonSupported(string))
                // - Use the above to chose the appropiate serializer for the values in QueryableValuesDbContextExtensions

                //var configuration = GetConfiguration(interceptionContext);
                var useJson = false;
                //(configuration.JsonOptions == QueryableValuesJsonOptions.Auto && IsJsonSupported(sqlCommand.Connection)) ||
                //configuration.JsonOptions == QueryableValuesJsonOptions.Always;

                var serializationFormat = useJson ? SerializationFormat.Json : SerializationFormat.Xml;
                var sb = StringBuilderPool.Shared.Get();

                try
                {
                    var parameterNames = new HashSet<string>();
                    var matches = Regex1.Matches(originalCommandText);
                    var lastStartIndex = 0;

                    foreach (Match match in matches)
                    {
                        var match2 = Regex2.Match(originalCommandText, match.Index);
                        if (match2.Success)
                        {
                            var topParameterName = match2.Groups["T"].Value;
                            var valueParameterName = match.Groups["V"].Value;
                            var dataType = match.Groups["DT"].Value;

                            parameterNames.Add(valueParameterName);

                            sb.Append(originalCommandText.Substring(lastStartIndex, match2.Index - lastStartIndex));
                            sb.Append("SELECT ");
                            sb.Append("TOP (@").Append(topParameterName).Append(") ");

                            switch (serializationFormat)
                            {
                                case SerializationFormat.Xml:
                                    BuildSqlQueryFragmentForXml(sb, dataType, valueParameterName);
                                    break;
                                case SerializationFormat.Json:
                                    BuildSqlQueryFragmentForJson(sb, dataType, valueParameterName);
                                    break;
                                default:
                                    throw new NotImplementedException(nameof(serializationFormat));
                            }

                            lastStartIndex = match.Index + match.Length;
                        }
                    }

                    sb.Append(originalCommandText.Substring(lastStartIndex));

                    entry = new InterceptedCommandData(sb.ToString(), parameterNames, serializationFormat);
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
                if (entry.ParameterNames.Contains(parameter.ParameterName))
                {
                    switch (entry.SerializationFormat)
                    {
                        case SerializationFormat.Xml:
                            parameter.SqlDbType = System.Data.SqlDbType.Xml;
                            parameter.Size = -1;
                            break;
                        case SerializationFormat.Json:
                            // todo
                            break;
                        default:
                            throw new NotImplementedException();
                    }
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
            public HashSet<string> ParameterNames { get; }
            public SerializationFormat SerializationFormat { get; }

            public InterceptedCommandData(string commandText, HashSet<string> parameterNames, SerializationFormat serializationFormat)
            {
                CommandText = commandText;
                ParameterNames = parameterNames;
                SerializationFormat = serializationFormat;
            }
        }

        private enum SerializationFormat
        {
            Xml,
            Json
        }
    }
}
