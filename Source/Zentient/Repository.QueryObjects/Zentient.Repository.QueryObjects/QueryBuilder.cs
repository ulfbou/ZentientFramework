using System;
using System.Linq.Expressions;
using Zentient.Core;

namespace Zentient.Repository.QueryObjects
{
    public class QueryBuilder<T> : IQueryBuilder<T> where T : class, IEntity
    {
        private readonly List<ParameterizedExpression<T>> _predicates = new();
        private readonly List<Expression<Func<T, object>>> _includes = new();
        private readonly List<Expression<Func<T, object>>> _selectors = new();
        private readonly List<Expression<Func<T, object>>> _orderBys = new();
        private readonly List<Expression<Func<T, object>>> _orderByDescendings = new();
        private readonly List<Expression<Func<T, object>>> _groupKeySelectors = new();
        private readonly object[] _parameters;
        private int _skip;
        private int _take;

        public QueryBuilder(params object[] parameters)
        {
            _parameters = parameters;
        }

        public IQueryBuilder<T> Where(ParameterizedExpression<T> predicate)
        {
            _predicates.Add(predicate);
            return this;
        }

        public IQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is Expression<Func<T, object>> orderByKeySelector)
            {
                _orderBys.Add(orderByKeySelector);
            }
            else
            {
                throw new ArgumentException("Key selector must be an expression of type Func<T, object>", nameof(keySelector));
            }

            return this;
        }

        public IQueryBuilder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is Expression<Func<T, object>> orderByKeySelector)
            {
                _orderByDescendings.Add(orderByKeySelector);
            }
            else
            {
                throw new ArgumentException("Key selector must be an expression of type Func<T, object>", nameof(keySelector));
            }

            return this;
        }

        public IQueryBuilder<T> GroupBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is Expression<Func<T, object>> groupByKeySelector)
            {
                _groupKeySelectors.Add(groupByKeySelector);
            }
            else
            {
                throw new ArgumentException("Key selector must be an expression of type Func<T, object>", nameof(keySelector));
            }

            return this;
        }

        public IQueryBuilder<T> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            if (selector is Expression<Func<T, object>> selectSelector)
            {
                _selectors.Add(selectSelector);
            }
            else
            {
                throw new ArgumentException("Selector must be an expression of type Func<T, object>", nameof(selector));
            }

            return this;
        }

        public IQueryBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            if (navigationPropertyPath is Expression<Func<T, object>> includeNavigationPropertyPath)
            {
                _includes.Add(includeNavigationPropertyPath);
            }
            else
            {
                throw new ArgumentException("Navigation property path must be an expression of type Func<T, object>", nameof(navigationPropertyPath));
            }

            return this;
        }

        public IQueryBuilder<T> Skip(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
            _skip = count;
            return this;
        }

        public IQueryBuilder<T> Take(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
            _take = count;
            return this;
        }

        public IQuery<T> Build()
        {
            return new Query<T>(_predicates, _orderBys, _orderByDescendings, _includes, _parameters, _skip, _take);
        }
    }
}