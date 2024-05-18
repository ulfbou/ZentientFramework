//
// EnumerableExtensions.cs
//
// Description: Provides extension methods for working with IEnumerables.
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

namespace Zentient.Extensions;

/// <summary>
/// Provides extension methods for working with IEnumerables.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Splits the sequence into smaller chunks of a specified size.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="chunkSize">The size of each chunk.</param>
    /// <returns>An enumerable of enumerable sequences containing the chunked elements.</returns>
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentOutOfRangeException.ThrowIfLessThan<int>(chunkSize, 1, nameof(chunkSize));
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(g => g.Select(x => x.Value).ToArray()).ToArray();
    }

    /// <summary>
    /// Returns distinct elements from a sequence based on a key selector.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key used for comparison.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key from each element.</param>
    /// <returns>An enumerable of distinct elements based on the key selector.</returns>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(keySelector, nameof(keySelector));
        return source.GroupBy(keySelector).Select(group => group.First());
    }

    /// <summary>
    /// Randomizes the order of elements in the sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>An enumerable with elements in random order.</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return source.OrderBy(x => Guid.NewGuid());
    }

    /// <summary>
    /// Batches the elements of the sequence into groups of similar elements, based on a specified key selector function.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key used to group elements.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function that extracts the key from an element.</param>
    /// <returns>An enumerable of enumerable sequences containing the batched elements.</returns>
    public static IEnumerable<IEnumerable<T>> Batch<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        IGrouping<T, TKey> x;
        // TODO: Implement the grouping logic using the provided keySelector
        var groups = source.GroupBy(keySelector);

        foreach(var item in groups)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Performs an action for each element in the sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="action">The action to perform on each element.</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }

    /// <summary>
    /// Flattens a sequence of sequences into a single sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence of sequences.</param>
    /// <returns>A single enumerable sequence containing all elements from the nested sequences.</returns>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source) =>
        source.SelectMany(x => x);

    /// <summary>
    /// Groups elements by multiple keys.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelectors">Functions to extract the keys from each element.</param>
    /// <returns>An enumerable of groups of elements based on the multiple keys.</returns>
    public static IEnumerable<IGrouping<object, T>> GroupByMany<T>(this IEnumerable<T> source, params Func<T, object>[] keySelectors) =>
        keySelectors.Aggregate(
            source.GroupBy(keySelectors.First()),
            (prev, keySelector) => prev.SelectMany(g => g.GroupBy(keySelector))
        );

    /// <summary>
    /// Takes the last N elements from the sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="count">The number of elements to take.</param>
    /// <returns>An enumerable containing the last N elements from the sequence.</returns>
    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count) =>
        source.Skip(Math.Max(0, source.Count() - count));

    /// <summary>
    /// Filters out null elements from the sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>An enumerable containing non-null elements from the source sequence.</returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source) =>
        source.Where(x => x != null);

    /// <summary>
    /// Combines two sequences into a single sequence of tuples.
    /// </summary>
    /// <typeparam name="TFirst">The type of elements in the first sequence.</typeparam>
    /// <typeparam name="TSecond">The type of elements in the second sequence.</typeparam>
    /// <param name="first">The first source sequence.</param>
    /// <param name="second">The second source sequence.</param>
    /// <returns>An enumerable of tuples containing elements from both sequences.</returns>
    public static IEnumerable<(TFirst, TSecond)> Zip<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second) =>
        first.Zip(second, (x, y) => (x, y));

    /// <summary>
    /// Partitions the sequence into two based on a predicate.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>A tuple containing two sequences: one with elements that satisfy the predicate and one with elements that do not.</returns>
    public static (IEnumerable<T>, IEnumerable<T>) Partition<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
        (source.Where(predicate), source.Where(x => !predicate(x)));

    /// <summary>
    /// Interleaves two sequences into a single sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequences.</typeparam>
    /// <param name="first">The first source sequence.</param>
    /// <param name="second">The second source sequence.</param>
    /// <returns>An enumerable sequence containing elements from both sequences interleaved.</returns>
    public static IEnumerable<T> Interleave<T>(this IEnumerable<T> first, IEnumerable<T> second) =>
        first.Zip(second, (x, y) => new[] { x, y }).Flatten();

    /// <summary>
    /// Finds the maximum element based on a key selector.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key used for comparison.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key from each element.</param>
    /// <returns>The maximum element based on the key selector.</returns>
    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
        source.OrderByDescending(keySelector).FirstOrDefault();

    /// <summary>
    /// Finds the minimum element based on a key selector.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key used for comparison.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key from each element.</param>
    /// <returns>The minimum element based on the key selector.</returns>
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
        source.OrderBy(keySelector).FirstOrDefault();
}
