// <copyright file="IMockDefinition.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.Common.Definitions;

namespace Zentient.Testing.Abstractions
{
    /// <summary>
    /// Represents the abstract definition of a mock for a service contract.
    /// It encapsulates the service type and the behaviors that the mock will exhibit.
    /// </summary>
    /// <typeparam name="TService">The service contract type being mocked.</typeparam>
    public interface IMockDefinition<TService> where TService : class
    {
        /// <summary>
        /// Gets the service contract type for the mock.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// Gets a read-only collection of behaviors that define how the mock will respond to calls.
        /// </summary>
        IReadOnlyCollection<IMockBehavior> Behaviors { get; }
    }
}
