//
// Class: DictionaryExtensions
//
// Description:
// This class provides extension methods for working with dictionaries.
//
// Purpose: 
// This class provides extension methods for working with dictionaries.
//
// Usage:
// This class is used to extend the functionality of dictionaries.
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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Generate useful, versatile and efficient extension methods for dictionaries.

namespace Zentient.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Determines whether the dictionary is empty
        /// </summary>
        /// <returns>True if the dictionary is empty; otherwise, false.</returns>
        public static bool IsEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) where TKey : notnull
        {
            return !dictionary.Any();
        }

        /// <summary>
        /// Gets the value associated with the specified key or a default value if the key is not found.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="key">The key to fetch, if it exists. </param>
        /// <param name="defaultValue">The default value to use, if the key does not exist.</param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
            where TKey : notnull
        {
            return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary if the key does not already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="key">The key to add or update.</param>
        /// <param name="value">The value to be added or updated.</param>
        /// <returns>True, if a new key/value pair was added to the dictionary. Otherwise, false.</returns>
        public static bool AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
            where TKey : notnull
        {
            bool added = false;
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
                added = true;
            }
            return added;
        }

        /// <summary>
        /// Merges the other dictionary to this dictionary, optionally overwriting existing keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="other">The dictionary to merge into this dictionary.</param>
        /// <param name="overwriteExisting">True to overwrite existing keys; otherwise, false.</param>
        /// <returns>The dictionary after the merge operation.</returns>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            Dictionary<TKey, TValue> other,
            bool overwriteExisting = true)
            where TKey : notnull
        {
            foreach (var kvp in other)
            {
                if (overwriteExisting || !dictionary.ContainsKey(kvp.Key))
                {
                    dictionary[kvp.Key] = kvp.Value;
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Removes all elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="predicate">The delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the dictionary.</returns>
        public static int RemoveWhere<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<TKey, TValue, bool> predicate)
            where TKey : notnull
        {
            var keysToRemove = dictionary.Where(kvp => predicate(kvp.Key, kvp.Value)).Select(kvp => kvp.Key).ToList();
            int removedCount = 0;
            foreach (var key in keysToRemove)
            {
                if (dictionary.Remove(key))
                {
                    removedCount++;
                }
            }
            return removedCount;
        }

        /// <summary>
        /// Creates a new dictionary by projecting the values of the original dictionary. The keys remains unchanged. 
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <typeparam name="TResult">The type of the projected values.</typeparam>
        /// <param name="selector">A projection method to apply to each value in the dictionary.</param>
        /// <returns>A new dictionary with the projected values.</returns>
        public static Dictionary<TKey, TResult> ToDictionaryByValue<TKey, TValue, TResult>(
            this Dictionary<TKey, TValue> dictionary,
            Func<TValue, TResult> selector) where TKey : notnull
        {
            return dictionary.ToDictionary(kvp => kvp.Key, kvp => selector(kvp.Value));
        }

        /// <summary>
        /// Creates a new dictionary by inverting the keys and values of the original dictionary. 
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <returns>A new dictionary with the inverted keys.</returns>
        public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull where TValue : notnull
        {
            return dictionary.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        /// <summary>
        /// Gets the value associated with the specified key or adds a new value if the key is not found.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="key">The key to fetch, if it exists. </param>
        /// <param name="valueFactory">The factory method to create a new value if the key does not exist.</param>
        /// <returns>The value associated with the key or the new value created by the factory method.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        {
            if (!dictionary.TryGetValue(key, out TValue? value))
            {
                value = valueFactory();
                dictionary.Add(key, value);
            }

            return value;
        }

        /// <summary>
        /// Gets a list of the keys in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <returns>A list of the values in the dictionary.</returns>
        public static List<TKey> KeysAsList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
        {
            return dictionary.Keys.ToList();
        }

        /// <summary>
        /// Gets a list of the values in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <returns>A list of the values in the dictionary.</returns>
        public static List<TValue> ValuesAsList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) where TKey : notnull
        {
            return dictionary.Values.ToList();
        }

        /// <summary>
        /// Clones the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <returns>A new dictionary with the same keys and values as the original dictionary.</returns>
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) where TKey : notnull
        {
            return new Dictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Determines whether the dictionary contains any of the specified keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="keys">The keys to check for.</param>
        /// <returns>True if the dictionary contains any of the specified keys; otherwise, false.</returns>
        public static bool ContainsAnyKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys) where TKey : notnull
        {
            return keys.Any(key => dictionary.ContainsKey(key));
        }

        /// <summary>
        /// Determines whether the dictionary contains any of the specified keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="keys">The keys to check for.</param>
        /// <returns>True if the dictionary contains any of the specified keys; otherwise, false.</returns>
        public static bool ContainsAnyKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, params TKey[] keys) where TKey : notnull
        {
            return keys.Any(key => dictionary.ContainsKey(key));
        }

        /// <summary>
        /// Determines whether the dictionary contains any of the specified keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="keys">The keys to check for.</param>
        /// <returns>True if the dictionary contains any of the specified keys; otherwise, false.</returns>
        public static bool ContainsAllKeys<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys) where TKey : notnull
        {
            return keys.All(key => dictionary.ContainsKey(key));
        }

        /// <summary>
        /// Determines whether the dictionary contains any of the specified keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="keys">The keys to check for.</param>
        /// <returns>True if the dictionary contains any of the specified keys; otherwise, false.</returns>
        public static bool ContainsAllKeys<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, params TKey[] keys) where TKey : notnull
        {
            return keys.All(key => dictionary.ContainsKey(key));
        }

        /// <summary>
        /// Get all keys associated with a specific value.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="value">The value to search for.</param>
        /// <returns>An enumerable collection of keys associated with the specified value.</returns>
        public static IEnumerable<TKey> GetKeysByValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value) where TKey : notnull
        {
            return dictionary.Where(pair => EqualityComparer<TValue>.Default.Equals(pair.Value, value)).Select(pair => pair.Key);
        }

        /// <summary>
        /// Converts a dictionary to a multi-value dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <returns>A multi-value dictionary.</returns>
        public static Dictionary<TKey, List<TValue>> ToMultiValueDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> pairs) where TKey : notnull
        {
            var multiValueDictionary = new Dictionary<TKey, List<TValue>>();
            foreach (var pair in pairs)
            {
                if (!multiValueDictionary.ContainsKey(pair.Key))
                {
                    multiValueDictionary[pair.Key] = new List<TValue>();
                }
                multiValueDictionary[pair.Key].Add(pair.Value);
            }
            return multiValueDictionary;
        }

        /// <summary>
        /// Tries to add a key-value pair to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>True if the key-value pair was added; otherwise, false.</returns>
        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a range of key-value pairs to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="pairs">The key-value pairs to add.</param>
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
            where TKey : notnull
        {
            foreach (var pair in pairs)
            {
                dictionary.Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Converts a dictionary to an immutable dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <returns>An immutable dictionary.</returns>
        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
        {
            return ImmutableDictionary.CreateRange(dictionary);
        }

        /// <summary>
        /// Filters a dictionary by Key/Value pairs.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="predicate">The predicate to filter by.</param>
        /// <returns>A new dictionary containing only the key-value pairs that satisfy the predicate.</returns>
        public static Dictionary<TKey, TValue> FilterByValue<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            Func<TKey, TValue, bool> predicate)
            where TKey : notnull
        {
            return dictionary.Where(pair => predicate(pair.Key, pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Filters a dictionary by value.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="predicate">The predicate to filter by.</param>
        /// <returns>A new dictionary containing only the key-value pairs that satisfy the predicate.</returns>
        public static Dictionary<TKey, TValue> FilterByValue<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            Func<TValue, bool> predicate)
            where TKey : notnull
        {
            return dictionary.Where(pair => predicate(pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Filters a dictionary by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <param name="predicate">The predicate to filter by.</param>
        /// <returns>A new dictionary containing only the key-value pairs that satisfy the predicate.</returns>
        public static Dictionary<TKey, TValue> FilterByKey<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            Func<TKey, bool> predicate)
            where TKey : notnull
        {
            return dictionary.Where(pair => predicate(pair.Key)).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Returns the key-value pair with the maximum value.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <returns>The key-value pair with the maximum value.</returns>
        public static KeyValuePair<TKey, TValue> MaxByValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull where TValue : IComparable<TValue>
        {
            return dictionary.Aggregate((maxPair, pair) => pair.Value.CompareTo(maxPair.Value) > 0 ? pair : maxPair);
        }

        /// <summary>
        /// Returns the key-value pair with the minimum value.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue"The type of the values in the dictionary.</typeparam>
        /// <returns>The key-value pair with the minimum value.</returns>
        public static KeyValuePair<TKey, TValue> MinByValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull where TValue : IComparable<TValue>
        {
            return dictionary.Aggregate((minPair, pair) => pair.Value.CompareTo(minPair.Value) < 0 ? pair : minPair);
        }
    }
}
