﻿using System.Linq.Expressions;

namespace EntityFramework.Extensions
{
    public static class LinQExtension
    {
        public static IQueryable<TEntity> WhereIfNotNull<TEntity>(
            this IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>>? predicate)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        public static IQueryable<TEntity> GetPagination<TEntity>(
           this IQueryable<TEntity> query,
           int pageNumber,
           int pageSize)
        {
            query = query.Skip((pageNumber - 1) * pageSize);
            return query;
        }
    }
}
