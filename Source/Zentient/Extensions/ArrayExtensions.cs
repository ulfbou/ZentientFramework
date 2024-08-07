//
// ArrayExtensions.cs
//
// Description: Provides extension methods for working with Arrays.
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

using System.Numerics;

namespace Zentient.Extensions
{
    /// <summary>
    /// Provides extension methods for working with Arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Randomly shuffles the elements of the array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to shuffle.</param>
        /// <returns>A array with its elements shuffled.</returns>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static IEnumerable<T> Shuffle<T>(this T[] array)
        {
            ArgumentNullException.ThrowIfNull(array, nameof(array));
            return array.OrderBy(x => Guid.NewGuid());
        }

        /// <summary>
        /// Splits the array into chunks of a specified size.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="chunkSize">The size of each chunk.</param>
        /// <returns>An array of arrays containing the chunks.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when chunkSize is less than one or equal or greater than <paramref name="array"/>.Length.</exception>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static T[][] Chunk<T>(this T[] array, int chunkSize)
        {
            ArgumentNullException.ThrowIfNull(array, nameof(array));
            ArgumentOutOfRangeException.ThrowIfLessThan<int>(chunkSize, 2, nameof(chunkSize));
            return array
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(g => g.Select(x => x.Value).ToArray()).ToArray();
        }

        /// <summary>
        /// Finds the index of the first element that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to search.</param>
        /// <param name="predicate">The condition to test each element against.</param>
        /// <returns>The index of the first element that satisfies the condition, or -1 if no such element is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="array"/> or <paramref name="predicate"/> is null.</exception>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static int IndexWhere<T>(this T[] array, Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(array, nameof(array));
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

            for (var i = 0; i < array.Length; i++)
            {
                if (predicate(array[i])) return i;
            }

            return -1;
        }

        /// <summary>
        /// Finds the index of the last element that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to search.</param>
        /// <param name="predicate">The condition to test each element against.</param>
        /// <returns>The index of the last element that satisfies the condition, or -1 if no such element is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="array"/> or <paramref name="predicate"/> is null.</exception>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static int LastIndexWhere<T>(this T[] array, Func<T, bool> predicate)
        {
            var i = array.Length;

            do
            {
                i--;
            }
            while (i >= 0 && !predicate(array[i]));

            return i;
        }

        /// <summary>
        /// Removes duplicate elements from the array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to remove duplicates from.</param>
        /// <returns>An array containing only the distinct elements of the original array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="array"/> is null.</exception>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static T[] Distinct<T>(this T[] array)
            => new HashSet<T>(array).ToArray<T>();

        /// <summary>
        /// Computes the sum of the elements in the array.
        /// </summary>
        /// <typeparam name="TSelf">The type of the elements in the array.</typeparam>
        /// <typeparam name="TOther">The type of the elements to be added to the array elements.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the addition operator.</typeparam>
        /// <param name="array">The array of integers.</param>
        /// <returns>The sum of the elements in the array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="array"/> is null.</exception>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static int Sum<TSelf, TOther, TResult>(this int[] array) where TSelf : struct, IAdditionOperators<TSelf, TOther, TResult>
        {
            ArgumentNullException.ThrowIfNull(array, nameof(array));
            return array.Aggregate((i, j) => i + j);
        }

        /// <summary>
        /// Computes the sum of the elements in the array.
        /// </summary>
        /// <typeparam name="TSelf">The type of the elements in the array.</typeparam>
        /// <typeparam name="TOther">The type of the elements to be added to the array elements.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the addition operator.</typeparam>
        /// <param name="array">The array of integers.</param>
        /// <returns>The sum of the elements in the array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="array"/> is null.</exception>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static int Sum<TTerm, TSum>(this int[] array) where TTerm : struct, IAdditionOperators<TTerm, TTerm, TSum>
        {
            ArgumentNullException.ThrowIfNull(array, nameof(array));
            return array.Aggregate((i, j) => i + j);
        }

        /// <summary>
        /// Computes the sum of the elements in the array.
        /// </summary>
        /// <typeparam name="TSelf">The type of the elements in the array.</typeparam>
        /// <typeparam name="TOther">The type of the elements to be added to the array elements.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the addition operator.</typeparam>
        /// <param name="array">The array of integers.</param>
        /// <returns>The sum of the elements in the array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="array"/> is null.</exception>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static int Sum<T>(this int[] array) where T : struct, IAdditionOperators<T, T, T>
        {
            ArgumentNullException.ThrowIfNull(array, nameof(array));
            return array.Aggregate((i, j) => i + j);
        }
    }
}
