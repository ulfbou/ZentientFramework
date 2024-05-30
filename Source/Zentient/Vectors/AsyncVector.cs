//
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
    private readonly ImmutableArray<T> _vector;

    public ImmutableArray<T> Vector { get => _vector; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSparseVector{T}"/> class with the specified source.
    /// </summary>
    /// <param name="source">The source representing the vector as <see cref="{T}[]">.</param>
    public AsyncVector(T[] vector)
    {
        ArgumentNullException.ThrowIfNull(vector, nameof(vector));

        _vector = [.. vector];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSparseVector{T}"/> class with the specified source.
    /// </summary>
    /// <param name="source">The source representing the vector as <see cref="ImmutableArray{T}">.</param>
    public AsyncVector(ImmutableArray<T> vector)
    {
        ArgumentNullException.ThrowIfNull(vector, nameof(vector));
        _vector = vector;
    }

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

        if (Vector.Length == 0)
        {
            return new AsyncVector<T>(other.Vector);
        }
        else if (other.Vector.Length == 0)
        {
            return new AsyncVector<T>(Vector);
        }
        else if (Vector.Length != other.Vector.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(other));
        }

        var producer1 = ProduceValuesAsync(Vector, cancellationToken);
        var producer2 = ProduceValuesAsync(other.Vector, cancellationToken);

        return await AdditionProducer(producer1, producer2, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously computes the dot product of this vector with another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The dot product of the two vectors.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the dimension of other does not match this vector.</exception>
    public async Task<T> DotProductAsync(
        AsyncVector<T> other,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(nameof(other));

        if (Vector.Length > 0 && other.Vector.Length > 0 && Vector.Length != other.Vector.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(other));
        }

        var producer1 = ProduceValuesAsync(_vector, cancellationToken);
        var producer2 = ProduceValuesAsync(other._vector, cancellationToken);

        return await DotProductProducer(producer1, producer2, cancellationToken).ConfigureAwait(false);
    }

    public override bool Equals(object? other)
    {
        if (other is null) return false;
        if (!(other is AsyncVector<T> otherVector)) return false;

        var vector1 = Vector;
        var vector2 = otherVector.Vector;

        if (vector1.Length != vector2.Length) return false;

        var producer1 = ProduceValuesAsync(vector1);
        var producer2 = ProduceValuesAsync(vector2);

        // Run the asynchronous method and wait for the result synchronously
        return Task.Run(async () => await EqualsAsync(producer1, producer2)).GetAwaiter().GetResult();
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

    private static async Task<bool> EqualsAsync(IAsyncEnumerable<T> producer1, IAsyncEnumerable<T> producer2)
    {
        await using var enumerator1 = producer1.GetAsyncEnumerator();
        await using var enumerator2 = producer2.GetAsyncEnumerator();

        var more1 = await enumerator1.MoveNextAsync();
        var more2 = await enumerator2.MoveNextAsync();

        while (more1 && more2)
        {
            if (!enumerator1.Current.Equals(enumerator2.Current)) return false;

            more1 = await enumerator1.MoveNextAsync();
            more2 = await enumerator2.MoveNextAsync();
        }

        return !(more1 || more2);
    }
}
