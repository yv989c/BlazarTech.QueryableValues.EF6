using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BlazarTech.QueryableValues
{
    /// <summary>
    /// Extension methods provided by QueryableValues on the <see cref="IQueryableValuesEnabledDbContext"/> interface.
    /// </summary>
    public static class QueryableValuesEnabledDbContextExtensions
    {
        private static DbContext GetDbContext(IQueryableValuesEnabledDbContext dbContext)
        {
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (dbContext is DbContext castedDbContext)
            {
                return castedDbContext;
            }
            else
            {
                throw new InvalidOperationException("QueryableValues only works on a System.Data.Entity.DbContext type.");
            }
        }

        /// <summary>
        /// <inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{byte})"/>
        /// </summary>
        /// <param name="dbContext"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{byte})" path="/param[@name='dbContext']"/></param>
        /// <param name="values"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{byte})" path="/param[@name='values']"/></param>
        /// <returns><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{byte})"/></returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{byte})"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<byte> AsQueryableValues(this IQueryableValuesEnabledDbContext dbContext, IEnumerable<byte> values)
        {
            return GetDbContext(dbContext).AsQueryableValues(values);
        }

        /// <summary>
        /// <inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{short})"/>
        /// </summary>
        /// <param name="dbContext"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{short})" path="/param[@name='dbContext']"/></param>
        /// <param name="values"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{short})" path="/param[@name='values']"/></param>
        /// <returns><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{short})"/></returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{short})"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<short> AsQueryableValues(this IQueryableValuesEnabledDbContext dbContext, IEnumerable<short> values)
        {
            return GetDbContext(dbContext).AsQueryableValues(values);
        }

        /// <summary>
        /// <inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{int})"/>
        /// </summary>
        /// <param name="dbContext"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{int})" path="/param[@name='dbContext']"/></param>
        /// <param name="values"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{int})" path="/param[@name='values']"/></param>
        /// <returns><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{int})"/></returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{int})"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<int> AsQueryableValues(this IQueryableValuesEnabledDbContext dbContext, IEnumerable<int> values)
        {
            return GetDbContext(dbContext).AsQueryableValues(values);
        }

        /// <summary>
        /// <inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{long})"/>
        /// </summary>
        /// <param name="dbContext"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{long})" path="/param[@name='dbContext']"/></param>
        /// <param name="values"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{long})" path="/param[@name='values']"/></param>
        /// <returns><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{long})"/></returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{long})"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<long> AsQueryableValues(this IQueryableValuesEnabledDbContext dbContext, IEnumerable<long> values)
        {
            return GetDbContext(dbContext).AsQueryableValues(values);
        }

        /// <summary>
        /// <inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{string}, bool)"/>
        /// </summary>
        /// <param name="dbContext"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{string}, bool)" path="/param[@name='dbContext']"/></param>
        /// <param name="values"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{string}, bool)" path="/param[@name='values']"/></param>
        /// <param name="isUnicode"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{string}, bool)" path="/param[@name='isUnicode']"/></param>
        /// <returns><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{string}, bool)"/></returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{string}, bool)"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<string> AsQueryableValues(this IQueryableValuesEnabledDbContext dbContext, IEnumerable<string> values, bool isUnicode = false)
        {
            return GetDbContext(dbContext).AsQueryableValues(values, isUnicode: isUnicode);
        }

        /// <summary>
        /// <inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{Guid})"/>
        /// </summary>
        /// <param name="dbContext"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{Guid})" path="/param[@name='dbContext']"/></param>
        /// <param name="values"><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{Guid})" path="/param[@name='values']"/></param>
        /// <returns><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{Guid})"/></returns>
        /// <remarks><inheritdoc cref="QueryableValuesDbContextExtensions.AsQueryableValues(DbContext, IEnumerable{Guid})"/></remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<Guid> AsQueryableValues(this IQueryableValuesEnabledDbContext dbContext, IEnumerable<Guid> values)
        {
            return GetDbContext(dbContext).AsQueryableValues(values);
        }
    }
}
