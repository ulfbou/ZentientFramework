﻿//
// Class: AsyncVector
//
// Description:
// Represents a dense vector with asynchronous dot product computation. This class provides functionality to handle dense vectors efficiently and compute dot products asynchronously, making it suitable for scenarios where dense vectors are prevalent, such as numerical computations and machine learning algorithms.
//
// Usage:
// To use the AsyncVector class, follow these steps:
// 1. Create an instance of the class by providing the data representing the dense vector.
// 2. Call the DotProductAsync method to compute the dot product with another dense vector asynchronously.
//
// Purpose:
// The purpose of the AsyncVector class is to provide a data structure for efficiently representing dense vectors and performing asynchronous dot product computations. By supporting asynchronous operations, the class enables efficient utilization of system resources and improved responsiveness in asynchronous programming scenarios.
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
public class AsyncVector<T> where T : struct, IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T>
{
    private readonly ImmutableArray<T> _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSparseVector{T}"/> class with the specified source.
    /// </summary>
    /// <param name="source">The source representing the vector as <see cref="{T}[]">.</param>
    public AsyncVector(T[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));

        _data = [.. data];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSparseVector{T}"/> class with the specified source.
    /// </summary>
    /// <param name="source">The source representing the vector as <see cref="ImmutableArray{T}">.</param>
    public AsyncVector(ImmutableArray<T> data) => _data = data;

    /// <summary>
    /// Asynchronously adds another sparse vector with this sparse vector.
    /// </summary>
    /// <param name="other">The other sparse vector to add.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A new <see cref="AsyncVector{T}"/> representing the result of the addition.</returns>
    public async Task<AsyncVector<T>> AddAsync(
        AsyncVector<T> other,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(nameof(other));

        var producer1 = ProduceValuesAsync(_data, cancellationToken);
        var producer2 = ProduceValuesAsync(other._data, cancellationToken);

        return await AdditionProducer(producer1, producer2, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously computes the dot product of this vector with another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The dot product of the two vectors.</returns>
    public async Task<T> DotProductAsync(
    AsyncVector<T> other,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(nameof(other));

        var producer1 = ProduceValuesAsync(_data, cancellationToken);
        var producer2 = ProduceValuesAsync(other._data, cancellationToken);

        return await DotProductProducer(producer1, producer2, cancellationToken).ConfigureAwait(false);
    }

    // Async method will run asynchronously since it returns IAsyncEnumerable<(int, T)>
#pragma warning disable CS1998
    private static async IAsyncEnumerable<T> ProduceValuesAsync(
#pragma warning restore CS1998
        ImmutableArray<T> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var value in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return value;
        }
    }

    private async Task<AsyncVector<T>> AdditionProducer(
        IAsyncEnumerable<T> producer1, 
        IAsyncEnumerable<T> producer2, 
        CancellationToken cancellationToken = default)
    {
        var resultDataBuilder = ImmutableArray.CreateBuilder<T>();

        try
        {
            await using var enumerator1 = producer1.GetAsyncEnumerator(cancellationToken);
            await using var enumerator2 = producer2.GetAsyncEnumerator(cancellationToken);

            var more1 = await enumerator1.MoveNextAsync();
            var more2 = await enumerator2.MoveNextAsync();

            while (more1 && more2)
            {
                cancellationToken.ThrowIfCancellationRequested();

                resultDataBuilder.Add(enumerator1.Current + enumerator2.Current);

                more1 = await enumerator1.MoveNextAsync();
                more2 = await enumerator2.MoveNextAsync();
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // TODO: Handle exceptions (optional)
            // Log or take appropriate action
            throw new InvalidOperationException("Error computing dot product.", ex);
        }

        return new AsyncVector<T>(resultDataBuilder.ToImmutable());
    }

    private static async Task<T> DotProductProducer(
        IAsyncEnumerable<T> producer1,
        IAsyncEnumerable<T> producer2,
        CancellationToken cancellationToken = default)
    {
        T result = default;

        try
        {
            var enumerator1 = producer1.GetAsyncEnumerator(cancellationToken);
            var enumerator2 = producer2.GetAsyncEnumerator(cancellationToken);

            var more1 = await enumerator1.MoveNextAsync();
            var more2 = await enumerator2.MoveNextAsync();

            while (more1 && more2)
            {
                cancellationToken.ThrowIfCancellationRequested();

                result += enumerator1.Current * enumerator2.Current;

                more1 = await enumerator1.MoveNextAsync();
                more2 = await enumerator2.MoveNextAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // TODO: Handle cancellation
        }
        catch (Exception ex)
        {
            // TODO: Handle exceptions (optional)
            // Log or take appropriate action
            throw new InvalidOperationException("Error computing dot product.", ex);
        }

        return result;
    }
}