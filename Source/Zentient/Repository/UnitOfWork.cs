//
// Class: UnitOfWork
//
// Description:
// The <see cref="UnitOfWork"/> class represents a unit of work for a database context. It provides a set of methods for managing the overall data access and transactional behavior of an application, including saving changes to the database, beginning and committing transactions, and getting repositories for specific entity types. The class is designed to be generic and flexible, allowing it to work with any database context that implements the <see cref="DbContext"/> class.
//
// Purpose:
// The purpose of the <see cref="UnitOfWork"/> class is to provide a common set of methods for working with a database context in a consistent and reusable way. By defining a standard set of operations for managing transactions and repositories, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The class also helps to decouple the data access logic from the rest of the application, making it easier to test and refactor the code in the future.
//
// Usage:
// The <see cref="UnitOfWork"/> class is typically used in conjunction with repositories (<see cref="IRepository{TEntity, TKey}") to manage the overall data access and transactional behavior of an application. Developers can create concrete implementations of the <see cref="UnitOfWork"/> class for specific database contexts, providing a consistent and reusable way to interact with the database. By using the unit of work pattern, developers can write code that is more modular, flexible, and maintainable, leading to a more robust and scalable application.
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
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using Zentient.Core;

namespace Zentient.Repository
{
    /// <summary>
    /// Represents a unit of work for a database context.
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private readonly DbContext _context;
        private readonly IMapper _mapper;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<Type, object> _repositories;
        private readonly ExceptionHandler? _exceptionHandler;
        private IDbContextTransaction? _transaction = null;
        private bool _disposed;

        /// <summary>
        /// Constructs a new unit of work.
        /// </summary>
        /// <param name="context">The database context for the unit of work.</param>
        /// <param name="mapper">Optional. The mapper for the unit of work.</param>
        /// <param name="loggerFactory">Optional. The logger factory for the unit of work.</param>
        /// <param name="exceptionHandler">The exception handler for the unit of work.</param>
        public UnitOfWork(DbContext context, IMapper? mapper = null, ILoggerFactory? loggerFactory = null, ExceptionHandler? exceptionHandler = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            _repositories = new ConcurrentDictionary<Type, object>();
        }

        /// <summary>
        /// Get a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type of the repository.</typeparam>
        /// <typeparam name="TKey">The key type of the entity.</typeparam>
        public IRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : class, IEntity<TKey>
            where TKey : struct
        {
            var type = typeof(TEntity);
            return (IRepository<TEntity, TKey>)_repositories.GetOrAdd(type, _ =>
            {
                ILogger<TEntity>? _logger = _loggerFactory.CreateLogger<TEntity>();
                if (_loggerFactory is not null)
                {
                    _logger = _loggerFactory.CreateLogger<TEntity>();
                }

                return new RepositoryBase<TEntity, TKey>(_context, _mapper, _logger, _exceptionHandler);
            });
        }

        /// <summary>
        /// Register a repository for the specified entity type. 
        /// </summary>
        /// <typeparam name="TEntity">The entity type of the repository.</typeparam>
        /// <typeparam name="TKey">The key type of the entity.</typeparam>
        /// <param name="implementationType">The implementation type of the repository.</param>
        /// <exception cref="ArgumentNullException">Thrown if the implementation type is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the repository could not be created or added.</exception>
        public void RegisterRepository<TEntity, TKey>(Type implementationType)
            where TEntity : class, IEntity<TKey>
            where TKey : struct
        {
            ArgumentNullException.ThrowIfNull(implementationType, nameof(implementationType));

            var serviceEntityType = typeof(IRepository<TEntity, TKey>);
            // add specific type checks or constraints to ensure that the implementationType is indeed a type that can be assigned to IRepository<TEntity, TKey>.
            if (!serviceEntityType.IsAssignableFrom(implementationType))
            {
                throw new InvalidOperationException($"The implementation type {implementationType.Name} does not implement {serviceEntityType.Name}.");
            }

            try
            {
                var repository = Activator.CreateInstance(implementationType, _context, _exceptionHandler);

                if (repository is null)
                {
                    throw new InvalidOperationException($"Repository for {serviceEntityType.Name} could not be created.");
                }

                if (!_repositories.TryAdd(serviceEntityType, repository))
                {
                    throw new InvalidOperationException($"Repository for {serviceEntityType.Name} could not be added.");
                }
            }
            catch (Exception ex)
            {
                if (_exceptionHandler is not null) _exceptionHandler(ex);
            }
        }

        /// <summary>
        /// Save all changes to the repositories.
        /// </summary>
        /// <returns>The number of state entries written to the database. </returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellation = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellation);
            }
            catch (Exception ex)
            {
                if (_exceptionHandler is not null) await _exceptionHandler(ex, cancellation);
                throw;
            }
        }

        /// <summary>
        /// Begin a transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if a transaction already has been activated.</exception>
        public async Task BeginTransactionAsync(CancellationToken cancellation = default)
        {
            if (_transaction is not null) throw new InvalidOperationException("A transaction has already been activated.");

            _transaction = await _context.Database.BeginTransactionAsync(cancellation);
        }

        /// <summary>
        /// Commit the transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if a transaction has not been activated.</exception>
        public async Task<int> CommitTransactionAsync(CancellationToken cancellation = default)
        {
            if (_transaction is null) throw new InvalidOperationException("Transaction has not been activated.");

            try
            {
                var result = await _context.SaveChangesAsync(cancellation);
                await _transaction.CommitAsync(cancellation);

                return result;
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync(cancellation);
                if (_exceptionHandler is not null) await _exceptionHandler(ex);
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if a transaction has not been activated.</exception>
        public async Task RollbackTransactionAsync(CancellationToken cancellation = default)
        {
            if (_transaction is null) throw new InvalidOperationException("Transaction has not been started.");

            await _transaction.RollbackAsync(cancellation);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                    _transaction?.Dispose();
                }
                _disposed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            if (_context is IAsyncDisposable asyncDisposableContext)
            {
                await asyncDisposableContext.DisposeAsync();
            }
            else
            {
                _context.Dispose();
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

