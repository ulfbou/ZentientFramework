using System.Linq.Expressions;
using Zentient.Core;

namespace Zentient.GenericRepository.QueryObjects
{
    public interface IQuery<TEntity> where TEntity : class, IEntity
    {
        Task<IQueryable<TEntity>> ApplyAsync(IQueryable<TEntity> query, params object[] parameters);
        Task<IQueryable<TEntity>> ApplyAsync(IQueryable<TEntity> query, string id);
        Task<IQueryable<TEntity>> ApplyAsync(IQueryable<TEntity> query, string? id = null, params Expression<Func<TEntity, object>>[] includeExpressions);
    }
}