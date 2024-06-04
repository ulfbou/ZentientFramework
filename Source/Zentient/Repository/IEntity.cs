// 
// Class: IEntity
//
// Description:
// The <see cref="IEntity{TKey}"/> interface represents an entity with a primary key. It provides a single property, Id, that represents the primary key of the entity. The interface is designed to be generic and flexible, allowing it to work with any entity type that implements the interface.
//
// Purpose:
// The purpose of the <see cref="IEntity{TKey}"/> interface is to provide a common set of methods for working with entities that have a primary key. By defining a standard property for the primary key, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The interface also helps to decouple the data access logic from the rest of the application, making it easier to test and refactor the code in the future.
//
// Usage:
// The <see cref="IEntity{TKey}"/> interface is typically used in conjunction with repositories (<see cref="IRepository{TEntity, TKey}") to represent entities that have a primary key. Developers can create concrete implementations of the <see cref="IEntity{TKey}"/> interface for specific entity types, providing a consistent and reusable way to interact with the database. By using the entity pattern, developers can write code that is more modular, flexible, and maintainable, leading to a more robust and scalable application.
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
    /// Represents an entity with a primary key.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}
