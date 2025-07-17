// <copyright file="IEnvelope.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions
{
    /// <summary>
    /// Factory for creating root <see cref="IContext"/> instances and managing ambient context.
    /// </summary>
    public interface IContextFactory
    {
        /// <summary>
        /// Creates a new root <see cref="IContext"/> instance with the specified type, optional initial metadata, and optional correlation ID.
        /// </summary>
        /// <param name="type">The logical type or name of the root context (e.g., "HttpRequest", "BackgroundTask").</param>
        /// <param name="initialMetadata">Optional initial metadata to associate with the context.</param>
        /// <param name="correlationId">Optional correlation ID for distributed tracing. If null, a new one may be generated.</param>
        /// <returns>A new root <see cref="IContext"/> instance.</returns>
        IContext CreateRoot(string type, IMetadata? initialMetadata = null, string? correlationId = null);

        /// <summary>
        /// Gets an empty, default <see cref="IContext"/> instance representing the absence of context.
        /// </summary>
        IContext Empty { get; }

        /// <summary>
        /// Gets the current ambient <see cref="IContext"/> for the executing logical flow (e.g., via <c>AsyncLocal</c>).
        /// </summary>
        IContext Current { get; }

        /// <summary>
        /// Sets the specified <see cref="IContext"/> as the current ambient context for the duration of the returned <see cref="IDisposable"/> scope.
        /// </summary>
        /// <param name="context">The context to set as current.</param>
        /// <returns>An <see cref="IDisposable"/> that, when disposed, restores the previous context.</returns>
        IDisposable SetCurrent(IContext context);
    }
}
