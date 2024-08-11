using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Linq.Expressions;

using Zentient.Core;
using Zentient.GenericRepository.QueryObjects;

namespace Zentient.GenericRepository
{
    public class GenericRepository<TEntity>
        : GenericRepository<TEntity, DbContext>, IGenericRepository<TEntity>
        where TEntity : class, IEntity
    {
        public GenericRepository(
            DbContext context, IMapper mapper, ILogger<TEntity> logger, ExceptionHandler exceptionHandler)
            : base(context, mapper, logger, exceptionHandler)
        { }
    }

    public class GenericRepository<TEntity, TContext>
        : GenericRepositoryBase<TEntity, TContext>
        where TEntity : class, IEntity
        where TContext : DbContext
    {
        protected QueryTemplates<TEntity> Templates { get; set; }
        protected QueryHandler<TEntity> GetByIdAsync { get; set; }

        public GenericRepository(
            TContext context, IMapper mapper, ILogger<TEntity> logger, ExceptionHandler exceptionHandler)
            : base(context, mapper, logger, exceptionHandler)
        {
            Templates = new();

            GetByIdAsync = Templates.GetByIdAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAsync(string id, CancellationToken cancellation = default)
        {
            return await ExecuteAsync(GetByIdAsync, id, null, cancellation);
        }

        public async Task<IEnumerable<TEntity>> GetAsync<TIncludable>(
            string id,
            CancellationToken cancellation = default,
            params Expression<Func<TEntity, object>>[] includeParameters)
        {
            return await ExecuteAsync(GetByIdAsync, id, null, cancellation, includeParameters);
        }
    }
}