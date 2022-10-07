using System;
using System.Collections.Concurrent;
using System.Data.SqlClient;

namespace BlazarTech.QueryableValues
{
    internal static class DbUtil
    {
        private static readonly ConcurrentDictionary<string, bool> JsonSupportByConnectionString = new();

        /// <summary>
        /// Gets a value indicating if the provided <paramref name="connection"/> can make use of the <c>OPENJSON</c> function in SQL Server.<br/>
        /// <see href="https://learn.microsoft.com/en-us/sql/t-sql/functions/openjson-transact-sql"/>
        /// </summary>
        /// <remarks>
        /// The result is cached on a per connection string fashion.
        /// </remarks>
        public static bool IsJsonSupported(SqlConnection connection)
        {
#if NET452
            return JsonSupportByConnectionString.GetOrAdd(connection.ConnectionString, key => isJsonSupported(key, connection));
#else
            return JsonSupportByConnectionString.GetOrAdd(connection.ConnectionString, isJsonSupported, connection);
#endif
            static bool isJsonSupported(string key, SqlConnection connection)
            {
                try
                {
                    var mustClose = false;

                    try
                    {
                        if (connection.State == System.Data.ConnectionState.Closed)
                        {
                            connection.Open();
                            mustClose = true;
                        }

                        var majorVersionNumber = getMajorVersionNumber(connection.ServerVersion);

                        // https://learn.microsoft.com/en-us/sql/t-sql/functions/openjson-transact-sql
                        if (majorVersionNumber >= 13)
                        {
                            using var cm = new SqlCommand("SELECT [compatibility_level] FROM [sys].[databases] WHERE [database_id] = DB_ID()", connection);
                            var compatibilityLevel = Convert.ToInt32(cm.ExecuteScalar());
                            return compatibilityLevel >= 130;
                        }
                    }
                    finally
                    {
                        if (mustClose)
                        {
                            connection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.TraceError(nameof(IsJsonSupported), ex);
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
#if NET452 || NET472
                        var substring = serverVersion.Substring(0, index);
#else
                        var substring = serverVersion.AsSpan(0, index);
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
    }
}
