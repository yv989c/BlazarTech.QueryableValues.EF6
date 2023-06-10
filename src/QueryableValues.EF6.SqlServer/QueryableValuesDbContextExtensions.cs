using BlazarTech.QueryableValues.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace BlazarTech.QueryableValues
{
    /// <summary>
    /// Extension methods provided by QueryableValues on the <see cref="DbContext"/> class.
    /// </summary>
    /// <remarks>
    /// Due to technical limitations the <see cref="IEnumerable{T}"/> provided to the <c>AsQueryableValues</c> methods in this class will be enumerated immediately.
    /// This is not the case on QueryableValues for EF Core, which enumerates only when the query is materialized.
    /// </remarks>
    public static class QueryableValuesDbContextExtensions
    {
        internal const string InternalId = "⚡-jDDd5B3uLYjJD9OnH1iEKtiHcaIcgo8VxoMN4vri0Rk";

        private const string SerializationFormatXml = "-" + SerializationFormatIdentifier.Xml + "-";
        private const string SerializationFormatJson = "-" + SerializationFormatIdentifier.Json + "-";

        private static readonly ConcurrentDictionary<Type, Type> DbSetByDbContext = new();

        private static readonly Func<Type, Type> DbSetByDbContextFactory = dbContextType =>
        {
            var entityType = dbContextType
                .GetProperties()
                .Select(i => i.PropertyType)
                .Where(i => i.IsGenericType && isDbSet(i.GetGenericTypeDefinition()))
                .Select(i => i.GenericTypeArguments[0])
                .FirstOrDefault();

            if (entityType is null)
            {
                throw new InvalidOperationException("QueryableValues only works on a DbContext with at least one public DbSet<> or IDbSet<>.");
            }

            return entityType;

            static bool isDbSet(Type type)
            {
                return type == typeof(DbSet<>) || type == typeof(IDbSet<>);
            }
        };

        private static readonly ISerializer XmlSerializer = new XmlSerializer();
        private static readonly ISerializer JsonSerializer = new JsonSerializer();

        private static readonly Func<IQueryable<object>, string, SerializationFormat, IQueryable<byte>> InnerQueryByte = (dbSet, serializedValues, serializationFormat) =>
        {
            var queryable = serializationFormat switch
            {
                SerializationFormat.Xml => dbSet.Where(i => InternalId + SerializationFormatXml + QueryTypeIdentifier.Byte == serializedValues),
                SerializationFormat.Json => dbSet.Where(i => InternalId + SerializationFormatJson + QueryTypeIdentifier.Byte == serializedValues),
                _ => throw new NotImplementedException(),
            };

            return queryable.Select(i => (byte)1);
        };

        private static readonly Func<IQueryable<object>, string, SerializationFormat, IQueryable<short>> InnerQueryShort = (dbSet, serializedValues, serializationFormat) =>
        {
            var queryable = serializationFormat switch
            {
                SerializationFormat.Xml => dbSet.Where(i => InternalId + SerializationFormatXml + QueryTypeIdentifier.Short == serializedValues),
                SerializationFormat.Json => dbSet.Where(i => InternalId + SerializationFormatJson + QueryTypeIdentifier.Short == serializedValues),
                _ => throw new NotImplementedException(),
            };

            return queryable.Select(i => (short)1);
        };


        private static readonly Func<IQueryable<object>, string, SerializationFormat, IQueryable<int>> InnerQueryInt = (dbSet, serializedValues, serializationFormat) =>
        {
            var queryable = serializationFormat switch
            {
                SerializationFormat.Xml => dbSet.Where(i => InternalId + SerializationFormatXml + QueryTypeIdentifier.Int == serializedValues),
                SerializationFormat.Json => dbSet.Where(i => InternalId + SerializationFormatJson + QueryTypeIdentifier.Int == serializedValues),
                _ => throw new NotImplementedException(),
            };

            return queryable.Select(i => 1);
        };

        private static readonly Func<IQueryable<object>, string, SerializationFormat, IQueryable<long>> InnerQueryLong = (dbSet, serializedValues, serializationFormat) =>
        {
            var queryable = serializationFormat switch
            {
                SerializationFormat.Xml => dbSet.Where(i => InternalId + SerializationFormatXml + QueryTypeIdentifier.Long == serializedValues),
                SerializationFormat.Json => dbSet.Where(i => InternalId + SerializationFormatJson + QueryTypeIdentifier.Long == serializedValues),
                _ => throw new NotImplementedException(),
            };

            return queryable.Select(i => 1L);
        };

        private static readonly Func<IQueryable<object>, string, SerializationFormat, IQueryable<string>> InnerQueryString = (dbSet, serializedValues, serializationFormat) =>
        {
            var queryable = serializationFormat switch
            {
                SerializationFormat.Xml => dbSet.Where(i => InternalId + SerializationFormatXml + QueryTypeIdentifier.String == serializedValues),
                SerializationFormat.Json => dbSet.Where(i => InternalId + SerializationFormatJson + QueryTypeIdentifier.String == serializedValues),
                _ => throw new NotImplementedException(),
            };

            return queryable.Select(i => "");
        };

        private static readonly Func<IQueryable<object>, string, SerializationFormat, IQueryable<string>> InnerQueryStringUnicode = (dbSet, serializedValues, serializationFormat) =>
        {
            var queryable = serializationFormat switch
            {
                SerializationFormat.Xml => dbSet.Where(i => InternalId + SerializationFormatXml + QueryTypeIdentifier.StringUnicode == serializedValues),
                SerializationFormat.Json => dbSet.Where(i => InternalId + SerializationFormatJson + QueryTypeIdentifier.StringUnicode == serializedValues),
                _ => throw new NotImplementedException(),
            };

            return queryable.Select(i => "");
        };

        private static readonly Func<IQueryable<object>, string, SerializationFormat, IQueryable<Guid>> InnerQueryGuid = (dbSet, serializedValues, serializationFormat) =>
        {
            var queryable = serializationFormat switch
            {
                SerializationFormat.Xml => dbSet.Where(i => InternalId + SerializationFormatXml + QueryTypeIdentifier.Guid == serializedValues),
                SerializationFormat.Json => dbSet.Where(i => InternalId + SerializationFormatJson + QueryTypeIdentifier.Guid == serializedValues),
                _ => throw new NotImplementedException(),
            };

            return queryable.Select(i => ((Guid?)null!).Value);
        };

        static QueryableValuesDbContextExtensions()
        {
            DbInterception.Add(new QueryableValuesCommandInterceptor());
        }

        private static IQueryable<T> AsQueryableValues<T>(
            DbContext dbContext,
            string serializedValues,
            SerializationFormat serializationFormat,
            int valuesCount,
            Func<IQueryable<object>, string, SerializationFormat, IQueryable<T>> getInnerQuery)
        {
            var entityType = DbSetByDbContext.GetOrAdd(dbContext.GetType(), DbSetByDbContextFactory);
            var dbSet = (IQueryable<object>)dbContext.Set(entityType);
            var innerQuery = getInnerQuery(dbSet, serializedValues, serializationFormat);
            var query = innerQuery.Take(() => valuesCount);
            return query;
        }

        private static int GetCount<T>(IEnumerable<T> values)
        {
            if (values.TryGetNonEnumeratedCount(out int count))
            {
                return count;
            }
            else
            {
                return int.MaxValue;
            }
        }

        private static void ThrowIfNull(object @object, string name)
        {
            if (@object is null)
            {
                throw new ArgumentNullException(name);
            }
        }

        private static ISerializer GetSerializer(DbContext dbContext)
        {
            var configuration = QueryableValuesConfigurator.GetConfiguration(dbContext.GetType());
            var useJson =
                configuration.SerializationOptions == SerializationOptions.UseJson ||
                (configuration.SerializationOptions == SerializationOptions.Auto && DbUtil.IsJsonSupported(dbContext.Database.Connection));

            if (useJson)
            {
                return JsonSerializer;
            }
            else
            {
                return XmlSerializer;
            }
        }

        /// <summary>
        /// Allows an <see cref="IEnumerable{Byte}">IEnumerable&lt;byte&gt;</see> to be composed in an Entity Framework query.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/> owning the query.</param>
        /// <param name="values">The sequence of values to compose.</param>
        /// <returns>An <see cref="IQueryable{Byte}">IQueryable&lt;byte&gt;</see> that can be composed with other entities in the query.</returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<byte> AsQueryableValues(this DbContext dbContext, IEnumerable<byte> values)
        {
            ThrowIfNull(dbContext, nameof(dbContext));
            ThrowIfNull(values, nameof(values));

            var serializer = GetSerializer(dbContext);

            return AsQueryableValues(dbContext, serializer.Serialize(values), serializer.Format, GetCount(values), InnerQueryByte);
        }

        /// <summary>
        /// Allows an <see cref="IEnumerable{Int16}">IEnumerable&lt;short&gt;</see> to be composed in an Entity Framework query.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/> owning the query.</param>
        /// <param name="values">The sequence of values to compose.</param>
        /// <returns>An <see cref="IQueryable{Int16}">IQueryable&lt;short&gt;</see> that can be composed with other entities in the query.</returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<short> AsQueryableValues(this DbContext dbContext, IEnumerable<short> values)
        {
            ThrowIfNull(dbContext, nameof(dbContext));
            ThrowIfNull(values, nameof(values));

            var serializer = GetSerializer(dbContext);

            return AsQueryableValues(dbContext, serializer.Serialize(values), serializer.Format, GetCount(values), InnerQueryShort);
        }

        /// <summary>
        /// Allows an <see cref="IEnumerable{Int32}">IEnumerable&lt;int&gt;</see> to be composed in an Entity Framework query.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/> owning the query.</param>
        /// <param name="values">The sequence of values to compose.</param>
        /// <returns>An <see cref="IQueryable{Int32}">IQueryable&lt;int&gt;</see> that can be composed with other entities in the query.</returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<int> AsQueryableValues(this DbContext dbContext, IEnumerable<int> values)
        {
            ThrowIfNull(dbContext, nameof(dbContext));
            ThrowIfNull(values, nameof(values));

            var serializer = GetSerializer(dbContext);

            return AsQueryableValues(dbContext, serializer.Serialize(values), serializer.Format, GetCount(values), InnerQueryInt);
        }

        /// <summary>
        /// Allows an <see cref="IEnumerable{Int64}">IEnumerable&lt;long&gt;</see> to be composed in an Entity Framework query.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/> owning the query.</param>
        /// <param name="values">The sequence of values to compose.</param>
        /// <returns>An <see cref="IQueryable{Int64}">IQueryable&lt;long&gt;</see> that can be composed with other entities in the query.</returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<long> AsQueryableValues(this DbContext dbContext, IEnumerable<long> values)
        {
            ThrowIfNull(dbContext, nameof(dbContext));
            ThrowIfNull(values, nameof(values));

            var serializer = GetSerializer(dbContext);

            return AsQueryableValues(dbContext, serializer.Serialize(values), serializer.Format, GetCount(values), InnerQueryLong);
        }

        /// <summary>
        /// Allows an <see cref="IEnumerable{String}">IEnumerable&lt;string&gt;</see> to be composed in an Entity Framework query.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/> owning the query.</param>
        /// <param name="values">The sequence of values to compose.</param>
        /// <param name="isUnicode">If <see langword="true"/>, will cast the <paramref name="values"/> as <c>nvarchar</c>, otherwise, <c>varchar</c>.</param>
        /// <returns>An <see cref="IQueryable{String}">IQueryable&lt;string&gt;</see> that can be composed with other entities in the query.</returns>
        /// <remarks>
        /// About Performance: If the result is going to be composed against the property of an entity that uses 
        /// unicode (<c>nvarchar</c>), then <paramref name="isUnicode"/> should be <see langword="true"/>.
        /// Failing to do this may force SQL Server's query engine to do an implicit casting, which results 
        /// in a scan instead of an index seek (assuming there's a covering index).
        /// <para><inheritdoc cref="QueryableValuesDbContextExtensions" path="/remarks"/></para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<string> AsQueryableValues(this DbContext dbContext, IEnumerable<string> values, bool isUnicode = false)
        {
            ThrowIfNull(dbContext, nameof(dbContext));
            ThrowIfNull(values, nameof(values));

            var serializer = GetSerializer(dbContext);

            return AsQueryableValues(dbContext, serializer.Serialize(values), serializer.Format, GetCount(values), isUnicode ? InnerQueryStringUnicode : InnerQueryString);
        }

        /// <summary>
        /// Allows an <see cref="IEnumerable{Guid}">IEnumerable&lt;Guid&gt;</see> to be composed in an Entity Framework query.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/> owning the query.</param>
        /// <param name="values">The sequence of values to compose.</param>
        /// <returns>An <see cref="IQueryable{Guid}">IQueryable&lt;Guid&gt;</see> that can be composed with other entities in the query.</returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<Guid> AsQueryableValues(this DbContext dbContext, IEnumerable<Guid> values)
        {
            ThrowIfNull(dbContext, nameof(dbContext));
            ThrowIfNull(values, nameof(values));

            var serializer = GetSerializer(dbContext);

            return AsQueryableValues(dbContext, serializer.Serialize(values), serializer.Format, GetCount(values), InnerQueryGuid);
        }
    }
}
