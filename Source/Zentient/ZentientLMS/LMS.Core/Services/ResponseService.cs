using LMS.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Zentient.Repository;

namespace LMS.Core.Services
{
    public class ResponseService<TEntity, TContext> : ResponseService<TEntity, TContext, int>
        where TEntity : class, IEntity<int>, new()
        where TContext : DbContext
    {
        public ResponseService(TContext dbContext) : base(dbContext) { }
    }

    public class ResponseService<TEntity, TContext, TKey> : ICRUDService<TEntity, TContext, TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TContext : DbContext
        where TKey : notnull, IEquatable<TKey>
    {
        private readonly TContext _dbContext;

        public ResponseService(TContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellation = default)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                return await _dbContext.Set<TEntity>().ToListAsync(cancellation);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EntityNotFoundException("Failed to retrieve entities.", ex);
            }
        }

        public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellation = default)
        {
            TEntity? entity = null;

            try
            {
                cancellation.ThrowIfCancellationRequested();
                entity = await _dbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellation);
            }
            catch (OperationCanceledException)
            {
                throw;
            }

            if (entity == null)
            {
                throw new EntityNotFoundException($"Entity with ID {id} not found.");
            }

            return entity;
        }

        public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellation = default)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                var result = await _dbContext.Set<TEntity>().AddAsync(entity, cancellation);
                await _dbContext.SaveChangesAsync(cancellation);
                return result.Entity;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EntityNotFoundException("Failed to create entity.", ex);
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellation = default)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                var result = _dbContext.Set<TEntity>().Update(entity);
                await _dbContext.SaveChangesAsync(cancellation);
                return result.Entity;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EntityNotFoundException("Failed to update entity.", ex);
            }
        }

        public async Task<bool> DeleteAsync(TKey id, CancellationToken cancellation = default)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                var entity = await GetAsync(id, cancellation);
                _dbContext.Set<TEntity>().Remove(entity);
                return await _dbContext.SaveChangesAsync(cancellation) == 1;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EntityNotFoundException("Failed to delete entity.", ex);
            }
        }

        public async Task<OperationResult> ValidateAsync(TEntity entity, CancellationToken cancellation = default)
        {
            var result = new OperationResult { Success = true };

            if (entity.Id.Equals(default(TKey)))
            {
                result.Success = false;
                result.Errors.Append("Id cannot be the default value.");
            }

            // TODO: Add more validation logic as required

            return await Task.FromResult(result);
        }
    }
}
