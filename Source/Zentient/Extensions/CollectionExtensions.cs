//
// File: CollectionExtensions.cs
//
// Description: Provides extension methods for working with collections.
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

namespace Zentient.Extensions
{
    /// <summary>
    /// Provides extension methods for working with collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of the ICollection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
        /// <param name="collection">The ICollection to which the elements will be added.</param>
        /// <param name="items">The collection whose elements should be added to the end of the ICollection.</param>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the other array.
        /// </remarks>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Removes all elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
        /// <param name="collection">The ICollection from which the elements will be removed.</param>
        /// <param name="predicate">The delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the ICollection.</returns>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static int RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var removedItems = collection.Where(predicate);

            foreach (var item in removedItems)
            {
                collection.Remove(item);
            }

            return removedItems.Count();
        }

        /// <summary>
        /// Determines whether the ICollection is null or empty.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
        /// <param name="collection">The ICollection to check.</param>
        /// <returns>True if the ICollection is null or empty; otherwise, false.</returns>
        /// <remarks>
        /// This method has an average time complexity of O(1).
        /// </remarks>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
            => collection is null || collection.Count == 0;

        /// <summary>
        /// Joins the elements of the ICollection into a single string using a separator.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
        /// <param name="collection">The ICollection whose elements will be joined.</param>
        /// <param name="separator">The string to use as a separator.</param>
        /// <returns>A string that consists of the elements of the ICollection joined by the separator.</returns>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static string Join<T>(this ICollection<T> collection, string separator)
                => string.Join(separator, collection);

        /// <summary>
        /// Determines whether the ICollection contains all elements of another collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
        /// <param name="collection">The ICollection to check.</param>
        /// <param name="otherCollection">The collection whose elements should be checked for containment.</param>
        /// <returns>True if the ICollection contains all elements of the other collection; otherwise, false.</returns>
        /// <remarks>
        /// This method has an average time complexity of O(n + m ), where n, m is the number of elements in the arrays.
        /// </remarks>
        public static bool ContainsAll<T>(this ICollection<T> collection, IEnumerable<T> otherCollection)
            => otherCollection.All(collection.Contains);

        /// <summary>
        /// Swaps two elements at the specified indices in the ICollection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
        /// <param name="collection">The ICollection in which elements will be swapped.</param>
        /// <param name="index1">The index of the first element to swap.</param>
        /// <param name="index2">The index of the second element to swap.</param>
        //public static void Swap<T>(this ICollection<T> collection, int index1, int index2)
        //{
        //    if (index1 < 0 || index2 >= collection.Count()) throw new ArgumentOutOfRangeException($"{nameof(index1)} out of range.");
        //    if (index2 < 0 || index2 >= collection.Count()) throw new ArgumentOutOfRangeException($"{nameof(index2)} out of range.");

        //    if (index1 != index2)
        //    {
        //        collection.Swap(index1, index2);
        //        T temp = collection.ElementAt(index1);
        //        collection.Remove(temp);
        //        collection.
        //        //collection.Insert(index1, collection.ElementAt(index2));
        //    }
        //}

        /// <summary>
        /// Returns a random element from the ICollection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
        /// <param name="collection">The ICollection from which to select a random element.</param>
        /// <returns>A random element from the ICollection.</returns>
        public static T RandomElement<T>(this ICollection<T> collection)
        {
            if (collection.IsNullOrEmpty())
            {
                throw new InvalidOperationException("The collection is empty.");
            }

            var randomIndex = new Random().Next(collection.Count);
            return collection.ElementAt(randomIndex);
        }

        /// <summary>
        /// Removes duplicate elements from the ICollection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
        /// <param name="collection">The ICollection from which to remove duplicate elements.</param>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static void Distinct<T>(this ICollection<T> collection)
        {
            HashSet<T> set = new HashSet<T>(collection);
            collection.Clear();

            foreach (var item in set)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Removes the last element from the ICollection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
        /// <param name="collection">The ICollection from which to remove the last element.</param>
        /// <exception cref="InvalidOperationException">Thrown when the ICollection is null or empty.</exception>
        /// <remarks>
        /// This method has an average time complexity of O(n), where n is the number of elements in the array.
        /// </remarks>
        public static void RemoveLast<T>(this ICollection<T> collection)
        {
            if (collection.IsNullOrEmpty()) throw new InvalidOperationException("Cannot remove an item from an empty collection.");

            collection.Remove(collection.Last());
        }

        /// <summary>
        /// Copies the elements of the ICollection to an array without allocating a new array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
        /// <param name="collection">The ICollection from which to copy elements.</param>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ICollection.</param>
        /// <param name="index">The zero-based index in the array at which copying begins.</param>
        public static void CopyToNonAlloc<T>(this ICollection<T> collection, T[] array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (index < 0 || index >= array.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if (array.Length - index < collection.Count) throw new ArgumentException("The destination array is not large enough to copy all the elements.");

            int i = index;

            foreach (var item in collection)
            {
                array[i++] = item;
            }
        }
    }
}
