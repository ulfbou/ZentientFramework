// 
// Class: PaginatedList
//
// Description:
// The <see cref="PaginatedList{T}"/> class represents a list of entities that have been paginated. It is a generic class that takes an entity type as a type parameter and inherits from the <see cref="List{T}"/> class. The class provides properties for the page index, total pages, and whether there is a previous or next page, as well as a method to create a new instance of the class asynchronously.
// 
// Purpose:
// The purpose of the <see cref="PaginatedList{T}"/> class is to provide a common set of methods for working with a list of entities that have been paginated. By defining a standard set of operations for managing pagination, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The class also helps to decouple the data access logic from the rest of the application, making it easier to test and refactor the code in the future.
//
// Usage:
// The <see cref="PaginatedList{T}"/> class is typically used in conjunction with repositories (<see cref="IRepository{TEntity, TKey}") to manage the overall data access and transactional behavior of an application. Developers can create concrete implementations of the <see cref="PaginatedList{T}"/> class for specific entity types, providing a consistent and reusable way to interact with the database. By using the paginated list pattern, developers can write code that is more modular, flexible, and maintainable, leading to a more robust and scalable application.
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
    /// <summary>
    /// Represents a list of entities that have been paginated.
    /// </summary>
    /// <typeparam name="TEntity">The entity type of the paginated list.</typeparam>
    public class PaginatedList<TEntity> : List<TEntity>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        /// <summary>
        /// Constructs a new paginated list.
        /// </summary>
        /// <param name="items">The list of entities to paginate.</param>
        /// <param name="count">The total number of entities in the list.</param>
        /// <param name="pageIndex">The current page index.</param>
        /// <param name="pageSize">The number of entities per page.</param>
        private PaginatedList(IEnumerable<TEntity> items, int count, int pageIndex, int pageSize) : base(items)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        /// <summary>
        /// Returns whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// Returns whether there is a next page.
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// Creates a new paginated list asynchronously.
        /// </summary>
        /// <param name="source">The source queryable to paginate.</param>
        /// <param name="pageIndex">The current page index.</param>
        /// <param name="pageSize">The number of entities per page.</param>
        /// <returns>A paginated list of entities.</returns>
        public static async Task<PaginatedList<TEntity>> CreateAsync(IQueryable<TEntity> source, int pageIndex = 0, int pageSize = 1)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));
            ArgumentOutOfRangeException.ThrowIfNegative(pageIndex, nameof(pageIndex));
            ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1, nameof(pageSize));
            var count = await source.CountAsync();
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(pageIndex, count, nameof(pageIndex));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(pageSize, count, nameof(pageSize));
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<TEntity>(items, count, pageIndex, pageSize);
        }
    }
}
