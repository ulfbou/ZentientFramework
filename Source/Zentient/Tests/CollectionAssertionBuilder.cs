﻿//
// Class: CollectionAssertionBuilder
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

public partial class CollectionAssertionBuilder<T>(T subject, IComparer<T> comparer, IEqualityComparer<T> equality, string message)
    : ICollectionAssertionBuilder<T>
{
    private readonly T _subject = subject;
    private readonly IComparer<T> _comparer = comparer;
    private readonly IEqualityComparer<T> _equality = equality;
    private readonly string _message = message;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionAssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public CollectionAssertionBuilder(T subject, string message = "")
        : this(subject, DefaultComparers<T>.Comparer, DefaultComparers<T>.EqualityComparer, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionAssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public CollectionAssertionBuilder(T subject, IComparer<T> comparer, string message = "")
        : this(subject, comparer, DefaultComparers<T>.EqualityComparer, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionAssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public CollectionAssertionBuilder(T subject, IEqualityComparer<T> equality, string message = "")
        : this(subject, DefaultComparers<T>.Comparer, equality, message) { }

    public IComparer<T> Comparer => _comparer;
    public IEqualityComparer<T> EqualityComparer => _equality;

    public override bool Equals(object? obj) => _equality.Equals(_subject, (T?)obj);
    public override int GetHashCode() => base.GetHashCode();
    public int Compare(object? obj) => Comparer.Compare(_subject, (T?)obj);

    #region CollectionAssertions
    /// <summary>
    /// Validates if the count of the collection matches the expected count.
    /// </summary>
    /// <param name="expectedCount">The expected count of items in the collection.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> CountEquals(int expectedCount, string message = "")
    {
        if (_subject is ICollection<T> collection)
        {
            Assert.Pass(collection.Count() > expectedCount);
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection is empty.
    /// </summary>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IsEmpty(string message = "")
    {
        if (_subject is ICollection<T> collection)
        {
            Assert.Pass(collection.Count() == 0);
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection is not empty.
    /// </summary>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IsNotEmpty(string message = "")
    {
        if (_subject is ICollection<T> collection)
        {
            Assert.Pass(collection.Count() > 0);
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection contains a specific item.
    /// </summary>
    /// <param name="item">The item to check for presence in the collection.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> Contains(T item, string message = "")
    {
        if (_subject is ICollection<T> collection)
        {
            if (!collection.Contains(item)) throw new AssertionFailureException($"{_message}{message}");
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection does not contain a specific item.
    /// </summary>
    /// <param name="item">The item to check for absence in the collection.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> DoesNotContain(T item, string message = "")
    {
        if (_subject is ICollection<T> collection)
        {
            Assert.Fail(collection.Contains(item));
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection contains the same elements in the same order as another collection.
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> SequenceEquals(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(nameof(otherCollection));
        if (_subject is ICollection<T> thisCollection)
        {
            Assert.Pass(thisCollection.SequenceEqual(otherCollection));
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection is a subset of another collection. 
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IsSubsetOf(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(nameof(otherCollection));

        if (_subject is ICollection<T> thisCollection)
        {
            foreach (var item in thisCollection)
            {
                Assert.Pass(otherCollection.Contains(item));
            }

            return this;
        }

        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection is a superset of another collection.
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IsSupersetOf(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(nameof(otherCollection));

        if (_subject is ICollection<T> thisCollection)
        {
            foreach (var item in otherCollection)
            {
                Assert.Pass(thisCollection.Contains(item));
            }

            return this;
        }

        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if this collection has any common elements with another collection.
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IntersectsWith(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(nameof(otherCollection));

        if (_subject is ICollection<T> thisCollection)
        {
            if (thisCollection.Any(item => otherCollection.Contains(item))) return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if all items in the collection are unique.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasUniqueItems(string message = "")
    {
        if (_subject is ICollection<T> collection)
        {
            Assert.Pass(collection.GroupBy(item => item)
                .ToArray()
                .Any(group => group.Count() == 1));
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection contains duplicate items.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasDuplicates(string message = "")
    {
        if (_subject is ICollection<T> collection)
        {
            Assert.Pass(collection.GroupBy(item => item).ToArray().All(group => group.Count() > 1));
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection has an item at a specific index.
    /// </summary>
    /// <param name="index">The index to validate.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasItemAt(int index, string message = "")
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index, message);

        if (_subject is ICollection<T> collection)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<int>(index, collection.Count(), message);

            if (collection.ElementAt<T>(index) is null) throw new AssertionFailureException($"{_message}{message}");
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if two collections contain the same elements, ignoring the order of elements.
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AreEqualIgnoringOrder(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);

        if (_subject is ICollection<T> thisCollection)
        {
            Assert.Pass(thisCollection.Count() == otherCollection.Count());

            var collection1 = thisCollection.OrderBy(item => item, Comparer).Distinct();
            var collection2 = otherCollection.OrderBy(item => item).Distinct();

            Assert.Pass(collection1.SequenceEqual(collection2));
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if two collections have the same elements, regardless of their order and duplicates.
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AreEquivalent(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);
        if (_subject is ICollection<T> thisCollection)
        {
            Assert.Pass(thisCollection.Count() == otherCollection.Count());

            var collection1 = thisCollection.OrderBy(item => item).Distinct();
            var collection2 = otherCollection.OrderBy(item => item).Distinct();

            Assert.Pass(collection1.SequenceEqual(collection2));
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if two collections have no common elements.
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AreDisjoint(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);

        if (_subject is ICollection<T> thisCollection)
        {
            int count = thisCollection.Count() + otherCollection.Count();

            Assert.Pass(count != thisCollection.Concat(otherCollection).Count());
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection has a minimum length. 
    /// </summary>
    /// <param name="length">The length to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasMinLength(int length, string message = "")
    {
        if (_subject is ICollection<T> thisCollection)
        {
            Assert.Pass(thisCollection.Count() >= length);
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if the collection has a maximum length.
    /// </summary>
    /// <param name="length">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasMaxLength(int length, string message = "")
    {
        if (_subject is ICollection<T> thisCollection)
        {
            Assert.Pass(thisCollection.Count() <= length);
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if at least one item in the collection satisfies a specific condition.
    /// </summary>
    /// <param name="predicate">The predicate to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasItemSatisfying(Func<T, bool> predicate, string message = "")
    {
        if (_subject is ICollection<T> collection)
        {
            Assert.Pass(collection.Any(predicate));
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>
    /// Validates if all items in the collection satisfy a specific condition.
    /// </summary>
    /// <param name="predicate">The predicateto compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AllItemsSatisfy(Func<T, bool> predicate, string message = "")
    {
        if (_subject is ICollection<T> collection)
        {
            Assert.Pass(collection.Any(predicate));
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>throw
    /// Validates if two collections are equal based on a specific property of the items.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property used for comparison.</typeparam>
    /// <param name="otherCollection">The other collection to compare with.</param>
    /// <param name="selector">A function to extract the property value from each item in the collections.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AreEqualByProperty<TProperty>(
        ICollection<T> otherCollection,
        Func<T, TProperty> selector,
        string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);
        ArgumentNullException.ThrowIfNull(selector);

        if (_subject is ICollection<T> thisCollection)
        {
            var thisPropertyValues = thisCollection.Select(selector);
            var otherPropertyValues = otherCollection.Select(selector);

            Assert.Pass(thisPropertyValues.SequenceEqual(otherPropertyValues));
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }

    /// <summary>throw
    /// Validates if two collections are equal based on a specific property of the items.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property used for comparison.</typeparam>
    /// <param name="otherCollection">The other collection to compare with.</param>
    /// <param name="selector">A function to extract the property value from each item in the collections.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AreNotEqualByProperty<TProperty>(
        ICollection<T> otherCollection,
        Func<T, TProperty> selector,
        string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);
        ArgumentNullException.ThrowIfNull(selector);

        if (_subject is ICollection<T> thisCollection)
        {
            var thisPropertyValues = thisCollection.Select(selector);
            var otherPropertyValues = otherCollection.Select(selector);

            Assert.Pass(thisPropertyValues.SequenceEqual(otherPropertyValues));
            return this;
        }
        throw new AssertionFailureException($"{_message}{message}");
    }
    #endregion
}
