//
// Interface: ICollectionAssertionBuilder
//
// Description:
// The Assert class provides a fluent API with a set of static methods for making assertions in unit tests. These methods allow developers to validate the behavior and output of code under test, ensuring that it meets the expected criteria.
// 
// Usage:
// The Assert class is commonly used within unit testing frameworks to verify the behavior of code under test. Developers use these assertion methods, part of a fluent API, to validate various aspects of the code's output, behavior, and state during testing.
// 
// Purpose:
// The purpose of the Assert class is to provide a convenient and expressive way for developers to write unit tests and make assertions about the behavior and output of their code. By using these assertion methods, which are part of a fluent API, developers can chain together multiple assertions in a readable and concise manner, ensuring that their code behaves as expected under different conditions and scenarios, leading to more robust and reliable software.
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


namespace Zentient.Tests;
public interface ICollectionAssertionBuilder<T> : IAssertionBuilder<ICollection<T>>
{
    /// <summary>
    /// Validates if the count of the collection matches the expected count.
    /// </summary>
    /// <param name="expectedCount">The expected count of items in the collection.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    ICollectionAssertionBuilder<T> CountEquals(int expectedCount, string message = "");

    /// <summary>
    /// Validates if the collection is empty.
    /// </summary>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    ICollectionAssertionBuilder<T> IsEmpty(string message = "");

    /// <summary>
    /// Validates if the collection is not empty.
    /// </summary>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    ICollectionAssertionBuilder<T> IsNotEmpty(string message = "");

    /// <summary>
    /// Validates if the collection contains a specific item.
    /// </summary>
    /// <param name="item">The item to check for presence in the collection.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    ICollectionAssertionBuilder<T> Contains(T item, string message = "");

    /// <summary>
    /// Validates if the collection does not contain a specific item.
    /// </summary>
    /// <param name="item">The item to check for absence in the collection.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    ICollectionAssertionBuilder<T> DoesNotContain(T item, string message = "");

    /// <summary>
    /// Validates if the collection contains the same elements in the same order as another collection.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    ICollectionAssertionBuilder<T> SequenceEquals(ICollection<T> collection, string message = "");

    /// <summary>
    /// Validates if the collection is a subset of another collection. 
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IsSubsetOf(ICollection<T> collection, string message = "");

    /// <summary>
    /// Validates if the collection is a superset of another collection.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IsSupersetOf(ICollection<T> collection, string message = "");

    /// <summary>
    /// Validates if the collection has any common elements with another collection.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IntersectsWith(ICollection<T> collection, string message = "");

    /// <summary>
    /// Validates if all items in the collection are unique.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasUniqueItems(string message = "");

    /// <summary>
    /// Validates if the collection contains duplicate items.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasDuplicates(string message = "");

    /// <summary>
    /// Validates if the collection has an item at a specific index.
    /// </summary>
    /// <param name="index">The index to validate</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasItemAt(int index, string message = "");

    /// <summary>
    /// Validates if two collections contain the same elements, ignoring the order of elements.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AreEqualIgnoringOrder(ICollection<T> collection, string message = "");

    /// <summary>
    /// Validates if two collections have the same elements, regardless of their order and duplicates.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AreEquivalent(ICollection<T> collection, string message = "");

    /// <summary>
    /// Validates if two collections have no common elements.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AreDisjoint(ICollection<T> collection, string message = "");

    /// <summary>
    /// Validates if the collection has a minimum length. 
    /// </summary>
    /// <param name="length">The length to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasMinLength(int length, string message = "");

    /// <summary>
    /// Validates if the collection has a maximum length.
    /// </summary>
    /// <param name="length">The length to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasMaxLength(int length, string message = "");

    /// <summary>
    /// Validates if at least one item in the collection satisfies a specific condition.
    /// </summary>
    /// <param name="predicate">The predicate to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasItemSatisfying(Func<T, bool> predicate, string message = "");

    /// <summary>
    /// Validates if all items in the collection satisfy a specific condition.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AllItemsSatisfy(Func<T, bool> predicate, string message = "");

    /// <summary>
    /// Validates if two collections are equal based on a specific property of the items.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="selector">The selector to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AreEqualByProperty<TProperty>(
        ICollection<T> collection,
        Func<T, TProperty> selector,
        string message = "");
}
