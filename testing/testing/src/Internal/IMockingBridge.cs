// <copyright file="IMockingBridge.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Configuration.Builders;
using Zentient.Testing.Abstractions;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Internal interface for applying mock definitions to mock instances.
    /// </summary>
    internal interface IMockingBridge
    {
        /// <summary>
        /// Applies the specified mock definition to the given mock instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service being mocked.</typeparam>
        /// <param name="definition">The mock definition.</param>
        /// <param name="mockInstance">The mock instance.</param>
        void Apply<TService>(IMockDefinition<TService> definition, TService mockInstance) where TService : class;
    }
}
