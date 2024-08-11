using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using Zentient.Core;

namespace Zentient.Repository.QueryObjects
{
    public class Query<TEntity> : IQuery<TEntity> where TEntity : class, IEntity
    {
        private readonly List<ParameterizedExpression<TEntity>> _predicates;
        private readonly List<Expression<Func<TEntity, object>>> _orderBys;
        private readonly List<Expression<Func<TEntity, object>>> _orderByDescendings;
        private readonly List<Expression<Func<TEntity, object>>> _includes;
        private readonly object[] _parameters;
        private readonly int _skip;
        private readonly int _take;

        Func<IQueryable<TEntity>, object[], IEnumerable<TEntity>>? _queryDelegate;

        public Query(
            List<ParameterizedExpression<TEntity>> predicates,
            List<Expression<Func<TEntity, object>>> orderBys,
            List<Expression<Func<TEntity, object>>> orderByDescendings,
            List<Expression<Func<TEntity, object>>> includes,
            object[] parameters,
            int skip = 0,
            int take = 0)
        {
            _predicates = predicates;
            _orderBys = orderBys;
            _orderByDescendings = orderByDescendings;
            _includes = includes;
            _parameters = parameters;
            _skip = skip;
            _take = take;
        }

        public async Task<IEnumerable<TEntity>> ApplyAsync(IQueryable<TEntity> query, object[] parameters)
        {
            _queryDelegate ??= Compile();
            return await Task.FromResult(_queryDelegate(query, parameters));
        }

        protected Func<IQueryable<TEntity>, object[], IEnumerable<TEntity>> Compile()
        {
            return (query, parameters) =>
            {
                query = ApplyPredicates(query, parameters);
                query = ApplyPagination(query);
                query = ApplyIncludingProperties(query);
                query = ApplyOrderBys(query);
                return query.AsEnumerable();
            };
        }

        private IQueryable<TEntity> ApplyPredicates(IQueryable<TEntity> query, object[] parameters)
        {
            foreach (var predicate in _predicates)
            {
                var lambda = predicate.Expression.Compile();
                query = query.Where(item => (bool)lambda.DynamicInvoke(item, predicate.Expression)!);
            }

            return query;
        }

        private IQueryable<TEntity> ApplyPagination(IQueryable<TEntity> query)
        {
            if (_skip > 0)
            {
                query = query.Skip(_skip);
            }

            if (_take > 0)
            {
                query = query.Take(_take);
            }

            return query;
        }

        private IQueryable<TEntity> ApplyIncludingProperties(IQueryable<TEntity> query)
        {
            foreach (var include in _includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        private IQueryable<TEntity> ApplyOrderBys(IQueryable<TEntity> query)
        {
            foreach (var orderBy in _orderBys)
            {
                query = query.OrderBy(orderBy);
            }

            foreach (var orderByDescending in _orderByDescendings)
            {
                query = query.OrderByDescending(orderByDescending);
            }

            return query;
        }
    }
}