
using System.Linq.Expressions;
using Zentient.Core;
using Zentient.Repository.QueryObjects;

namespace Zentient.GenericRepository.QueryObjects
{
    public delegate Task<IQueryable<TEntity>> QueryHandler<TEntity>(IQueryable<TEntity> query, QueryParameters parameters)
        where TEntity : class, IEntity;
    public delegate Task<IQueryable<TResult>> QueryHandler<TEntity, TResult>(IQueryable<TEntity> query, QueryParameters parameters)
        where TEntity : class, IEntity;

    public class Query<TEntity> : IQuery<TEntity> where TEntity : class, IEntity
    {
        private QueryHandler<TEntity> _queryHandler;

        public Query(QueryHandler<TEntity> queryHandler)
        {
            _queryHandler = queryHandler;
        }

        public async Task<IQueryable<TEntity>> ApplyAsync(IQueryable<TEntity> query, string id)
        {
            if (query == null) throw new ArgumentNullException(nameof(query), "Query can not be null.");
            if (id == null) throw new ArgumentNullException(nameof(id), "Id can not be null.");

            return await _queryHandler(query, new QueryParameters { Id = id });
        }

        public async Task<IQueryable<TEntity>> ApplyAsync(IQueryable<TEntity> query, params object[] parameters)
        {
            return await _queryHandler(query, new QueryParameters { Parameters = parameters });
        }

        public async Task<IQueryable<TEntity>> ApplyAsync(IQueryable<TEntity> query, string? id = null, params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            return await _queryHandler(query, new QueryIncludeParameters<TEntity>(id, null, includeExpressions));
        }
    }

    public class Query
    {
        public static IQueryBuilder<TEntity> From<TEntity>() where TEntity : class, IEntity
            => new QueryBuilder<TEntity>();

        public static IQueryBuilder<TEntity> Where<TEntity>(
            params Expression<Func<TEntity, QueryParameters, bool>>[] expressions)
            where TEntity : class, IEntity
        {
            var builder = new QueryBuilder<TEntity>();

            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    builder.Where(expression);
                }
            }

            return builder;
        }

        public static IQueryBuilder<TEntity> OrderBy<TEntity>(
            params Expression<Func<TEntity, object>>[] expressions)
            where TEntity : class, IEntity
        {
            var builder = new QueryBuilder<TEntity>();

            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    builder.OrderBy(expression);
                }
            }

            return builder;
        }

        public static IQueryBuilder<TEntity> OrderByDescending<TEntity>(
            params Expression<Func<TEntity, object>>[] expressions)
            where TEntity : class, IEntity
        {
            var builder = new QueryBuilder<TEntity>();

            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    builder.OrderBy(expression, true);
                }
            }

            return builder;
        }

        public static IQueryBuilder<TEntity> GroupBy<TEntity>(
            params Expression<Func<TEntity, object>>[] expressions)
            where TEntity : class, IEntity
        {
            var builder = new QueryBuilder<TEntity>();

            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    builder.GroupBy(expression);
                }
            }

            return builder;
        }

        public static IQueryBuilder<TEntity> Include<TEntity>(
            params Expression<Func<TEntity, object>>[] expressions)
            where TEntity : class, IEntity
        {
            var builder = new QueryBuilder<TEntity>();

            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    builder.Include(expression);
                }
            }

            return builder;
        }
    }
}