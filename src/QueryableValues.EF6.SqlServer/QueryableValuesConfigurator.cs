using System;
using System.Collections.Concurrent;
using System.Data.Entity;

namespace BlazarTech.QueryableValues
{
    /// <summary>
    /// Provides APIs for the configuration of QueryableValues.
    /// </summary>
    public static class QueryableValuesConfigurator
    {
        private static readonly ConcurrentDictionary<Type, QueryableValuesConfigurationBuilder> ConfigurationByDbContextType = new ConcurrentDictionary<Type, QueryableValuesConfigurationBuilder>();
        private static readonly object BuilderByDbContextTypeAddLock = new object();
        private static readonly QueryableValuesConfigurationBuilder DefaultConfigurationBuilder = QueryableValuesConfigurationBuilder.Create();

        internal static IQueryableValuesConfiguration DefaultConfiguration => DefaultConfigurationBuilder;

        private static QueryableValuesConfigurationBuilder GetOrCreateBuilder(Type dbContextType)
        {
            if (!ConfigurationByDbContextType.TryGetValue(dbContextType, out QueryableValuesConfigurationBuilder? instance))
            {
                lock (BuilderByDbContextTypeAddLock)
                {
                    if (!ConfigurationByDbContextType.TryGetValue(dbContextType, out instance))
                    {
                        instance = QueryableValuesConfigurationBuilder.Create();
                        ConfigurationByDbContextType.TryAdd(dbContextType, instance);
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Gets the default configuration builder.
        /// </summary>
        /// <returns>A <see cref="QueryableValuesConfigurationBuilder"/> that implements a fluent API.</returns>
        public static QueryableValuesConfigurationBuilder Configure()
        {
            return DefaultConfigurationBuilder;
        }

        /// <summary>
        /// Gets a configuration builder for the provided <typeparamref name="TDbContext"/>.
        /// </summary>
        /// <typeparam name="TDbContext">The <see cref="DbContext"/> to be configured.</typeparam>
        /// <returns><inheritdoc cref="Configure"/></returns>
        public static QueryableValuesConfigurationBuilder Configure<TDbContext>()
            where TDbContext : DbContext
        {
            return GetOrCreateBuilder(typeof(TDbContext));
        }

        internal static IQueryableValuesConfiguration GetConfiguration(Type dbContextType)
        {
            if (ConfigurationByDbContextType.TryGetValue(dbContextType, out QueryableValuesConfigurationBuilder? instance))
            {
                return instance;
            }
            else
            {
                return DefaultConfiguration;
            }
        }
    }
}
