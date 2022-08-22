using BlazarTech.QueryableValues.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace BlazarTech.QueryableValues
{
    public static class QueryableValuesDbContextExtensions
    {
        internal const string InternalId = "qv-jDDd5B3uLYjJD9OnH1iEKtiHcaIcgo8VxoMN4vri0Rk-";

        private static readonly ConcurrentDictionary<Type, Type> DbSetByDbContext = new ConcurrentDictionary<Type, Type>();

        private static readonly Func<Type, Type> DbSetByDbContextFactory = dbContextType =>
        {
            var entityType = dbContextType
                .GetProperties()
                .Select(i => i.PropertyType)
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(i => i.GenericTypeArguments[0])
                .FirstOrDefault();

            if (entityType is null)
            {
                throw new InvalidOperationException("QueryableValues only works on a DbContext with at least one public DbSet<>.");
            }

            return entityType;
        };

        private static readonly ISerializer Serializer = new XmlSerializer();

        private static readonly Func<IQueryable<object>, string, IQueryable<short>> InnerQueryShort = (dbSet, serializedValues) =>
            from i in dbSet
            where InternalId + "short" == serializedValues
            select (short)1;

        private static readonly Func<IQueryable<object>, string, IQueryable<int>> InnerQueryInt = (dbSet, serializedValues) =>
            from i in dbSet
            where InternalId + "int" == serializedValues
            select 1;

        private static readonly Func<IQueryable<object>, string, IQueryable<long>> InnerQueryLong = (dbSet, serializedValues) =>
            from i in dbSet
            where InternalId + "long" == serializedValues
            select 1L;

        static QueryableValuesDbContextExtensions()
        {
            DbInterception.Add(new QueryableValuesCommandInterceptor());
        }

        private static IQueryable<T> AsQueryableValues<T>(
            DbContext dbContext,
            string serializedValues,
            int valuesCount,
            Func<IQueryable<object>, string, IQueryable<T>> getInnerQuery)
        {
            var entityType = DbSetByDbContext.GetOrAdd(dbContext.GetType(), DbSetByDbContextFactory);
            var dbSet = (IQueryable<object>)dbContext.Set(entityType);
            var innerQuery = getInnerQuery(dbSet, serializedValues);
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

        public static IQueryable<short> AsQueryableValues(this DbContext dbContext, IEnumerable<short> values)
        {
            return AsQueryableValues(dbContext, Serializer.Serialize(values), GetCount(values), InnerQueryShort);
        }

        public static IQueryable<int> AsQueryableValues(this DbContext dbContext, IEnumerable<int> values)
        {
            return AsQueryableValues(dbContext, Serializer.Serialize(values), GetCount(values), InnerQueryInt);
        }

        public static IQueryable<long> AsQueryableValues(this DbContext dbContext, IEnumerable<long> values)
        {
            return AsQueryableValues(dbContext, Serializer.Serialize(values), GetCount(values), InnerQueryLong);
        }
    }
}
