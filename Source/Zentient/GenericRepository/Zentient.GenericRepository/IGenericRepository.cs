using Zentient.Core;

namespace Zentient.GenericRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        Task<IEnumerable<TEntity>> GetAsync(string id, CancellationToken cancellation = default);
    }
}