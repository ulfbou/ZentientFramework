// <copyright file="IEnvelope.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions
{
    /// <summary>
    /// Represents a universal, protocol-agnostic code for an operation outcome or detail.
    /// This code provides a symbolic name and an optional numeric value.
    /// </summary>
    public interface ICode
    {
        /// <summary>
        /// Gets the symbolic name of the code (e.g., "NotFound", "ValidationError", "Success", "InsufficientFunds").
        /// This provides a human-readable or machine-interpretable identifier for the specific situation.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets an optional numeric value associated with the code (e.g., 404, 200, 1001 for a custom business error).
        /// This value is context-dependent and should not, at this abstraction level, imply a specific transport protocol (e.g., HTTP).
        /// </summary>
        int? Value { get; }
    }
}
