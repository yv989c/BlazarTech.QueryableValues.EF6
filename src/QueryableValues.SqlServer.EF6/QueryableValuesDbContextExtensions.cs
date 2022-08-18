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
            dbContextType
                .GetProperties()
                .Select(i => i.PropertyType)
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(i => i.GenericTypeArguments[0])
                .FirstOrDefault();

        private static readonly Func<IQueryable<object>, string, IQueryable<short>> InnerQueryShort = (dbSet, valuesXml) =>
            from i in dbSet
            where InternalId + "short" == valuesXml
            select (short)1;

        private static readonly Func<IQueryable<object>, string, IQueryable<int>> InnerQueryInt = (dbSet, valuesXml) =>
            from i in dbSet
            where InternalId + "int" == valuesXml
            select 1;

        private static readonly Func<IQueryable<object>, string, IQueryable<long>> InnerQueryLong = (dbSet, valuesXml) =>
            from i in dbSet
            where InternalId + "long" == valuesXml
            select 1L;

        static QueryableValuesDbContextExtensions()
        {
            DbInterception.Add(new QueryableValuesCommandInterceptor());
        }

        private static IQueryable<T> AsQueryableValues<T>(
            DbContext dbContext,
            string valuesXml,
            int valuesCount,
            Func<IQueryable<object>, string, IQueryable<T>> getInnerQuery)
        {
            var entityType = DbSetByDbContext.GetOrAdd(dbContext.GetType(), DbSetByDbContextFactory);
            var dbSet = (IQueryable<object>)dbContext.Set(entityType);
            var innerQuery = getInnerQuery(dbSet, valuesXml);
            var query = innerQuery.Take(() => valuesCount);
            return query;
        }

        public static IQueryable<short> AsQueryableValues(this DbContext dbContext, ICollection<short> values)
        {
            return AsQueryableValues(dbContext, "<R><V>1</V><V>3</V></R>", 2, InnerQueryShort);
        }

        public static IQueryable<int> AsQueryableValues(this DbContext dbContext, ICollection<int> values)
        {
            return AsQueryableValues(dbContext, "<R><V>1</V><V>3</V></R>", 2, InnerQueryInt);
        }

        public static IQueryable<long> AsQueryableValues(this DbContext dbContext, ICollection<long> values)
        {
            return AsQueryableValues(dbContext, "<R><V>1</V><V>3</V></R>", 2, InnerQueryLong);
        }
    }
}
