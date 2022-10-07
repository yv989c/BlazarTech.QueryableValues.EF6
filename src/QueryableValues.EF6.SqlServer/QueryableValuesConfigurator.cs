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
        private static readonly ConcurrentDictionary<Type, QueryableValuesConfigurationBuilder> ConfigurationByDbContextType = new();
        private static readonly object BuilderByDbContextTypeAddLock = new();

        private static QueryableValuesConfigurationBuilder _defaultConfigurationBuilder = QueryableValuesConfigurationBuilder.Create();

        internal static IQueryableValuesConfiguration DefaultConfiguration => _defaultConfigurationBuilder;

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
        /// <remarks>
        /// This configuration applies to any <see cref="DbContext"/> that doesn't have an explicit configuration via <see cref="Configure{TDbContext}"/>.
        /// </remarks>
        /// <returns>A <see cref="QueryableValuesConfigurationBuilder"/> that implements a fluent API.</returns>
        public static QueryableValuesConfigurationBuilder Configure()
        {
            return _defaultConfigurationBuilder;
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

        internal static void Reset()
        {
            _defaultConfigurationBuilder = QueryableValuesConfigurationBuilder.Create();
            ConfigurationByDbContextType.Clear();
        }
    }
}
