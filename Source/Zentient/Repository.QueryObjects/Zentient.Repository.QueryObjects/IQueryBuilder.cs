using System.Linq.Expressions;
using Zentient.Core;

namespace Zentient.Repository.QueryObjects
{
    public interface IQueryBuilder<TEntity> where TEntity : class, IEntity
    {
        IQueryBuilder<TEntity> Where(ParameterizedExpression<TEntity> predicate);
        IQueryBuilder<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector);
        IQueryBuilder<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector);
        IQueryBuilder<TEntity> GroupBy<TKey>(Expression<Func<TEntity, TKey>> keySelector);
        IQueryBuilder<TEntity> Select<TResult>(Expression<Func<TEntity, TResult>> selector);
        IQueryBuilder<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigationPropertyPath);
        IQueryBuilder<TEntity> Skip(int count);
        IQueryBuilder<TEntity> Take(int count);
        IQuery<TEntity> Build();
    }
}