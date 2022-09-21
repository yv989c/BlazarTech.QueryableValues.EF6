namespace BlazarTech.QueryableValues
{
    /// <summary>
    /// QueryableValues configuration builder.
    /// </summary>
    public sealed class QueryableValuesConfigurationBuilder : IQueryableValuesConfiguration
    {
        internal QueryableValuesJsonOptions JsonOptions { get; private set; } = QueryableValuesJsonOptions.Auto;

        QueryableValuesJsonOptions IQueryableValuesConfiguration.JsonOptions => JsonOptions;

        private QueryableValuesConfigurationBuilder() { }

        internal static QueryableValuesConfigurationBuilder Create()
        {
            return new QueryableValuesConfigurationBuilder();
        }

        /// <summary>
        /// Specifies if JSON should be used instead of XML for serialization purposes.
        /// </summary>
        /// <param name="options">JSON serialization options.</param>
        /// <returns>The <see cref="QueryableValuesConfigurationBuilder"/> so that additional calls can be chained.</returns>
        /// <remarks>
        /// todo: advantages and requirements
        /// https://learn.microsoft.com/en-us/sql/t-sql/functions/openjson-transact-sql
        /// </remarks>
        public QueryableValuesConfigurationBuilder UseJson(QueryableValuesJsonOptions options = QueryableValuesJsonOptions.Auto)
        {
            JsonOptions = options;
            return this;
        }
    }

    /// <summary>
    /// JSON serialization options.
    /// </summary>
    public enum QueryableValuesJsonOptions
    {
        /// <summary>
        /// JSON serialization will be used if support is detected.
        /// This is the default.
        /// </summary>
        Auto,

        /// <summary>
        /// QueryableValues will assume that JSON serialization is supported.
        /// <b>A runtime exception will occur if this is not the case.</b>
        /// </summary>
        Always,

        /// <summary>
        /// QueryableValues will not attempt to use JSON serialization.
        /// </summary>
        Never
    }
}
