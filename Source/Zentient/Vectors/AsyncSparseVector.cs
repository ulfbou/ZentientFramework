//
// Class: AsyncSparseVector
//
// Description:
// Represents a sparse vector with asynchronous dot product computation. This class provides functionality to handle sparse vectors efficiently and compute dot products asynchronously, making it suitable for scenarios where sparse vectors are prevalent, such as numerical computations and machine learning algorithms.
//
// Usage:
// To use the AsyncSparseVector class, follow these steps:
// 1. Create an instance of the class by providing the data representing the sparse vector.
// 2. Call the DotProductAsync method to compute the dot product with another sparse vector asynchronously.
//
// Purpose:
// The purpose of the AsyncSparseVector class is to provide a data structure for efficiently representing sparse vectors and performing asynchronous dot product computations. By supporting asynchronous operations, the class enables efficient utilization of system resources and improved responsiveness in asynchronous programming scenarios.
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

using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Zentient.Vectors;

/// <summary>
/// Represents a sparse vector with asynchronous dot product computation.
/// </summary>
/// <typeparam name="T">The type of elements in the vector.</typeparam>
public class AsyncSparseVector<T> where T : struct, IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T>
{
    private readonly ImmutableDictionary<int, T> _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSparseVector{T}"/> class with the specified data.
    /// </summary>
    /// <param name="data">The data representing the sparse vector as key-value pairs.</param>
    public AsyncSparseVector(ImmutableDictionary<int, T> data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <summary>
    /// Asynchronously adds another sparse vector with this sparse vector.
    /// </summary>
    /// <param name="other">The other sparse vector to add.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A new AsyncSparseVector representing the result of the addition.</returns>
    public async Task<AsyncSparseVector<T>> AddAsync(AsyncSparseVector<T> other, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(other);

        var resultDataBuilder = ImmutableDictionary.CreateBuilder<int, T>();

        var producer1 = ProduceValuesAsync(_data, cancellationToken);
        var producer2 = ProduceValuesAsync(other._data, cancellationToken);

        await foreach (var (index, value) in AdditionProducers(producer1, producer2, cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            resultDataBuilder[index] = value;
        }

        return new AsyncSparseVector<T>(resultDataBuilder.ToImmutable());
    }

    /// <summary>
    /// Asynchronously computes the dot product of this sparse vector with another sparse vector.
    /// </summary>
    /// <param name="other">The other sparse vector.</param>
    /// <returns>The dot product of the two sparse vectors.</returns>
    public async Task<T> DotProductAsync(AsyncSparseVector<T> other, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(other);

        var producer1 = ProduceValuesAsync(_data, cancellationToken);
        var producer2 = ProduceValuesAsync(other._data, cancellationToken);

        return await DotProductProducer(producer1, producer1);
    }

    // Async method will run asynchronously since it returns IAsyncEnumerable<(int, T)>
#pragma warning disable CS1998
    private async IAsyncEnumerable<(int, T)> ProduceValuesAsync(
#pragma warning restore CS1998
        ImmutableDictionary<int, T> data,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var kvp in data)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return (kvp.Key, kvp.Value);
        }
    }

    private async IAsyncEnumerable<(int, T)> AdditionProducers(
        IAsyncEnumerable<(int, T)> producer1,
        IAsyncEnumerable<(int, T)> producer2,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var enumerator1 = producer1.GetAsyncEnumerator(cancellationToken);
        await using var enumerator2 = producer2.GetAsyncEnumerator(cancellationToken);

        var more1 = await enumerator1.MoveNextAsync();
        var more2 = await enumerator2.MoveNextAsync();

        while (more1 || more2)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (more1 && more2)
            {
                var (index1, value1) = enumerator1.Current;
                var (index2, value2) = enumerator2.Current;

                if (index1 == index2)
                {
                    yield return (index1, value1 + value2);
                    more1 = await enumerator1.MoveNextAsync();
                    more2 = await enumerator2.MoveNextAsync();
                }
                else if (index1 < index2)
                {
                    yield return (index1, value1);
                    more1 = await enumerator1.MoveNextAsync();
                }
                else
                {
                    yield return (index2, value2);
                    more2 = await enumerator2.MoveNextAsync();
                }
            }
            else if (more1)
            {
                var (index1, value1) = enumerator1.Current;
                yield return (index1, value1);
                more1 = await enumerator1.MoveNextAsync();
            }
            else if (more2)
            {
                var (index2, value2) = enumerator2.Current;
                yield return (index2, value2);
                more2 = await enumerator2.MoveNextAsync();
            }
        }
    }

    private async Task<T> DotProductProducer(
        IAsyncEnumerable<(int, T)> producer11,
        IAsyncEnumerable<(int, T)> producer12,
        CancellationToken cancellationToken = default)
    {
        T result = default;

        try
        {
            await using var enumerator1 = producer11.GetAsyncEnumerator(cancellationToken);
            await using var enumerator2 = producer12.GetAsyncEnumerator(cancellationToken);
            var more1 = await enumerator1.MoveNextAsync();
            var more2 = await enumerator2.MoveNextAsync();

            while (more1 && more2)
            {
                cancellationToken.ThrowIfCancellationRequested();
                (int index1, T value1) = enumerator1.Current;
                (int index2, T value2) = enumerator2.Current;

                if (index1 == index2)
                {
                    result += value1 * value2;
                    more1 = await enumerator1.MoveNextAsync();
                    more2 = await enumerator2.MoveNextAsync();
                }
                else if (index1 < index2)
                {
                    more1 = await enumerator1.MoveNextAsync();
                }
                else
                {
                    more2 = await enumerator2.MoveNextAsync();
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Handle exceptions (optional)
            // Log or take appropriate action
            throw new InvalidOperationException("Error computing dot product.", ex);
        }

        return result;
    }
}
