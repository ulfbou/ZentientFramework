// <copyright file="TestConfigurationBuilder.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Configuration.Builders;
using Zentient.Testing.Abstractions;
using Zentient.Testing.Configuration;
using Zentient.Testing.Diagnostics;
using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Probes;

namespace Zentient.Testing.Configuration.Internal
{
    /// <summary>
    /// A fluent builder for configuring a test harness. It collects all dependencies,
    /// mocks, and probes before creating an immutable ITestHarnessConfiguration object.
    /// </summary>
    internal class TestConfigurationBuilder : ITestConfigurationBuilder
    {
        private readonly List<object> _dependencies = new();
        private readonly List<object> _mockDefinitions = new();
        private ProbeType _enabledProbes = ProbeType.None;

        /// <inheritdoc />
        public ITestConfigurationBuilder WithDependency<TService>(TService instance) where TService : class
        {
            _dependencies.Add(instance);
            return this;
        }

        /// <inheritdoc />
        public ITestConfigurationBuilder WithDependency<TService, TImplementation>()
            where TService : class
            where TImplementation : TService
        {
            _dependencies.Add(typeof(TService));
            _dependencies.Add(typeof(TImplementation));
            return this;
        }

        /// <inheritdoc />
        public ITestConfigurationBuilder RegisterMock<TService, TDefinition>(IMockDefinition<TService> mockDefinition)
            where TService : class
            where TDefinition : ITypeDefinition
        {
            _mockDefinitions.Add(mockDefinition);
            return this;
        }

        /// <inheritdoc />
        public ITestConfigurationBuilder WithProbes(ProbeType probes)
        {
            _enabledProbes = probes;
            return this;
        }

        /// <inheritdoc />
        public ITestHarnessConfiguration Build()
        {
            var probes = new List<IProbe>();
            if (_enabledProbes.HasFlag(ProbeType.Cache))
                probes.Add(new CacheProbe());
            if (_enabledProbes.HasFlag(ProbeType.Retry))
                probes.Add(new RetryProbe());
            if (_enabledProbes.HasFlag(ProbeType.DiConfiguration))
                probes.Add(new DIConfigurationProbe());
            if (_enabledProbes.HasFlag(ProbeType.DiGraph))
                probes.Add(new DIGraphProbe());
            if (_enabledProbes.HasFlag(ProbeType.Resolution))
                probes.Add(new ResolutionProbe());

            return new TestHarnessConfiguration(_dependencies, _mockDefinitions, probes);
        }
    }
}
