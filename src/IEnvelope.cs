// <copyright file="IEnvelope.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions
{
    /// <summary>
    /// Represents the minimal shape of a protocol-agnostic operation outcome envelope.
    /// </summary>
    public interface IEnvelope
    {
        /// <summary>
        /// Indicates whether the operation succeeded.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// A transport-agnostic code describing the result (e.g., "NotFound", "Aborted").
        /// </summary>
        IEndpointCode? Code { get; }

        /// <summary>
        /// Optional informational or diagnostic messages.
        /// </summary>
        IReadOnlyCollection<string>? Messages { get; }

        /// <summary>
        /// Optional metadata container for tags relevant to the result.
        /// </summary>
        IMetadata? Metadata { get; }
    }
}
