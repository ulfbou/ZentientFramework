//
// Class: IUnitOfWork
//
// Description:
// The IUnitOfWork interface represents a unit of work for a database context. It provides a set of methods for managing the overall data access and transactional behavior of an application, including saving changes to the database, beginning and committing transactions, and getting repositories for specific entity types. The interface is designed to be generic and flexible, allowing it to work with any database context that implements the <see cref="DbContext"/> class.
// 
// Purpose:
// The purpose of the IUnitOfWork interface is to provide a common set of methods for working with a database context in a consistent and reusable way. By defining a standard set of operations for managing transactions and repositories, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The interface also helps to decouple the data access logic from the rest of the application, making it easier to test and refactor the code in the future.
//
// Usage:
// The IUnitOfWork interface is typically used in conjunction with repositories (IRepository) to manage the overall data access and transactional behavior of an application. Developers can create concrete implementations of the IUnitOfWork interface for specific database contexts, providing a consistent and reusable way to interact with the database. By using the unit of work pattern, developers can write code that is more modular, flexible, and maintainable, leading to a more robust and scalable application.
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

namespace Zentient.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Get a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type of the repository.</typeparam>
        /// <typeparam name="TKey">The key type of the entity.</typeparam>
        public IRepository<T, TKey> GetRepository<T, TKey>() where T : class;

        /// <summary>
        /// Save all changes to the repositories.
        /// </summary>
        /// <returns>The number of state entries written to the database. </returns>
        public Task<int> SaveChangesAsync();

        /// <summary>
        /// Begin a transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if a transaction already has been activated.</exception>
        public void BeginTransaction();

        /// <summary>
        /// Commit the transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if a transaction has not been activated.</exception>
        public void CommitTransaction();

        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if a transaction has not been activated.</exception>
        public void RollbackTransaction();
    }
}
