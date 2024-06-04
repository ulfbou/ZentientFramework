//
// Class: AuditLog
//
// Description:
// The <see cref="AuditLog{TKey}"/> class represents an audit log entry for an entity. It provides a set of properties that describe the changes made to an entity, including the entity's primary key, type, operation, timestamp, user ID, and changes. The class is designed to be generic and flexible, allowing it to work with any entity type that implements the interface.
//
// Purpose:
// The purpose of the <see cref="AuditLog{TKey}"/> class is to provide a common set of properties for logging changes to entities. By defining a standard set of properties for the audit log entry, developers can write code that is more modular, flexible, and maintainable, leading to higher-quality software. The class also helps to decouple the data access logic from the rest of the application, making it easier to test and refactor the code in the future.
//
// Usage:
// The <see cref="AuditLog{TKey}"/> class is typically used in conjunction with repositories (<see cref="IRepository{TEntity, TKey}") to log changes to entities. Developers can create concrete implementations of the <see cref="AuditLog{TKey}"/> class for specific entity types, providing a consistent and reusable way to interact with the database. By using the audit log pattern, developers can write code that is more modular, flexible, and maintainable, leading to a more robust and scalable application.
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
    /// Represents an audit log entry for an entity.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
    public class AuditLog<TKey>
    {
        public int Id { get; set; }
        public TKey EntityId { get; set; }
        public string EntityType { get; set; }
        public string Operation { get; set; }
        public DateTime Timestamp { get; set; }
        public string Changes { get; set; }
    }
}
