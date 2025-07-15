// <copyright file="IEnvelope.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions
{
    /// <summary>
    /// Represents a runtime execution context for the current operation.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Gets the logical name or type of the current context (e.g., "http", "grpc").
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets metadata relevant to the context.
        /// </summary>
        IMetadata? Metadata { get; }
    }
}
