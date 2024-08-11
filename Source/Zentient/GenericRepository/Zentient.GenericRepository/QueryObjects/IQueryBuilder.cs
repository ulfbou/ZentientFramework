using System.Linq.Expressions;
using Zentient.Core;

namespace Zentient.GenericRepository.QueryObjects
{
    public interface IQueryBuilder<TEntity> where TEntity : class, IEntity
    {
        Func<IQueryable<TEntity>, QueryParameters, Task<bool>> Any();
        IQuery<TEntity> Build();
        Func<IQueryable<TEntity>, QueryParameters, Task<int>> Count();
        QueryBuilder<TEntity> GroupBy(Expression<Func<TEntity, object>> groupBy);
        QueryBuilder<TEntity> Include(Expression<Func<TEntity, object>> include);
        QueryBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy, bool orderbyDescending = false);
        QueryBuilder<TEntity> Skip(int skip);
        QueryBuilder<TEntity> Take(int take);
        QueryBuilder<TEntity> Where(Expression<Func<TEntity, QueryParameters, bool>> expression);
    }

    public interface IQueryBuilder<TEntity, TResult> : IQueryBuilder<TEntity>
        where TEntity : class, IEntity
        where TResult : class
    {
        new QueryHandler<TEntity, TResult> Build();
        QueryBuilder<TEntity, TResult> Select(Expression<Func<TEntity, TResult>> selector);
    }
}