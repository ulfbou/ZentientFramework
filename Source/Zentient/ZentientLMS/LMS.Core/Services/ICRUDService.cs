using Microsoft.EntityFrameworkCore;
using Zentient.Repository;

namespace LMS.Core.Services
{
    public interface ICRUDService<TEntity>
        : ICRUDService<TEntity, DbContext>
        where TEntity : class, IEntity<Guid>
    {
    }
    public interface ICRUDService<TEntity, TContext>
        : ICRUDService<TEntity, TContext, Guid>
        where TEntity : class, IEntity<Guid>
        where TContext : DbContext
    {
    }
    public interface ICRUDService<TEntity, TContext, TKey>
        where TEntity : class, IEntity<TKey>
        where TContext : DbContext
        where TKey : notnull, IEquatable<TKey>
    {
        public Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellation = default);
        public Task<TEntity> GetAsync(TKey id, CancellationToken cancellation = default);
        public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellation = default);
        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellation = default);
        public Task<bool> DeleteAsync(TKey id, CancellationToken cancellation = default);
        public Task<OperationResult> ValidateAsync(TEntity entity, CancellationToken cancellation = default);
    }
}