// <copyright file="IEnvelope.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions
{
    /// <summary>
    /// Represents detailed, immutable information about an error,
    /// containing a structured code, a high-level category, a human-readable message,
    /// optional details, associated metadata, and support for nested errors.
    /// </summary>
    public interface IErrorInfo
    {
        /// <summary>
        /// Gets the high-level category of the error, providing a broad classification.
        /// </summary>
        ErrorCategory Category { get; }

        /// <summary>
        /// Gets the structured, protocol-agnostic code representing the specific error type.
        /// This property is never null.
        /// </summary>
        ICode Code { get; }

        /// <summary>Gets a human-readable message describing the error. This property is never null.</summary>
        string Message { get; }

        /// <summary>
        /// Gets optional, more verbose technical details or diagnostic information about the error. Can be null.
        /// </summary>
        string? Detail { get; }

        /// <summary>
        /// Gets optional, immutable metadata associated with this specific error instance.
        /// This property is never null; if no metadata is present, an empty IMetadata instance is returned.
        /// </summary>
        IMetadata Metadata { get; }

        /// <summary>
        /// Gets an immutable list of inner errors, allowing for hierarchical error reporting
        /// (e.g., for composite errors like validation failures).
        /// This property is never null; if no inner errors are present, an empty IReadOnlyList instance is returned.
        /// </summary>
        IReadOnlyList<IErrorInfo> InnerErrors { get; }
    }
}
