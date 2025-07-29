// <copyright file="IEnvelope.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions
{
    /// <summary>
    /// Factory for creating common policy instances and combinators for operation execution.
    /// </summary>
    public interface IPolicyFactory
    {
        /// <summary>
        /// Creates a retry policy that retries the operation up to the specified number of times on failure.
        /// </summary>
        /// <typeparam name="T">The result type of the operation.</typeparam>
        /// <param name="retryCount">The maximum number of retry attempts. Defaults to 3.</param>
        /// <returns>An <see cref="IPolicy{T}"/> representing the retry policy.</returns>
        IPolicy<T> CreateRetryPolicy<T>(int retryCount = 3);

        /// <summary>
        /// Creates a circuit breaker policy that opens the circuit after a specified number of failures and breaks for a given duration.
        /// </summary>
        /// <typeparam name="T">The result type of the operation.</typeparam>
        /// <param name="failureThreshold">The number of failures before opening the circuit. Defaults to 5.</param>
        /// <param name="breakDuration">The duration to keep the circuit open before resetting. Defaults to <see cref="TimeSpan.Zero"/>.</param>
        /// <returns>An <see cref="IPolicy{T}"/> representing the circuit breaker policy.</returns>
        IPolicy<T> CreateCircuitBreakerPolicy<T>(int failureThreshold = 5, TimeSpan breakDuration = default);

        /// <summary>
        /// Creates a fallback policy that executes the specified fallback operation if the primary operation fails.
        /// </summary>
        /// <typeparam name="T">The result type of the operation.</typeparam>
        /// <param name="fallbackOperation">A delegate representing the fallback operation to execute on failure.</param>
        /// <returns>An <see cref="IPolicy{T}"/> representing the fallback policy.</returns>
        IPolicy<T> CreateFallbackPolicy<T>(Func<CancellationToken, Task<T>> fallbackOperation);
        // ... more specific policy builders
    }
}
