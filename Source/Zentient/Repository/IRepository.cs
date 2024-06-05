//
// Class: IRepository
//
// Description:
// The IRepository interface represents a generic repository for entities in a database context. It provides a set of methods for performing CRUD operations on entities, such as adding, updating, and removing them, as well as querying for entities based on various criteria. The interface is designed to be generic and flexible, allowing it to work with any entity type that implements the IEntity interface.
// The IRepository interface is typically used in conjunction with a unit of work (IUnitOfWork) to manage the overall data access and transactional behavior of an application. By using the repository pattern, developers can encapsulate the data access logic for their entities in a reusable and testable way, making it easier to maintain and extend the application over time.
//
// Purpose:
// The purpose of the IRepository interface is to provide a common set of methods for working with entities in a database context. By defining a standard set of CRUD operations, developers can write code that is more consistent, maintainable, and testable, leading to higher-quality software. The interface also helps to decouple the data access logic from the rest of the application, making it easier to swap out different data access technologies or refactor the code in the future.
//
// Usage:
// The IRepository interface is typically used in conjunction with a unit of work (IUnitOfWork) to manage the overall data access and transactional behavior of an application. Developers can create concrete implementations of the IRepository interface for each entity type in their application, providing a consistent and reusable way to interact with the database. By using the repository pattern, developers can write code that is more modular, flexible, and maintainable, leading to a more robust and scalable application.
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

using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zentient.Repository
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        // Generate professional documentation for the IRepository interface
        /// <summary>
        /// Get an entity by its key asynchronously.
        /// </summary>
        /// <param name="id">The key of the entity to get.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type `TEntity` with the specified primary key value, if it exists; otherwise null.</returns>
        Task<TEntity?> GetAsync(TKey id, CancellationToken cancellation = default);

        /// <summary>
        /// Get all entities asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains all entities of type `TEntity`.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellation = default);

        /// <summary>
        /// Find entities that match the predicate asynchronously.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operations. The task result contains the entities of type `TEntity` that match the predicate.</returns>
        Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellation = default);

        /// <summary>
        /// Add an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operations. The task result contains the entity entry of type `EntityEntry<TEntity>`, if it was added; otherwise null.</returns>
        Task<EntityEntry<TEntity>?> AddAsync(TEntity entity, CancellationToken cancellation = default);

        /// <summary>
        /// Add multiple entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operations. The task result contains the number of entities of type `TEntity` added.</returns>
        Task<int> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default);

        /// <summary>
        /// Update an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operations. The task result contains the entity entry of type `EntityEntry<TEntity>`, if it was updated; otherwise null.</returns>
        Task<EntityEntry<TEntity>?> UpdateAsync(TEntity entity, CancellationToken cancellation = default);

        /// <summary>
        /// Remove an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operations. The task result contains the entity entry of type `EntityEntry<TEntity>`, if it was removed; otherwise null.</returns>
        Task<EntityEntry<TEntity>?> RemoveAsync(TEntity entity, CancellationToken cancellation = default);

        /// <summary>
        /// Remove multiple entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operations. The task result contains the number of entities of type `TEntity` removed.</returns>
        Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellation = default);

        /// <summary>
        /// Get a paginated list of entities asynchronously.
        /// </summary>
        /// <param name="pageIndex">Optional. The index of the page to get.</param>
        /// <param name="pageSize">Optional. The size of the page to get. Defaults to 10.</param>
        /// <param name="filter">Optional. The filter to apply to the search.</param>
        /// <param name="orderBy">Optional. The order to apply to the search.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operations. The task result contains the paginated list of entities of type `TEntity`.</returns>
        Task<PaginatedList<TEntity>> GetPagedAsync(
            int pageIndex,
            int pageSize = 10,
            Expression<Func<TEntity, bool>> filter = default!,
            Func<IQueryable<TEntity>,
                 IOrderedQueryable<TEntity>> orderBy = default!,
            CancellationToken cancellation = default);

        /// <summary>
        /// Get a paginated list of entities by cursor asynchronously.
        /// </summary>
        /// <param name="lastCursor">The last cursor to use for the search.</param>
        /// <param name="pageSize">Optional. The size of the page to get. Defaults to 10.</param>
        /// <param name="filter">The filter to apply to the search.</param>
        /// <param name="orderBy">The order to apply to the search.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operations. The task result contains the cursor paginated list of entities of type `TEntity`.</returns>
        Task<CursorPaginatedList<TEntity>> GetPagedByCursorAsync(
            TKey lastCursor,
            int pageSize = 10,
            Expression<Func<TEntity, bool>> filter = default!,
            Func<IQueryable<TEntity>,
                 IOrderedQueryable<TEntity>> orderBy = default!,
            CancellationToken cancellation = default);

        /// <summary>
        /// Soft delete an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to soft delete.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operations. The task result contains the entity entry of type `EntityEntry<TEntity>`, if it was soft deleted; otherwise null.</returns>
        Task<EntityEntry<TEntity>?> SoftDeleteAsync(TEntity entity, CancellationToken cancellation = default);

        /// <summary>
        /// Undelete an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to soft delete.</param>
        /// <param name="cancellation">Optional. The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operations. The task result contains the entity entry, if it was soft deleted; otherwise null.</returns>
        /// <exception cref="InvalidOperationException">Thrown if entity of type `TEntity` does not support soft delete.</exception>
        public Task<EntityEntry<TEntity>?> SoftUndeleteAsync(TEntity entity, CancellationToken cancellation = default);

        /// <summary>
        /// Count the number of entities asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operations. The task result contains the number of entities of type `TEntity` in the repository.</returns>
        public Task<int> CountAsync(CancellationToken cancellation = default);

        /// <summary>
        /// Set the exception handler for the repository.
        /// </summary>
        void SetExceptionHandler(ExceptionHandler exceptionHandler);
    }
}
