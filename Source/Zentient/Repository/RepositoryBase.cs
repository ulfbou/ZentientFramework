﻿//
// Class: RepositoryBase
//
// Description:
// The RepositoryBase class is a base class for repositories that interact with a database context. It provides a set of methods for querying, adding, updating, and deleting entities in the database, as well as handling exceptions that occur during data access operations. The class is designed to be generic and flexible, allowing it to work with any entity type.
//
// Purpose:
// The purpose of the RepositoryBase class is to provide a common set of methods for working with a database context in a consistent and reusable way. By defining a standard set of operations for managing entities, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The class also helps to decouple the data access logic from the rest of the application, making it easier to test and refactor the code in the future.
//
// Usage:
// The RepositoryBase class is typically used as a base class for concrete repository implementations that interact with a specific database context. Developers can create concrete repository classes that inherit from the RepositoryBase class, providing a consistent and reusable way to interact with the database. By using the repository pattern, developers can write code that is more modular, flexible, and maintainable, leading to a more robust and scalable application.
//
// MIT License
//
// Copyright (c) 2024 Ulf Bourelius
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System.Linq.Expressions;
using Zentient.Core;

namespace Zentient.Repository
{
    /// <summary>
    /// Base class for a repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TKey">The key type</typeparam>
    public class RepositoryBase<TEntity, TKey>
        : IRepository<TEntity, TKey>, IDisposable, IAsyncDisposable
        where TEntity : class, IEntity<TKey>
        where TKey : struct
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        private readonly IMapper? _mapper;
        private readonly ILogger<TEntity>? _logger;
        protected readonly string _entityType = typeof(TEntity).Name;
        protected ExceptionHandler? _exceptionHandler;
        protected bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="context">The database context of the repository.</param>
        /// <param name="exceptionHandler">Optional. The exception handler for the repository.</param>
        /// <throws></throws>
        public RepositoryBase(DbContext context, IMapper? mapper = null, ILogger<TEntity>? logger = null, ExceptionHandler? exceptionHandler = default)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context?.Set<TEntity>() ?? throw new InvalidOperationException("db set is null");
            _mapper = mapper;
            _logger = logger;
            _exceptionHandler = exceptionHandler;
        }

        // TODO: Verify and document the events
        public event Func<TEntity, Task>? EntityAdded;
        public event Func<TEntity, Task>? EntityUpdated;
        public event Func<TEntity, Task>? EntityRemoved;

        /// <summary>
        /// Get an entity by its key asynchronously.
        /// </summary>
        /// <param name="id">The key of the entity to get.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>The entity, if it exists. Otherwise null.</returns>
        public virtual async Task<TEntity?> GetAsync(TKey id, CancellationToken cancellation = default)
        {
            return await ExecuteDbOperationAsync<TEntity?>(() => _dbSet.FindAsync(new object[] { id }, cancellation).AsTask(), cancellation);
        }

        /// <summary>
        /// Get all entities asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>All entities in the repository.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellation = default)
        {
            var op = () => _dbSet.ToListAsync(cancellation);
            return await ExecuteDbOperationAsync(op, cancellation);
        }

        /// <summary>
        /// Find entities that match the predicate asynchronously.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>Entities that match the predicate.</returns>
        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            var op = () => _dbSet.Where(predicate).ToListAsync(cancellation);
            return await ExecuteDbOperationAsync(op, cancellation);
        }

        protected async Task<T> ExecuteDbOperationAsync<T>(Func<Task<T>> operation, CancellationToken cancellation)
        {
            try
            {
                cancellation.ThrowIfCancellationRequested();
                return await operation();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, cancellation);
                return default!;
            }
        }

        /// <summary>
        /// Add an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>The entity entry, if it was added. Otherwise null.</returns>
        public virtual async Task<EntityEntry<TEntity>?> AddAsync(
            TEntity entity,
            CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            EntityEntry<TEntity>? entityentry = null;

            try
            {
                cancellation.ThrowIfCancellationRequested();
                entityentry = await _dbSet.AddAsync(entity, cancellation);
                await OnEntityAddedAsync(entity);
                //await LogAuditAsync(entity, "Add", cancellation);
                await _context.SaveChangesAsync(cancellation);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, cancellation);
            }

            return entityentry;
        }

        /// <summary>
        /// Add multiple entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>The number of entities added.</returns>
        public virtual async Task<int> AddRangeAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));

            int result = 0;

            try
            {
                cancellation.ThrowIfCancellationRequested();
                await _dbSet.AddRangeAsync(entities, cancellation);

                foreach (var entity in entities)
                {
                    await OnEntityAddedAsync(entity);
                    //await LogAuditAsync(entity, "Add", cancellation);
                }

                result = await _context.SaveChangesAsync(cancellation);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, cancellation);
            }

            return result;
        }

        /// <summary>
        /// Update an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The entity entry, if it was updated. Otherwise null.</returns>
        public virtual async Task<EntityEntry<TEntity>?> UpdateAsync(
            TEntity entity,
            CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            EntityEntry<TEntity>? entityEntry = null;

            try
            {
                cancellation.ThrowIfCancellationRequested();
                entityEntry = _dbSet.Update(entity);
                await OnEntityUpdatedAsync(entity);
                //await LogAuditAsync(entity, "Update", cancellation);
                await _context.SaveChangesAsync(cancellation);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, cancellation);
            }

            return entityEntry;
        }

        /// <summary>
        /// Remove an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>The entity entry, if it was removed. Otherwise null.</returns>
        public virtual async Task<EntityEntry<TEntity>?> RemoveAsync(
            TEntity entity,
            CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            EntityEntry<TEntity>? entityEntry = null;

            try
            {
                cancellation.ThrowIfCancellationRequested();
                entityEntry = _dbSet.Remove(entity);
                await OnEntityRemovedAsync(entity);
                //await LogAuditAsync(entity, "Remove", cancellation);
                await _context.SaveChangesAsync(cancellation);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, cancellation);
            }

            return entityEntry;
        }

        /// <summary>
        /// Remove multiple entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>The number of entities removed.</returns>
        public virtual async Task<int> RemoveRangeAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));

            int count = 0;

            try
            {
                cancellation.ThrowIfCancellationRequested();
                _dbSet.RemoveRange(entities);

                foreach (var entity in entities)
                {
                    await OnEntityRemovedAsync(entity);
                    //await LogAuditAsync(entity, "Add", cancellation);
                }

                count = await _context.SaveChangesAsync();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, cancellation);
            }

            return count;
        }

        /// <summary>
        /// Get a paginated list of entities asynchronously.
        /// </summary>
        /// <param name="pageIndex">Optional. The index of the page to get.</param>
        /// <param name="pageSize">Optional. The size of the page to get. Defaults to 10.</param>
        /// <param name="filter">Optional. The filter to apply to the search.</param>
        /// <param name="orderBy">Optional. The order to apply to the search.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A paginated list of entities.</returns>
        public virtual async Task<PaginatedList<TEntity>> GetPagedAsync(
            int pageIndex,
            int pageSize = 10,
            Expression<Func<TEntity, bool>> filter = default!,
            Func<IQueryable<TEntity>,
                 IOrderedQueryable<TEntity>> orderBy = default!,
            CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(pageIndex, nameof(pageIndex));
            ArgumentOutOfRangeException.ThrowIfLessThan(pageIndex, 1, nameof(pageIndex));
            ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1, nameof(pageSize));

            try
            {
                IQueryable<TEntity> query = _dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                return await PaginatedList<TEntity>.CreateAsync(query, pageIndex, pageSize);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, cancellation);
                return await PaginatedList<TEntity>.CreateAsync(new List<TEntity>().AsQueryable());
            }
        }

        /// <summary>
        /// Get a paginated list of entities by cursor asynchronously.
        /// </summary>
        /// <param name="lastCursor">The last cursor to use for the search.</param>
        /// <param name="pageSize">Optional. The size of the page to get. Defaults to 10.</param>
        /// <param name="filter">Optional. The filter to apply to the search.</param>
        /// <param name="orderBy">Optional. The order to apply to the search.</param>
        /// <param name="keySelector">Optional. The key selector for the search.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A cursor paginated list of entities.</returns>
        /// <remarks>The primary key is assumed to be name Id.</remarks>
        public virtual async Task<CursorPaginatedList<TEntity>> GetPagedByCursorAsync(
            TKey lastCursor,
            int pageSize = 10,
            Expression<Func<TEntity, bool>> filter = default!,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = default!,
            CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(lastCursor, nameof(lastCursor));
            ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1, nameof(pageSize));

            try
            {
                IQueryable<TEntity> query = _dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                return await CursorPaginatedList<TEntity>.CreateAsync(query, lastCursor, pageSize);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, cancellation);
                return await CursorPaginatedList<TEntity>.CreateAsync(new List<TEntity>().AsQueryable(), lastCursor, pageSize);
            }
        }

        /// <summary>
        /// Soft delete an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to soft delete.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>The entity entry, if it was soft deleted. Otherwise null.</returns>
        /// <exception cref="InvalidOperationException">Thrown if entity does not support soft delete.</exception>
        public virtual async Task<EntityEntry<TEntity>?> SoftDeleteAsync(
            TEntity entity,
            CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            if (entity is ISoftDeletable<TKey> softDeletable)
            {
                EntityEntry<TEntity>? entityEntry = null;
                try
                {
                    cancellation.ThrowIfCancellationRequested();
                    softDeletable.IsDeleted = true;
                    entityEntry = _dbSet.Update(entity);
                    //await LogAuditAsync(entity, "SoftDelete", cancellation);
                    await _context.SaveChangesAsync(cancellation);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(ex, cancellation);
                }

                return entityEntry;
            }

            throw new InvalidOperationException($"Entity type `{_entityType}` does not support soft delete, since it does not implement the interface ISoftDeletable.");
        }

        /// <summary>
        /// Undelete an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to soft delete.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>The entity entry, if it was soft deleted. Otherwise null.</returns>
        /// <exception cref="InvalidOperationException">Thrown if entity does not support soft delete.</exception>
        public virtual async Task<EntityEntry<TEntity>?> SoftUndeleteAsync(TEntity entity, CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            if (entity is ISoftDeletable<TKey> softDeletable)
            {
                try
                {
                    cancellation.ThrowIfCancellationRequested();
                    softDeletable.IsDeleted = false;
                    var entityEntry = _dbSet.Update(entity);
                    //await LogAuditAsync(entity, "SoftUndelete", cancellation);
                    await _context.SaveChangesAsync(cancellation);
                    return entityEntry;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(ex, cancellation);
                }
            }

            throw new InvalidOperationException($"Entity type `{_entityType}` does not support soft delete, since it does not implement the interface ISoftDeletable.");
        }

        /// <summary>
        /// Count the number of entities asynchronously.
        /// </summary>
        /// <returns>The number of entities in the repository.</returns>
        public virtual async Task<int> CountAsync(CancellationToken cancellation = default)
        {
            try
            {
                return await _dbSet.CountAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
                return 0;
            }
        }

        // TODO: Verify and document the rest of the methods

        protected virtual async Task OnEntityAddedAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            if (EntityAdded != null)
            {
                await EntityAdded(entity);
            }
        }

        protected virtual async Task OnEntityUpdatedAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            if (EntityUpdated != null)
            {
                await EntityUpdated(entity);
            }
        }

        protected virtual async Task OnEntityRemovedAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            if (EntityRemoved != null)
            {
                await EntityRemoved(entity);
            }
        }

        protected virtual async Task HandleExceptionAsync(Exception ex, CancellationToken cancellation = default)
        {
            if (_exceptionHandler != null)
            {
                await _exceptionHandler(ex, cancellation);
            }
            else
            {
                throw ex;
            }
        }

        protected virtual async Task DefaultExceptionHandler(Exception ex)
        {
            ArgumentNullException.ThrowIfNull(ex, nameof(ex));

            await Console.Out.WriteLineAsync(ex.Message);
        }

        /// <summary>
        /// Set the exception handler for the repository.
        /// </summary>
        public void SetExceptionHandler(ExceptionHandler exceptionHandler)
        {
            ArgumentNullException.ThrowIfNull(exceptionHandler, nameof(exceptionHandler));
            _exceptionHandler = exceptionHandler;
        }

        // TODO: Verify and document the rest of the methods
        private async Task LogAuditAsync(TEntity entity, string operation, CancellationToken cancellation)
        {
            var auditLog = new AuditLog<TKey>
            {
                EntityId = (entity as dynamic).Id,
                EntityType = _entityType,
                Operation = operation,
                Timestamp = DateTime.UtcNow,
                Changes = JsonConvert.SerializeObject(entity)
            };
            //await _context.Set<AuditLog<TKey>>().AddAsync(auditLog, cancellation);
            await _context.SaveChangesAsync(cancellation);
        }
        public void Dispose()
        {
            // Call the virtual Dispose method. Suppress finalization.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_context is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                _context.Dispose();
            }
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }
    }
}