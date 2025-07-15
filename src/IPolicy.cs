// <copyright file="IEnvelope.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions
{
    /// <summary>
    /// Represents a policy that can be applied to an operation producing a result of type <typeparamref name="T"/>.
    /// </summary>
    public interface IPolicy<T>
    {
        /// <summary>
        /// Executes the given delegate under the policy’s behavior.
        /// </summary>
        Task<T> Execute(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default);
    }
}
