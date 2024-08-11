using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Zentient.Core.Services
{
    public interface ICRUDService<TEntity, TKey>
        where TEntity : class, IEntity<Guid>
        where TKey : notnull, IEquatable<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(Guid id);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(Guid id);
        Task<OperationResult> ValidateAsync(TEntity entity);
    }
}
