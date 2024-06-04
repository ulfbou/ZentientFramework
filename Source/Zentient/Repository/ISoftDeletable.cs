// Generate professional documentation for the IRepository interface following in the same style as the current documentation. Provide additional documentation whereever you see fit. provide full xml support in the documentaiton including <see cref="MyModel{TEntity, TKey}"/>. 
//
// Class: ISoftDeletable
//
// Description:
// The <see cref="ISoftDeletable{TKey}"/> interface represents an entity that can be soft-deleted. It provides a single property, IsDeleted, that represents whether the entity has been soft-deleted. The interface is designed to be generic and flexible, allowing it to work with any entity type that implements the interface.
//
// Purpose:
// The purpose of the <see cref="ISoftDeletable{TKey}"/> interface is to provide a common set of methods for working with entities that can be soft-deleted. By defining a standard property for the soft-deleted flag, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The interface also helps to decouple the data access logic from the rest of the application, making it easier to test and refactor the code in the future.
//
// Usage:
// The <see cref="ISoftDeletable{TKey}"/> interface is typically used in conjunction with repositories (<see cref="IRepository{TEntity, TKey}") to represent entities that can be soft-deleted. Developers can create concrete implementations of the <see cref="ISoftDeletable{TKey}"/> interface for specific entity types, providing a consistent and reusable way to interact with the database. By using the soft-deletable pattern, developers can write code that is more modular, flexible, and maintainable, leading to a more robust and scalable application.
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

namespace Zentient.Repository
{
    /// <summary>
    /// Represents an entity that can be soft-deleted.
    /// </summary>
    /// <typeparam name="TKey">The type that enables soft-deletion.</typeparam>
    public interface ISoftDeletable<TKey> : IEntity<TKey>
    {
        bool IsDeleted { get; set; }
    }
}
