// <copyright file="IEnvelope.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions
{
    /// <summary>
    /// Represents structured contextual metadata with key-based access.
    /// </summary>
    public interface IMetadata
    {
        /// <summary>
        /// Gets the set of available tag keys.
        /// </summary>
        IReadOnlyCollection<string> Keys { get; }

        /// <summary>
        /// Gets the set of available tag values.
        /// </summary>
        IReadOnlyCollection<object> Values { get; }

        /// <summary>
        /// Attempts to retrieve a metadata tag of type <typeparamref name="TValue"/> by key.
        /// </summary>
        /// <typeparam name="TValue">The type of the tag value.</typeparam>
        /// <param name="key">The key for the tag.</param>
        /// <param name="value">When this method returns, contains the tag value if found; otherwise, <see langword="null"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the tag was found and successfully cast to <typeparamref name="TValue"/>; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
        bool TryGetTag<TValue>(string key, out TValue? value);

        /// <summary>
        /// Returns a new metadata instance with the given tag added or replaced.
        /// </summary>
        /// <typeparam name="TValue">The type of the tag value.</typeparam>
        /// <param name="key">The key for the tag.</param>
        /// <param name="value">The value to associate with the key.</param>
        /// <returns>A new <see cref="IMetadata"/> instance with the updated tag.</returns>
        IMetadata WithTag<TValue>(string key, TValue value);
    }
}
