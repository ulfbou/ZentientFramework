namespace Zentient.Abstractions.DependencyInjection.Resolution
{
    /// <summary>
    /// Represents an async-first, DI-agnostic resolver for retrieving services from the container.
    /// </summary>
    /// <remarks>
    /// This is the primary abstraction for consumers to resolve services asynchronously. It 
    /// provides result-based resolution patterns with full support for strongly-typed contextual 
    /// resolution, metadata-driven queries, and graceful error handling.
    /// All operations are asynchronous to support modern async/await patterns and
    /// enable sophisticated resolution scenarios including remote service discovery.
    /// </remarks>
    public interface IServiceResolver
    {
        /// <summary>Gets the factory for creating new service scopes.</summary>
        IServiceScopeFactory ScopeFactory { get; }

        // === Core Resolution Methods ===

        /// <summary>
        /// Resolves a single instance of the specified contract type asynchronously.
        /// </summary>
        /// <typeparam name="TContract">The contract type to resolve.</typeparam>
        /// <typeparam name="TContextDefinition">The specific context definition type.</typeparam>
        /// <param name="predicate">
        /// Optional function to filter service descriptors based on metadata and context.
        /// </param>
        /// <param name="context">Optional strongly-typed context for resolution.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task containing the result with the resolved service or error information.
        /// </returns>
        Task<IResult<TContract>> Resolve<TContract, TContextDefinition>(
            Func<IServiceDescriptor, IContext<TContextDefinition>?, bool>? predicate = null,
            IContext<TContextDefinition>? context = null,
            CancellationToken cancellationToken = default)
            where TContextDefinition : IContextDefinition;

        /// <summary>
        /// Resolves a single instance of the specified contract type asynchronously.
        /// Simplified overload for cases without strongly-typed context requirements.
        /// </summary>
        /// <typeparam name="TContract">The contract type to resolve.</typeparam>
        /// <param name="predicate">
        /// Optional function to filter service descriptors based on metadata.
        /// </param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task containing the result with the resolved service or error information.
        /// </returns>
        Task<IResult<TContract>> Resolve<TContract>(
            Func<IServiceDescriptor, bool>? predicate = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves all registered instances of the specified contract asynchronously.
        /// </summary>
        /// <typeparam name="TContract">The contract type to resolve.</typeparam>
        /// <typeparam name="TContextDefinition">The specific context definition type.</typeparam>
        /// <param name="predicate">
        /// Optional function to filter service descriptors based on metadata and context.
        /// </param>
        /// <param name="context">Optional strongly-typed context for resolution.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An asynchronous enumerable of all resolved instances.</returns>
        IAsyncEnumerable<TContract> ResolveMany<TContract, TContextDefinition>(
            Func<IServiceDescriptor, IContext<TContextDefinition>?, bool>? predicate = null,
            IContext<TContextDefinition>? context = null,
            CancellationToken cancellationToken = default)
            where TContextDefinition : class, IContextDefinition;

        /// <summary>
        /// Resolves all registered instances of the specified contract asynchronously.
        /// Simplified overload for cases without strongly-typed context requirements.
        /// </summary>
        /// <typeparam name="TContract">The contract type to resolve.</typeparam>
        /// <param name="predicate">
        /// Optional function to filter service descriptors based on metadata.
        /// </param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An asynchronous enumerable of all resolved instances.</returns>
        IAsyncEnumerable<TContract> ResolveMany<TContract>(
            Func<IServiceDescriptor, bool>? predicate = null,
            CancellationToken cancellationToken = default);

        // === Non-Generic Overloads for Dynamic Scenarios ===

        /// <summary>
        /// Resolves a service instance of the specified type asynchronously.
        /// </summary>
        /// <typeparam name="TContextDefinition">The specific context definition type.</typeparam>
        /// <param name="contractType">The contract type to resolve.</param>
        /// <param name="predicate">
        /// Optional function to filter service descriptors based on metadata and context.
        /// </param>
        /// <param name="context">Optional strongly-typed context for resolution.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task containing the result with the resolved service or error information.
        /// </returns>
        Task<IResult<object>> Resolve<TContextDefinition>(
            Type contractType,
            Func<IServiceDescriptor, IContext<TContextDefinition>?, bool>? predicate = null,
            IContext<TContextDefinition>? context = null,
            CancellationToken cancellationToken = default)
            where TContextDefinition : class, IContextDefinition;

        /// <summary>
        /// Resolves a service instance of the specified type asynchronously.
        /// Simplified overload for cases without strongly-typed context requirements.
        /// </summary>
        /// <param name="contractType">The contract type to resolve.</param>
        /// <param name="predicate">
        /// Optional function to filter service descriptors based on metadata.
        /// </param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task containing the result with the resolved service or error information.
        /// </returns>
        Task<IResult<object>> ResolveAsync(
            Type contractType,
            Func<IServiceDescriptor, bool>? predicate = null,
            CancellationToken cancellationToken = default);
    }
}
