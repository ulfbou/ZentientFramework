using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Zentient.Core;
using Zentient.GenericRepository.QueryObjects;
using Zentient.Repository.QueryObjects;

namespace Zentient.GenericRepository
{
    public class GenericRepositoryBase<TEntity>
        : GenericRepositoryBase<TEntity, DbContext>
        where TEntity : class, IEntity
    {
        public GenericRepositoryBase(
            DbContext context, IMapper mapper, ILogger<TEntity> logger, ExceptionHandler exceptionHandler)
            : base(context, mapper, logger, exceptionHandler)
        { }
    }

    public class GenericRepositoryBase<TEntity, TContext>
        where TEntity : class, IEntity
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _entities;
        private readonly IMapper? _mapper;
        private readonly ILogger<TEntity>? _logger;
        protected readonly string _entityType = typeof(TEntity).Name;
        protected ExceptionHandler? _exceptionHandler;

        public GenericRepositoryBase(
            TContext context, IMapper mapper, ILogger<TEntity> logger, ExceptionHandler exceptionHandler)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _entities = _context?.Set<TEntity>() ?? throw new InvalidOperationException("db set is null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
        }

        protected async Task<IEnumerable<TEntity>> ExecuteAsync(QueryHandler<TEntity> queryHandler, CancellationToken cancellation = default, params object[] parameters)
        {
            try
            {
                return await queryHandler(_entities, new QueryParameters(parameters));
            }
            catch (Exception ex)
            {
                throw new QueryExecutionException("An error occurred while executing the query.", ex);
            }
        }
        protected async Task<IEnumerable<TEntity>> ExecuteAsync(QueryHandler<TEntity> queryHandler, string id, CancellationToken cancellation = default)
        {
            try
            {
                return await queryHandler(_entities, new QueryParameters(id));
            }
            catch (Exception ex)
            {
                throw new QueryExecutionException("An error occurred while executing the query.", ex);
            }
        }

        protected async Task<IEnumerable<TEntity>> ExecuteAsync(
            QueryHandler<TEntity> queryHandler,
            string? id = null,
            object[]? parameters = null,
            CancellationToken cancellation = default,
            params Expression<Func<TEntity, object>>[] includeParameters)
        {
            try
            {
                return await queryHandler(_entities, new QueryIncludeParameters<TEntity>(id, parameters, includeParameters));
            }
            catch (Exception ex)
            {
                throw new QueryExecutionException("An error occurred while executing the query.", ex);
            }
        }
    }
}