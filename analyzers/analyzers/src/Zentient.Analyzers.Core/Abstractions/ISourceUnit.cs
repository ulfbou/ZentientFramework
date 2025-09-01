// <copyright file="src/Zentient.Analyzers/Abstractions/ISourceUnit.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Represents a unit of generated source code.
    /// </summary>
    public interface ISourceUnit
    {
        /// <summary>
        /// Gets the source code content.
        /// </summary>
        string Content { get; }

        /// <summary>
        /// Gets the fingerprint of the source unit.
        /// </summary>
        string Fingerprint { get; }

        /// <summary>
        /// Gets the name of the source unit.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the provenance information for the source unit.
        /// </summary>
        string Provenance { get; }

        /// <summary>
        /// Deconstructs the source unit into its components.
        /// </summary>
        /// <param name="Name">The name of the source unit.</param>
        /// <param name="Content">The content of the source unit.</param>
        /// <param name="Provenance">The provenance information.</param>
        /// <param name="Fingerprint">The fingerprint.</param>
        void Deconstruct(out string Name, out string Content, out string Provenance, out string Fingerprint);

        /// <summary>
        /// Determines whether the specified object is equal to the current source unit.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if equal; otherwise, false.</returns>
        bool Equals(object? obj);

        /// <summary>
        /// Determines whether the specified source unit is equal to the current source unit.
        /// </summary>
        /// <param name="other">The source unit to compare.</param>
        /// <returns>True if equal; otherwise, false.</returns>
        bool Equals(ISourceUnit? other);

        /// <summary>
        /// Gets the hash code for the source unit.
        /// </summary>
        /// <returns>The hash code.</returns>
        int GetHashCode();

        /// <summary>
        /// Returns a string representation of the source unit.
        /// </summary>
        /// <returns>The string representation.</returns>
        string ToString();
    }
}
