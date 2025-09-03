// <copyright file="src/Zentient.Metadata/Builders/MetadataBuilders.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Readers;

namespace Zentient.Metadata.Internal
{
    /// <summary>
    /// A concrete, immutable implementation of <see cref="IMetadata"/>.
    /// This is the final, read-only metadata object produced by the builder.
    /// </summary>
    internal sealed class MetadataImpl : IMetadata
    {
        private readonly IReadOnlyDictionary<string, object?> _tags;

        public MetadataImpl(IDictionary<string, object?>? tags = null)
        {
            _tags = new Dictionary<string, object?>(tags ?? new Dictionary<string, object?>());
        }

        // --- IMetadataReader Implementation (via Composition) ---
        public int Count => _tags.Count;
        public IEnumerable<string> Keys => _tags.Keys;
        public IEnumerable<object?> Values => _tags.Values;
        public IEnumerable<KeyValuePair<string, object?>> Tags => _tags;

        public bool ContainsKey(string key)
        {
            Guard.AgainstNullOrWhitespace(key, nameof(key));
            return _tags.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object? value) => _tags.TryGetValue(key, out value);

        public bool TryGetValue<TValue>(string key, [MaybeNullWhen(false)] out TValue value)
        {
            Guard.AgainstNullOrWhitespace(key, nameof(key));
            if (_tags.TryGetValue(key, out var objValue) && objValue is TValue typedValue)
            {
                value = typedValue;
                return true;
            }
            value = default;
            return false;
        }

        public object? GetValueOrDefault(string key, object? defaultValue = null)
        {
            return _tags.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public TValue GetValueOrDefault<TValue>(string key, TValue defaultValue = default!)
        {
            Guard.AgainstNullOrWhitespace(key, nameof(key));
            if (_tags.TryGetValue(key, out var value) && value is TValue typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        // --- IMetadata Implementation (Immutability) ---
        public IMetadata WithTag(string key, object? value)
        {
            var newTags = new Dictionary<string, object?>(_tags);
            newTags[key] = value;
            return new MetadataImpl(newTags);
        }

        public IMetadata WithoutTag(string key)
        {
            if (!_tags.ContainsKey(key))
                return this;
            var newTags = new Dictionary<string, object?>(_tags);
            newTags.Remove(key);
            return new MetadataImpl(newTags);
        }

        public IMetadata WithTag(Func<KeyValuePair<string, object?>> tagFactory)
        {
            var newTag = tagFactory();
            return WithTag(newTag.Key, newTag.Value);
        }

        public IMetadata WithTags(IEnumerable<KeyValuePair<string, object?>> tags)
        {
            var newTags = new Dictionary<string, object?>(_tags);
            foreach (var tag in tags)
            {
                newTags[tag.Key] = tag.Value;
            }
            return new MetadataImpl(newTags);
        }

        public IMetadata WithTags(Func<IEnumerable<KeyValuePair<string, object?>>> tagsFactory)
        {
            var tags = tagsFactory();
            return WithTags(tags);
        }

        public IMetadata WithoutTags(IEnumerable<string> keys)
        {
            var keySet = keys as ICollection<string> ?? keys.ToArray();
            if (keySet.Count == 0)
                return this;

            var hashSet = keySet is HashSet<string> hs ? hs : new HashSet<string>(keySet);
            var newTags = new Dictionary<string, object?>(_tags);
            foreach (var key in hashSet)
            {
                newTags.Remove(key);
            }
            return new MetadataImpl(newTags);
        }

        public IMetadata WithoutTag(Func<string, bool> keyPredicate)
        {
            var newTags = new Dictionary<string, object?>(_tags);
            var keysToRemove = _tags.Keys.Where(keyPredicate).ToList();
            foreach (var key in keysToRemove)
            {
                newTags.Remove(key);
            }
            return new MetadataImpl(newTags);
        }

        public IMetadata Merge(IMetadataReader other)
        {
            var newTags = new Dictionary<string, object?>(_tags);
            foreach (var tag in other.Tags)
            {
                newTags[tag.Key] = tag.Value;
            }
            return new MetadataImpl(newTags);
        }
    }
}
