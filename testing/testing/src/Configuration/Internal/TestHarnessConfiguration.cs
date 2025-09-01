// <copyright file="TestHarnessConfiguration.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;

using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Configuration.Builders;
using Zentient.Testing.Configuration;
using Zentient.Testing.Diagnostics.Abstractions;

namespace Zentient.Testing.Configuration.Internal
{
    /// <summary>
    /// An immutable data object representing the finalized configuration of a test harness.
    /// </summary>
    internal class TestHarnessConfiguration : ITestHarnessConfiguration
    {
        /// <inheritdoc />
        public IReadOnlyCollection<object> Dependencies { get; }

        /// <summary>
        /// Gets a read-only collection of mock definitions for the test harness.
        /// </summary>
        public IReadOnlyCollection<object> MockDefinitions { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IProbe> Probes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestHarnessConfiguration"/> class.
        /// </summary>
        /// <param name="dependencies">A collection of concrete dependencies or types to register.</param>
        /// <param name="mockDefinitions">A collection of mock definitions.</param>
        /// <param name="probes">A collection of enabled diagnostic probes.</param>
        public TestHarnessConfiguration(
            IEnumerable<object> dependencies,
            IEnumerable<object> mockDefinitions,
            IEnumerable<IProbe> probes)
        {
            Dependencies = dependencies.ToList();
            MockDefinitions = mockDefinitions.ToList();
            Probes = probes.ToList();
        }
    }
}
