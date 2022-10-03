using System;

namespace BlazarTech.QueryableValues
{
    /// <summary>
    /// QueryableValues configuration builder.
    /// </summary>
    public sealed class QueryableValuesConfigurationBuilder : IQueryableValuesConfiguration
    {
        internal SerializationOptions SerializationOptions { get; private set; } = SerializationOptions.Auto;

        SerializationOptions IQueryableValuesConfiguration.SerializationOptions => SerializationOptions;

        private QueryableValuesConfigurationBuilder() { }

        internal static QueryableValuesConfigurationBuilder Create()
        {
            return new QueryableValuesConfigurationBuilder();
        }

        /// <summary>
        /// Configures serialization options.
        /// </summary>
        /// <param name="options">Serialization options.</param>
        /// <returns>The <see cref="QueryableValuesConfigurationBuilder"/> so that additional calls can be chained.</returns>
        public QueryableValuesConfigurationBuilder Serialization(SerializationOptions options = SerializationOptions.Auto)
        {
            if (Enum.IsDefined(typeof(SerializationOptions), options))
            {
                SerializationOptions = options;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(options));
            }

            return this;
        }
    }
}
