// <copyright file="MetadataExtensions.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Definitions;
using Zentient.Abstractions.Metadata.Readers;

namespace Zentient.Metadata.Extensions
{
    /// <summary>
    /// Provides a rich set of fluent, type-safe extension methods for working with <see cref="IMetadata"/>.
    /// These methods enhance developer ergonomics and enable intuitive interactions with the metadata system.
    /// </summary>
    public static class MetadataExtensions
    {
        #region Behavioral and Categorical Extensions

        /// <summary>
        /// Adds a behavior to the metadata.
        /// </summary>
        /// <typeparam name="TBehavior">The concrete behavior definition type, which must implement <see cref="IBehaviorDefinition"/>.</typeparam>
        /// <param name="metadata">The metadata instance to extend.</param>
        /// <returns>A new <see cref="IMetadata"/> instance with the added behavior.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> is null.</exception>
        public static IMetadata WithBehavior<TBehavior>(this IMetadata metadata)
            where TBehavior : IBehaviorDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.WithTag(typeof(TBehavior).FullName!, new TBehavior());
        }

        /// <summary>
        /// Checks if the metadata contains a specific behavior.
        /// </summary>
        /// <typeparam name="TBehavior">The concrete behavior definition type.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <returns>True if the behavior is present; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> is null.</exception>
        public static bool HasBehavior<TBehavior>(this IMetadata metadata)
            where TBehavior : IBehaviorDefinition
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.ContainsKey(typeof(TBehavior).FullName!);
        }

        /// <summary>
        /// Adds a category to the metadata.
        /// </summary>
        /// <typeparam name="TCategory">The concrete category definition type, which must implement <see cref="ICategoryDefinition"/>.</typeparam>
        /// <param name="metadata">The metadata instance to extend.</param>
        /// <returns>A new <see cref="IMetadata"/> instance with the added category.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> is null.</exception>
        public static IMetadata WithCategory<TCategory>(this IMetadata metadata)
            where TCategory : ICategoryDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.WithTag(typeof(TCategory).FullName!, new TCategory());
        }

        /// <summary>
        /// Checks if the metadata is associated with a specific category.
        /// </summary>
        /// <typeparam name="TCategory">The concrete category definition type.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <returns>True if the category is present; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> is null.</exception>
        public static bool HasCategory<TCategory>(this IMetadata metadata)
            where TCategory : ICategoryDefinition
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.ContainsKey(typeof(TCategory).FullName!);
        }

        #endregion

        #region Tag Extensions

        /// <summary>
        /// Adds or updates a tag on the metadata.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type, which must implement <see cref="IMetadataTagDefinition"/>.</typeparam>
        /// <typeparam name="TValue">The type of the tag's value.</typeparam>
        /// <param name="metadata">The metadata instance to extend.</param>
        /// <param name="value">The value of the tag.</param>
        /// <returns>A new <see cref="IMetadata"/> instance with the added or updated tag.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> is null.</exception>
        public static IMetadata WithTag<TTag, TValue>(this IMetadata metadata, TValue value)
            where TTag : IMetadataTagDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.WithTag(typeof(TTag).FullName!, value);
        }

        /// <summary>
        /// Adds a tag to the metadata only if a tag with the same key does not already exist.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type, which must implement <see cref="IMetadataTagDefinition"/>.</typeparam>
        /// <typeparam name="TValue">The type of the tag's value.</typeparam>
        /// <param name="metadata">The metadata instance to extend.</param>
        /// <param name="value">The value of the tag.</param>
        /// <returns>A new <see cref="IMetadata"/> instance if the tag was added; otherwise, the original instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> is null.</exception>
        public static IMetadata WithTagIfNotExists<TTag, TValue>(this IMetadata metadata, TValue value)
            where TTag : IMetadataTagDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var key = typeof(TTag).FullName!;
            if (metadata.ContainsKey(key))
            {
                return metadata;
            }
            return metadata.WithTag(key, value);
        }

        /// <summary>
        /// Gets the value of a tag in a type-safe manner.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type, which must implement <see cref="IMetadataTagDefinition"/>.</typeparam>
        /// <typeparam name="TValue">The expected type of the tag's value.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <param name="defaultValue">The value to return if the tag does not exist or has the wrong type.</param>
        /// <returns>The value of the tag, or the default value if not found or the type is incorrect.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> is null.</exception>
        public static TValue GetTagValue<TTag, TValue>(this IMetadata metadata, TValue defaultValue = default!)
            where TTag : IMetadataTagDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var key = typeof(TTag).FullName!;
            return metadata.GetValueOrDefault(key, defaultValue);
        }

        /// <summary>
        /// Attempts to get the value of a tag in a type-safe manner.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type, which must implement <see cref="IMetadataTagDefinition"/>.</typeparam>
        /// <typeparam name="TValue">The expected type of the tag's value.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified tag, if the tag is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>True if the metadata contains an element with the specified tag; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> is null.</exception>
        public static bool TryGetTagValue<TTag, TValue>(this IMetadata metadata, out TValue value)
            where TTag : IMetadataTagDefinition, new()
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var key = typeof(TTag).FullName!;
            return metadata.TryGetValue(key, out value!);
        }

        /// <summary>
        /// Checks if the metadata contains a specific tag.
        /// </summary>
        /// <typeparam name="TTag">The concrete tag definition type.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <returns>True if the tag is present; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> is null.</exception>
        public static bool HasTag<TTag>(this IMetadata metadata)
            where TTag : IMetadataTagDefinition
        {
            ArgumentNullException.ThrowIfNull(metadata);
            return metadata.ContainsKey(typeof(TTag).FullName!);
        }

        #endregion

        #region Merging and Traversal

        /// <summary>
        /// Merges tags from another <see cref="IMetadataReader"/> instance into the current metadata.
        /// </summary>
        /// <param name="metadata">The metadata instance to extend.</param>
        /// <param name="other">The metadata reader to merge from.</param>
        /// <returns>A new <see cref="IMetadata"/> instance with the merged tags.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> or <paramref name="other"/> is null.</exception>
        public static IMetadata WithMergedTags(this IMetadata metadata, IMetadataReader other)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentNullException.ThrowIfNull(other);
            return metadata.Merge(other);
        }

        /// <summary>
        /// Retrieves a nested metadata instance associated with a specific behavior.
        /// </summary>
        /// <typeparam name="TBehavior">The behavior definition type that defines the nested metadata.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <returns>The nested metadata instance if found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> is null.</exception>
        public static IMetadata? GetBehavior<TBehavior>(this IMetadata metadata)
            where TBehavior : IBehaviorDefinition
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var key = typeof(TBehavior).FullName!;
            if (metadata.TryGetValue(key, out var behavior))
            {
                return behavior as IMetadata;
            }
            return null;
        }

        /// <summary>
        /// Gets the value associated with the specified key, or a default value if the key does not exist or the value is of an incorrect type.
        /// </summary>
        /// <typeparam name="TValue">The expected type of the value.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defaultValue">The value to return if the key does not exist or the type is incorrect.</param>
        /// <returns>The value associated with the specified key, or the default value.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> or <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is an empty string.</exception>
        /// <remarks>
        /// This method enhances type safety by allowing developers to specify the expected type of the value.
        /// If the actual value cannot be cast to the specified type, the provided default value is returned instead.
        /// This approach helps prevent runtime errors due to invalid casts and provides a clear fallback mechanism.
        /// </remarks>
        public static TValue GetValueOrDefault<TValue>(this IMetadata metadata, string key, TValue defaultValue = default!)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            return metadata.TryGetValue(key, out var obj) && obj is TValue typedValue
                ? typedValue
                : defaultValue;
        }

        /// <summary>
        /// Attempts to get the value associated with the specified key in a type-safe manner.
        /// </summary>
        /// <typeparam name="TValue">The expected type of the value.</typeparam>
        /// <param name="metadata">The metadata instance to query.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found and the value is of the correct type; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>True if the metadata contains an element with the specified key and the value is of the correct type; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadata"/> or <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is an empty string.</exception>
        /// <remarks>
        /// This method enhances type safety by allowing developers to specify the expected type of the value.
        /// If the actual value cannot be cast to the specified type, the method returns false and sets the output parameter to the default value.
        /// This approach helps prevent runtime errors due to invalid casts and provides a clear mechanism for safely retrieving values.
        /// </remarks>
        public static bool TryGetValue<TValue>(this IMetadata metadata, string key, out TValue? value)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            if (metadata.TryGetValue(key, out var obj) && obj is TValue typedValue)
            {
                value = typedValue;
                return true;
            }

            value = default!;
            return false;
        }

        #endregion

    }
}
