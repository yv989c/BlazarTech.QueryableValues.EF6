using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
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

        private static readonly Regex Regex1 = new(@"'" + QueryableValuesDbContextExtensions.InternalId + /*lang=regex*/@"\-(?<F>[a-zA-Z]+)\-(?<DT>[a-zA-Z]+)'\s*=\s*@(?<V>.+?)(?:\)\s*AND\s*\(.+?\))?(?=\s*\))", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex Regex2 = new(@"SELECT\s+TOP\s+\(\s*@(?<T>.+?)\s*\)", RegexOptions.CultureInvariant | RegexOptions.RightToLeft | RegexOptions.Compiled);

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
                throw Util.NewOnlyWorksWithSqlServerException();
            }

            var entry = (InterceptedCommandData)Cache.Get(originalCommandText);

            if (entry is null)
            {
                var sb = StringBuilderPool.Shared.Get();

                try
                {
                    var parameters = new Dictionary<string, SerializationFormat>();
                    var matches = Regex1.Matches(originalCommandText);
                    var lastStartIndex = 0;

                    foreach (Match match in matches)
                    {
                        var match2 = Regex2.Match(originalCommandText, match.Index);
                        if (match2.Success)
                        {
                            var valueParameterName = match.Groups["V"].Value;
                            var dataType = match.Groups["DT"].Value;
                            var topParameterName = match2.Groups["T"].Value;

                            var serializationFormat = match.Groups["F"].Value switch
                            {
                                SerializationFormatIdentifier.Xml => SerializationFormat.Xml,
                                SerializationFormatIdentifier.Json => SerializationFormat.Json,
                                _ => throw new NotImplementedException(),
                            };

                            parameters.Add(valueParameterName, serializationFormat);

#if NET452 || NET472
                            sb.Append(originalCommandText.Substring(lastStartIndex, match2.Index - lastStartIndex));
#else
                            sb.Append(originalCommandText.AsSpan(lastStartIndex, match2.Index - lastStartIndex));
#endif
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

#if NET452 || NET472
                    sb.Append(originalCommandText.Substring(lastStartIndex));
#else
                    sb.Append(originalCommandText.AsSpan(lastStartIndex));
#endif

                    entry = new InterceptedCommandData(sb.ToString(), parameters);
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
                if (entry.Parameters.TryGetValue(parameter.ParameterName, out SerializationFormat serializationFormat))
                {
                    if (serializationFormat == SerializationFormat.Xml)
                    {
                        parameter.SqlDbType = SqlDbType.Xml;
                    }

                    // max
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
            public Dictionary<string, SerializationFormat> Parameters { get; }

            public InterceptedCommandData(string commandText, Dictionary<string, SerializationFormat> parameters)
            {
                CommandText = commandText;
                Parameters = parameters;
            }
        }
    }
}
