﻿//
// Class: CursorPaginatedList
//
// Description:
// The <see cref="CursorPaginatedList{TEntity}"/> class represents a paginated list of items with a cursor. It is a generic class that takes an entity type as a type parameter and inherits from the <see cref="List{T}"/> class. The class provides properties for the last cursor, page size, and total pages, as well as a method to create a new instance of the class asynchronously.
//
// Purpose:
// The purpose of the <see cref="CursorPaginatedList{T}"/> class is to provide a common set of methods for working with a paginated list of items with a cursor. By defining a standard set of operations for managing pagination, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The class also helps to decouple the data access logic from the rest of the application, making it easier to test and refactor the code in the future.
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
    /// Represents a paginated list of items with a cursor.
    /// </summary>
    /// <typeparam name="TEntity">The entity type of the paginated list.</typeparam>
    public class CursorPaginatedList<TEntity> : List<TEntity>
    {
        public object LastCursor { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages => (int)Math.Ceiling((double)this.Count() / PageSize);

        private CursorPaginatedList(IEnumerable<TEntity> items, object lastCursor, int pageSize) : base(items)
        {
            LastCursor = lastCursor;
            PageSize = pageSize;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CursorPaginatedList{TEntity}"/> class asynchronously.
        /// </summary>
        /// <param name="source">The queryable source of entities to paginate.</param>
        /// <param name="lastCursor">The last cursor value from the previous page.</param>
        /// <param name="pageSize">Optional. The number of entities per page. Defaults to 1.</param>
        /// <returns>A new instance of the <see cref="CursorPaginatedList{TEntity}"/> class.</returns>
        public static async Task<CursorPaginatedList<TEntity>> CreateAsync(IQueryable<TEntity> source, object lastCursor, int pageSize = 1)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));
            ArgumentNullException.ThrowIfNull(lastCursor, nameof(lastCursor));
            ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1, nameof(pageSize));
            var items = await source.Take(pageSize).ToListAsync();
            return new CursorPaginatedList<TEntity>(items, lastCursor, pageSize);
        }
    }
}
