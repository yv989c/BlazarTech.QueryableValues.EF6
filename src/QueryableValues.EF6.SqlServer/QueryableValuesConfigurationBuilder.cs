namespace BlazarTech.QueryableValues
{
    /// <summary>
    /// QueryableValues configuration builder.
    /// </summary>
    public sealed class QueryableValuesConfigurationBuilder : IQueryableValuesConfiguration
    {
        internal QueryableValuesJsonSerializationOptions JsonSerializationOptions { get; private set; } = QueryableValuesJsonSerializationOptions.Auto;

        QueryableValuesJsonSerializationOptions IQueryableValuesConfiguration.JsonSerializationOptions => JsonSerializationOptions;

        private QueryableValuesConfigurationBuilder() { }

        internal static QueryableValuesConfigurationBuilder Create()
        {
            return new QueryableValuesConfigurationBuilder();
        }

        /// <summary>
        /// Specifies if JSON serialization should be used instead of XML.
        /// </summary>
        /// <param name="options">JSON serialization options.</param>
        /// <returns>The <see cref="QueryableValuesConfigurationBuilder"/> so that additional calls can be chained.</returns>
        /// <remarks>
        /// In my tests, JSON significantly outperforms XML.
        /// <para>
        /// JSON can only be used when the following is true:<br/>
        /// - The SQL Server instance is 2016 and above.<br/>
        /// - The database has its compatibility level 130 or higher.<br/>
        /// </para>
        /// <br/>
        /// When JSON is used, QueryableValues switches from the <see href="https://learn.microsoft.com/en-us/sql/t-sql/xml/nodes-method-xml-data-type"><c>nodes</c></see> XML method to the <see href="https://learn.microsoft.com/en-us/sql/t-sql/functions/openjson-transact-sql"><c>OPENJSON</c></see> table-valued function.
        /// </remarks>
        public QueryableValuesConfigurationBuilder UseJsonSerialization(QueryableValuesJsonSerializationOptions options = QueryableValuesJsonSerializationOptions.Auto)
        {
            JsonSerializationOptions = options;
            return this;
        }
    }

    /// <summary>
    /// JSON serialization options.
    /// </summary>
    public enum QueryableValuesJsonSerializationOptions
    {
        /// <summary>
        /// JSON serialization will be used if support is detected.
        /// This is the default.
        /// </summary>
        /// <remarks>
        /// This option causes an additional roundtrip to the database to check if JSON serialization is supported. This only happens once per connection string uniqueness.
        /// </remarks>
        Auto,

        /// <summary>
        /// Always use JSON serialization.
        /// </summary>
        /// <remarks>
        /// <b>WARNING:</b> An error will occur at runtime if JSON serialization is not supported.
        /// </remarks>
        Always,

        /// <summary>
        /// Do not use JSON serialization.
        /// </summary>
        Never
    }
}
