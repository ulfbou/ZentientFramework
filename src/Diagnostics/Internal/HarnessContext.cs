// <copyright file="HarnessContext.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.DependencyInjection;
using Zentient.Testing.Configuration;
using Zentient.Testing.Harnesses;
using Zentient.Testing.Harnesses.Abstractions;

namespace Zentient.Testing.Diagnostics.Internal
{
    /// <summary>
    /// Provides a contextual environment for diagnostic probes during a test run.
    /// It encapsulates state required by probes to perform their analysis, such as
    /// access to the service resolver and configuration.
    /// </summary>
    internal sealed class HarnessContext : IHarnessContext
    {
        private readonly IServiceResolver _resolver;
        private readonly ITestHarnessConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HarnessContext"/> class.
        /// </summary>
        /// <param name="resolver">The service resolver for the test's dependency injection container.</param>
        /// <param name="configuration">The immutable test harness configuration.</param>
        internal HarnessContext(IServiceResolver resolver, ITestHarnessConfiguration configuration)
        {
            _resolver = resolver;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets the service resolver associated with the test harness.
        /// </summary>
        public IServiceResolver Resolver => _resolver;

        /// <summary>
        /// Gets the immutable test harness configuration.
        /// </summary>
        public ITestHarnessConfiguration Configuration => _configuration;
    }
}
