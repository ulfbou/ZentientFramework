using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Zentient.Core;


namespace Zentient.GenericRepository.QueryObjects
{
    // Projection query builder
    public class QueryBuilder<TEntity, TResult> : QueryBuilder<TEntity>, IQueryBuilder<TEntity, TResult> where TEntity : class, IEntity where TResult : class
    {
        private Expression<Func<TEntity, TResult>>? _selector;

        public QueryBuilder<TEntity, TResult> Select(Expression<Func<TEntity, TResult>> selector)
        {
            _selector = selector;
            return this;
        }

        public new QueryHandler<TEntity, TResult> Build()
        {
            if (_selector == null) throw new InvalidBuildException("Selector is required, but missing.");

            return async (query, parameters) =>
            {
                QueryHandler<TEntity> queryHandler = CreateQueryHandler();
                query = await queryHandler(query, parameters);
                var result = query.Select<TEntity, TResult>(_selector);

                return result;
            };
        }

        // Create a query handler that accepts specific parameters
        public new QueryHandler<TEntity, TResult> Build<TParameter>() where TParameter : QueryParameters
        {
            if (_selector == null) throw new InvalidBuildException("Selector is required, but missing.");

            return async (query, parameters) =>
            {
                QueryHandler<TEntity> queryHandler = CreateQueryHandler<TParameter>();
                query = await queryHandler(query, parameters);
                var result = query.Select<TEntity, TResult>(_selector);

                return result;
            };
        }
    }

    public class QueryBuilder<TEntity> : IQueryBuilder<TEntity> where TEntity : class, IEntity
    {
        private readonly HashSet<Expression<Func<TEntity, QueryParameters, bool>>> _expressions = new();
        private readonly ConcurrentDictionary<int, Func<TEntity, QueryParameters, bool>> _compiledExpressions = new();
        private Expression<Func<TEntity, object>>? _orderBy;
        private bool? _orderByDescending;
        private Expression<Func<TEntity, object>>? _groupBy;
        private readonly List<Expression<Func<TEntity, object>>> _includes = new();
        private int? _skip;
        private int? _take;

        public QueryBuilder<TEntity> Where(Expression<Func<TEntity, QueryParameters, bool>> expression)
        {
            _expressions.Add(expression);

            var cacheKey = GetCacheKey(expression);
            if (!_compiledExpressions.TryGetValue(cacheKey, out var compiledExpression))
            {
                compiledExpression = expression.Compile();
                _compiledExpressions.TryAdd(cacheKey, compiledExpression);
            }

            return this;
        }

        public QueryBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy, bool orderbyDescending = false)
        {
            _orderBy = orderBy;
            _orderByDescending = orderbyDescending;
            return this;
        }

        public QueryBuilder<TEntity> Skip(int skip)
        {
            _skip = skip;
            return this;
        }

        public QueryBuilder<TEntity> Take(int take)
        {
            _take = take;
            return this;
        }

        public QueryBuilder<TEntity> Include(Expression<Func<TEntity, object>> include)
        {
            _includes.Add(include);
            return this;
        }

        public QueryBuilder<TEntity> GroupBy(Expression<Func<TEntity, object>> groupBy)
        {
            _groupBy = groupBy;
            return this;
        }

        public IQuery<TEntity> Build()
        {
            QueryHandler<TEntity> queryDelegate = CreateQueryHandler();
            return new Query<TEntity>(queryDelegate);
        }

        public IQuery<TEntity> Build<TParameter>() where TParameter : QueryParameters
        {
            QueryHandler<TEntity> queryDelegate = CreateQueryHandler<TParameter>();
            return new Query<TEntity>(queryDelegate);
        }

        public Func<IQueryable<TEntity>, QueryParameters, Task<int>> Count()
        {
            return async (query, parameters) =>
            {
                var builtQuery = await Build().ApplyAsync(query, parameters);
                return builtQuery.Count();
            };
        }

        public Func<IQueryable<TEntity>, QueryParameters, Task<bool>> Any()
        {
            return async (query, parameters) =>
            {
                var builtQuery = await Build().ApplyAsync(query, parameters);
                return builtQuery.Any();
            };
        }

        internal QueryHandler<TEntity> CreateQueryHandler()
        {
            return (query, parameters) =>
            {
                if (parameters == null) throw new ArgumentNullException(nameof(parameters), "Parameters can not be null.");

                foreach (var expression in _expressions)
                {
                    var compiledExpression = _compiledExpressions[GetCacheKey(expression)];
                    query = query.Where(item => compiledExpression(item, parameters));
                }

                if (_orderBy != null)
                {
                    query = _orderByDescending == true ? query.OrderByDescending(_orderBy) : query.OrderBy(_orderBy);
                }

                if (_skip.HasValue)
                {
                    query = query.Skip(_skip.Value);
                }

                if (_take.HasValue)
                {
                    query = query.Take(_take.Value);
                }

                if (_groupBy != null)
                {
                    query = query.GroupBy(_groupBy).SelectMany(group => group);
                }

                if (_includes.Any())
                {
                    foreach (var include in _includes)
                    {
                        query = query.Include(include);
                    }
                }

                return Task.FromResult(query);
            };
        }

        // Create a query handler that accepts specific parameters
        internal QueryHandler<TEntity> CreateQueryHandler<TParameter>() where TParameter : QueryParameters
        {
            return (query, parameters) =>
            {
                if (parameters == null) throw new ArgumentNullException(nameof(parameters), "Parameters can not be null.");
                if (parameters is TParameter typedParameters)
                {
                    foreach (var expression in _expressions)
                    {
                        var compiledExpression = _compiledExpressions[GetCacheKey(expression)];
                        query = query.Where(item => compiledExpression(item, parameters));
                    }

                    if (_orderBy != null)
                    {
                        query = _orderByDescending == true ? query.OrderByDescending(_orderBy) : query.OrderBy(_orderBy);
                    }

                    if (_skip.HasValue)
                    {
                        query = query.Skip(_skip.Value);
                    }

                    if (_take.HasValue)
                    {
                        query = query.Take(_take.Value);
                    }

                    if (_groupBy != null)
                    {
                        query = query.GroupBy(_groupBy).SelectMany(group => group);
                    }

                    if (_includes.Any())
                    {
                        foreach (var include in _includes)
                        {
                            query = query.Include(include);
                        }
                    }

                    return Task.FromResult(query);
                }

                throw new QueryParametersTypeMismatchException(typeof(TParameter), parameters.GetType());
            };
        }

        private int GetCacheKey(Expression<Func<TEntity, QueryParameters, bool>> expression)
        {
            // TODO: Implement cache key generation
            return expression.GetHashCode();
        }
    }
}