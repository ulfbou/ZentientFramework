using LMS.Core.Entities;
using LMS.Core.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// reaper pattern is a design pattern that is used to delete the objects that are no longer in use by the application in order to free up the memory space. 
// reaper is a garbage collector that is used to delete the objects that are no longer in use by the application in order to free up the memory space. 
// Path: Source/Zentient/LMS/LMS.Core/Services/ICRUDService.cs

namespace LMS.Core.Services
{
    public class CRUDService<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : notnull, IEquatable<TKey>
    {
        private IdentityDbContext<ApplicationUser, ApplicationRole, Guid> _context;

        public CRUDService(IdentityDbContext<ApplicationUser, ApplicationRole, Guid> context)
        {
            _context = context;
        }
    }

    public class CRUDService<TEntity, TKey, TContext>
        : ICRUDService<TEntity, TKey, TContext>
        where TEntity : class, IEntity<TKey>
        where TKey : notnull, IEquatable<TKey>
        where TContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        private readonly IdentityDbContext<ApplicationUser, ApplicationRole, Guid> _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public CRUDService(IdentityDbContext<ApplicationUser, ApplicationRole, Guid> dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbContext.Set<TEntity>().Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<OperationResult> ValidateAsync(TEntity entity)
        {
            // Implement validation logic
            var successResult = new OperationResult { Success = true };
            var errors = new List<string>();
            var failureResult = new OperationResult { Success = false, Messages = errors };
            return await Task.FromResult(successResult);
        }
    }
}
