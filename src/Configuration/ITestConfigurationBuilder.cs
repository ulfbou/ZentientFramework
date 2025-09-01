// <copyright file="ITestConfigurationBuilder.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System;
using System.Collections.Generic;

using Zentient.Abstractions.Common;
using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Configuration.Builders;
using Zentient.Testing.Abstractions;
using Zentient.Testing.Configuration;
using Zentient.Testing.Diagnostics;
using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Probes;
using Zentient.Testing.Internal;
using Zentient.Testing.Configuration.Internal;

namespace Zentient.Testing.Configuration
{
    /// <summary>
    /// A fluent builder for creating an immutable test harness configuration.
    /// </summary>
    public interface ITestConfigurationBuilder
    {
        /// <summary>
        /// Registers a concrete dependency instance with the test harness.
        /// </summary>
        ITestConfigurationBuilder WithDependency<TService>(TService instance) where TService : class;

        /// <summary>
        /// Registers a service contract and its concrete implementation type.
        /// </summary>
        ITestConfigurationBuilder WithDependency<TService, TImplementation>()
            where TService : class
            where TImplementation : TService;

        /// <summary>
        /// Registers a mock definition for a service contract.
        /// </summary>
        ITestConfigurationBuilder RegisterMock<TService, TDefinition>(IMockDefinition<TService> mockDefinition)
            where TService : class
            where TDefinition : ITypeDefinition;

        /// <summary>
        /// Explicitly enables the specified diagnostic probes for the test run.
        /// </summary>
        ITestConfigurationBuilder WithProbes(ProbeType probes);

        /// <summary>
        /// Finalizes the configuration and returns an immutable configuration object.
        /// </summary>
        ITestHarnessConfiguration Build();
    }
}
