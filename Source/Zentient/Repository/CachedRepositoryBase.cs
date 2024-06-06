// 
// Class: CachedRepositoryBase
// 
// Description:
// The <see cref="CachedRepositoryBase{TEntity, TKey}"/> class is a repository base class for caching entities. It provides a set of methods for caching entities in memory, including getting, adding, updating, and removing entities. The class is designed to be generic and flexible, allowing it to work with any entity type that implements the <see cref="IEntity{TKey}"/> interface.
// 
// Purpose:
// The purpose of the <see cref="CachedRepositoryBase{TEntity, TKey}"/> class is to provide a common set of methods for caching entities in memory. By defining a standard set of operations for caching entities, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The class also helps to improve the performance of the application by reducing the number of database queries needed to retrieve entities.
//
// Usage:
// The <see cref="CachedRepositoryBase{TEntity, TKey}"/> class is typically used in conjunction with repositories (<see cref="IRepository{TEntity, TKey}") to cache entities in memory. Developers can create concrete implementations of the <see cref="CachedRepositoryBase{TEntity, TKey}"/> class for specific entity types, providing a consistent and reusable way to interact with the database. By using the caching pattern, developers can write code that is more modular, flexible, and maintainable, leading to a more robust and scalable application.
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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace Zentient.Repository
{
    /// <summary>
    /// Repository base class for caching entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class CachedRepositoryBase<TEntity, TKey> : RepositoryBase<TEntity, TKey>, IDisposable
        where TEntity : class, IEntity<TKey> where TKey : struct
    {
        protected readonly IMemoryCache _cache;
        protected readonly TimeSpan _cacheDuration;

        /// <summary>
        /// Base class for a repository.
        /// </summary>"
        /// <param name="context">The database context of the repository.</param>
        /// <param name="cache">The memory cache to use for caching entities.</param>
        /// <param name="cacheDuration">Optional. The duration to cache entities. Defaults to 5 minutes.</param>
        /// <param name="exceptionHandler">Optional. The exception handler for the repository.</param>
        public CachedRepositoryBase(
            DbContext context,
            IMemoryCache cache,
            TimeSpan? cacheDuration = null,
            ExceptionHandler? exceptionHandler = null)
            : base(context, exceptionHandler)
        {
            _cache = cache;
            _cacheDuration = cacheDuration ?? TimeSpan.FromMinutes(5);
        }

        /// <inheritdoc/>
        public override async Task<TEntity?> GetAsync(TKey id, CancellationToken cancellation = default)
        {
            string cacheKey = $"{_entityType}_{id}";
            if (!_cache.TryGetValue(cacheKey, out TEntity? entity))
            {
                entity = await base.GetAsync(id, cancellation);
                if (entity != null)
                {
                    _cache.Set(cacheKey, entity, _cacheDuration);
                }
            }
            return entity;
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellation = default)
        {
            string cacheKey = $"{_entityType}_All";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<TEntity>? entities))
            {
                entities = await base.GetAllAsync(cancellation);

                if (entities != null)
                {
                    _cache.Set(cacheKey, entities, _cacheDuration);
                }
            }

            return entities ?? Enumerable.Empty<TEntity>();
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            string cacheKey = $"{_entityType}_Find_{predicate}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<TEntity>? entities))
            {
                entities = await base.FindAsync(predicate, cancellation);

                if (entities != null)
                {
                    _cache.Set(cacheKey, entities, _cacheDuration);
                }
            }

            return entities ?? Enumerable.Empty<TEntity>();
        }

        /// <inheritdoc/>
        public override async Task<EntityEntry<TEntity>?> AddAsync(
            TEntity entity,
            CancellationToken cancellation = default)
        {
            var result = await base.AddAsync(entity, cancellation);

            if (result != null)
            {
                _cache.Remove($"{_entityType}_All");
            }

            return result;
        }

        /// <inheritdoc/>
        public override async Task<int> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default)
        {
            var result = await base.AddRangeAsync(entities, cancellation);

            if (result > 0)
            {
                _cache.Remove($"{_entityType}_All");
            }

            return result;
        }

        /// <inheritdoc/>
        public override async Task<EntityEntry<TEntity>?> UpdateAsync(
            TEntity entity,
            CancellationToken cancellation = default)
        {
            var result = await base.UpdateAsync(entity, cancellation);

            if (result != null)
            {
                _cache.Remove($"{_entityType}_All");
            }

            return result;
        }

        /// <inheritdoc/>
        public override async Task<EntityEntry<TEntity>?> RemoveAsync(TEntity entity, CancellationToken cancellation = default)
        {
            var result = await base.RemoveAsync(entity, cancellation);

            if (result != null)
            {
                _cache.Remove($"{_entityType}_All");
            }

            return result;
        }

        /// <inheritdoc/>
        public override async Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default)
        {
            var result = await base.RemoveRangeAsync(entities, cancellation);

            if (result > 0)
            {
                _cache.Remove($"{_entityType}_All");
            }

            return result;
        }

        /// <inheritdoc/>
        public override async Task<PaginatedList<TEntity>> GetPagedAsync(
            int pageIndex,
            int pageSize = 10,
            Expression<Func<TEntity, bool>> filter = default!,
            Func<IQueryable<TEntity>,
                 IOrderedQueryable<TEntity>> orderBy = default!,
            CancellationToken cancellation = default)
        {
            string cacheKey = $"{_entityType}_Page_{pageIndex}_{pageSize}_{filter}_{orderBy}";

            if (!_cache.TryGetValue(cacheKey, out PaginatedList<TEntity>? page))
            {
                page = await base.GetPagedAsync(pageIndex, pageSize, filter, orderBy, cancellation);

                if (page != null)
                {
                    _cache.Set(cacheKey, page, _cacheDuration);
                }
            }

            return page ?? await PaginatedList<TEntity>.CreateAsync(Enumerable.Empty<TEntity>().AsQueryable(), pageIndex, pageSize);
        }

        /// <inheritdoc/>
        public override async Task<CursorPaginatedList<TEntity>> GetPagedByCursorAsync(
            TKey lastCursor,
            int pageSize = 10,
            Expression<Func<TEntity, bool>> filter = default!,
            Func<IQueryable<TEntity>,
                 IOrderedQueryable<TEntity>> orderBy = default!,
            CancellationToken cancellation = default)
        {
            string cacheKey = $"{_entityType}_Cursor_{lastCursor}_{pageSize}_{filter}_{orderBy}";

            if (!_cache.TryGetValue(cacheKey, out CursorPaginatedList<TEntity>? page))
            {
                page = await base.GetPagedByCursorAsync(lastCursor, pageSize, filter, orderBy, cancellation);

                if (page != null)
                {
                    _cache.Set(cacheKey, page, _cacheDuration);
                }
            }

            return page ?? await CursorPaginatedList<TEntity>.CreateAsync(Enumerable.Empty<TEntity>().AsQueryable(), 0, 1);
        }

        /// <inheritdoc/>
        public override async Task<int> CountAsync(CancellationToken cancellation = default)
        {
            string cacheKey = $"{_entityType}_Count";

            if (!_cache.TryGetValue(cacheKey, out int count))
            {
                count = await base.CountAsync(cancellation);

                if (count > 0)
                {
                    _cache.Set(cacheKey, count, _cacheDuration);
                }
            }

            return count;
        }

        /// <inheritdoc/>
        public override async Task<EntityEntry<TEntity>?> SoftDeleteAsync(TEntity entity, CancellationToken cancellation = default)
        {
            var result = await base.SoftDeleteAsync(entity, cancellation);

            if (result != null)
            {
                _cache.Remove($"{_entityType}_All");
            }

            return result;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _cache.Dispose();
                }
            }
        }
    }
}
