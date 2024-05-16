//
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

using Newtonsoft.Json.Linq;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Zentient.Tests;

public partial class CollectionAssertionBuilder<T> :
    AssertionBuilderBase<ICollection<T>>, ICollectionAssertionBuilder<T> where T : notnull
{
    private readonly IComparer<T> _comparer;
    private readonly IEqualityComparer<T> _equality;

    public CollectionAssertionBuilder(
        ICollection<T> actual,
        IComparer<T> comparer,
        IEqualityComparer<T> equality,
        string message)
        : base(actual, message)
    {
        _comparer = comparer;
        _equality = equality;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionAssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public CollectionAssertionBuilder(ICollection<T> subject, string message = "")
        : this(subject,
              DefaultComparers<T>.Comparer,
              DefaultComparers<T>.EqualityComparer,
              message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionAssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public CollectionAssertionBuilder(
        ICollection<T> subject, IComparer<T> comparer, string message = "")
        : this(subject, comparer, DefaultComparers<T>.EqualityComparer, message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionAssertionBuilder{T}"/> class.
    /// </summary>
    /// <param name="subject">The subject to be asserted.</param>
    public CollectionAssertionBuilder(
        ICollection<T> subject, IEqualityComparer<T> equality, string message = "")
        : this(subject, DefaultComparers<T>.Comparer, equality, message)
    { }

    #region CollectionAssertions
    public int Compare(T? actual, T? expected) => _comparer.Compare(actual, expected);
    public bool Equals(T? actual, T? expected) => _equality.Equals(actual, expected);

    /// <summary>
    /// Validates if the count of the collection matches the expected count.
    /// </summary>
    /// <param name="expectedCount">The expected count of items in the collection.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> CountEquals(int expectedCount, string message = "")
    {
        Assert.Pass(_actual.Count() == expectedCount);
        return this;
    }

    /// <summary>
    /// Validates if the collection is empty.
    /// </summary>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IsEmpty(string message = "")
    {
        Assert.Pass(_actual.Count() == 0);
        return this;
    }

    /// <summary>
    /// Validates if the collection is not empty.
    /// </summary>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IsNotEmpty(string message = "")
    {
        Assert.Pass(_actual.Count() > 0);
        return this;
    }

    /// <summary>
    /// Validates if the collection contains a specific item.
    /// </summary>
    /// <param name="item">The item to check for presence in the collection.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> Contains(T item, string message = "")
    {
        if (!_actual.Contains(item, _equality)) throw new AssertionFailureException($"{_message}{message}");
        return this;
    }

    /// <summary>
    /// Validates if the collection does not contain a specific item.
    /// </summary>
    /// <param name="item">The item to check for absence in the collection.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> DoesNotContain(T item, string message = "")
    {
        Assert.Fail(_actual.Contains(item, _equality));
        return this;
    }

    /// <summary>
    /// Validates if the collection contains the same elements in the same order as another collection.
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> SequenceEquals(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);

        Assert.Pass(_actual.SequenceEqual(otherCollection, _equality));
        return this;

    }

    /// <summary>
    /// Validates if the collection is a subset of another collection. 
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IsSubsetOf(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);

        foreach (var item in _actual)
        {
            Assert.Pass(otherCollection.Contains(item, _equality));
        }

        return this;
    }

    /// <summary>
    /// Validates if the collection is a superset of another collection.
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IsSupersetOf(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);

        foreach (var item in otherCollection)
        {
            Assert.Pass(_actual.Contains(item, _equality));
        }

        return this;
    }

    /// <summary>
    /// Validates if this collection has any common elements with another collection.
    /// </summary>
    /// <param name="otherCollection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> IntersectsWith(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);

        if (_actual.Any(item => otherCollection.Contains(item, _equality))) return this;
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
        Assert.Pass(_actual.Distinct().Count() == _actual.Count());
        return this;
    }

    /// <summary>
    /// Validates if the collection contains duplicate items.
    /// </summary>
    /// <param name="collection">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasDuplicates(string message = "")
    {
        Assert.Pass(_actual.Distinct().Count() < _actual.Count());
        return this;
    }

    /// <summary>
    /// Validates if the collection has an item at a specific index.
    /// </summary>
    /// <param name="index">The index to validate.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasItemAt(int index, string message = "")
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<int>(index, _actual.Count());
        Assert.Fail(_actual.ElementAt<T>(index) is null);
        return this;
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
        Assert.Pass(_actual.Count == otherCollection.Count, message);

        var actualOrdered = _actual.OrderBy(item => item, _comparer);
        var expectedOrdered = otherCollection.OrderBy(item => item, _comparer);

        Assert.Pass(actualOrdered.SequenceEqual(expectedOrdered), message);
        return this;
    }
    public ICollectionAssertionBuilder<T> AreEqualIgnoringOrdera(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);
        Assert.Pass(_actual.Count == otherCollection.Count, message);

        var actual = _actual.OrderBy(item => item, _comparer);
        var expected = otherCollection.OrderBy(item => item, _comparer);

        Assert.Pass(actual.SequenceEqual(expected));
        return this;
    }
    public ICollectionAssertionBuilder<T> AreEqualIgnoringOrder2(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);
        Assert.Pass(_actual.Count == otherCollection.Count, message);

        //Sort both collections using OrderBy and then use the Zip method to merge the sorted collections into pairs. Check each pair for equality, and if any pair is not equal, the collections are not equal.
        var actual = _actual.OrderBy(item => item, _comparer);
        var expected = otherCollection.OrderBy(item => item, _comparer);
        var zipped = actual.Zip(expected);
        var res = (zipped.Where((x, y) => Equals(x, y)));

        Assert.Pass(zipped.Count() == res.Count());
        return this;
    }
    public ICollectionAssertionBuilder<T> AreEqualIgnoringOrder3(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);
        Assert.Pass(_actual.Count == otherCollection.Count, message);

        // Sort both collections using OrderBy and then compare the count of each collection. If the counts are different, the collections are not equal. Otherwise, loop through each collection and ensure each item in one collection is found in the other collection.

        return this;
    }
    public ICollectionAssertionBuilder<T> AreEqualIgnoringOrder4(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);
        Assert.Pass(_actual.Count == otherCollection.Count, message);

        var actual = _actual.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var expected = otherCollection.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        Assert.Pass(!(actual.Except(expected).Any() || expected.Except(actual).Any()));

        return this;
    }
    public ICollectionAssertionBuilder<T> AreEqualIgnoringOrder5(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);
        Assert.Pass(_actual.Count == otherCollection.Count, message);

        // Use GroupBy and Count to create a collection of IGrouping<T, int> containing each unique item and its count in each collection.
        // Convert each grouped collection into a Dictionary<T, int> containing items as keys and their counts as values.
        // Create HashSet<KeyValuePair<T, int>> from the Dictionary<T, int> in each collection.
        // Compare the two HashSet<KeyValuePair< T, int>> for equality using the SetEquals method.

        return this;
    }
    public ICollectionAssertionBuilder<T> AreEqualIgnoringOrder6(ICollection<T> otherCollection, string message = "")
    {
        ArgumentNullException.ThrowIfNull(otherCollection);
        Assert.Pass(_actual.Count == otherCollection.Count, message);

        // Create dictionaries to store element counts
        var actualCounts = _actual.GroupBy(item => item, _equality)
                                  .ToDictionary(g => g.Key, g => g.Count(), _equality);
        var otherCounts = otherCollection.GroupBy(item => item, _equality)
                                         .ToDictionary(g => g.Key, g => g.Count(), _equality);

        // Check if the dictionaries have the same counts for each element
        Assert.Pass(actualCounts.Count == otherCounts.Count &&
                    actualCounts.All(kv => otherCounts.TryGetValue(kv.Key, out int count) && count == kv.Value), message);

        return this;
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
        Assert.Pass(_actual.Count() == otherCollection.Count());

        var collection1 = _actual.OrderBy(item => item).Distinct();
        var collection2 = otherCollection.OrderBy(item => item).Distinct();

        Assert.Pass(collection1.SequenceEqual(collection2));
        return this;
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

        int count = _actual.Count() + otherCollection.Count();

        Assert.Pass(count != _actual.Concat(otherCollection).Count());
        return this;
    }

    /// <summary>
    /// Validates if the collection has a minimum length. 
    /// </summary>
    /// <param name="length">The length to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasMinLength(int length, string message = "")
    {
        Assert.Pass(_actual.Count() >= length);
        return this;
    }

    /// <summary>
    /// Validates if the collection has a maximum length.
    /// </summary>
    /// <param name="length">The collection to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasMaxLength(int length, string message = "")
    {
        Assert.Pass(_actual.Count() <= length);
        return this;
    }

    /// <summary>
    /// Validates if at least one item in the collection satisfies a specific condition.
    /// </summary>
    /// <param name="predicate">The predicate to compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> HasItemSatisfying(Func<T, bool> predicate, string message = "")
    {
        Assert.Pass(_actual.Any(predicate));
        return this;
    }

    /// <summary>
    /// Validates if all items in the collection satisfy a specific condition.
    /// </summary>
    /// <param name="predicate">The predicateto compare with.</param>
    /// <param name="message">An optional custom error message to include in case of assertion failure.</param>
    /// <returns>The instance of the collection assertion builder for method chaining.</returns>
    public ICollectionAssertionBuilder<T> AllItemsSatisfy(Func<T, bool> predicate, string message = "")
    {
        Assert.Pass(_actual.All(predicate));
        return this;
    }

    /// <summary>
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

        var thisPropertyValues = _actual.Select(selector);
        var otherPropertyValues = otherCollection.Select(selector);

        Assert.Pass(thisPropertyValues.SequenceEqual(otherPropertyValues));
        return this;
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

        var thisPropertyValues = _actual.Select(selector);
        var otherPropertyValues = otherCollection.Select(selector);

        Assert.Pass(thisPropertyValues.SequenceEqual(otherPropertyValues));
        return this;
    }
    #endregion
}
