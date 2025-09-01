// <copyright file="CacheProbe.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System;
using System.Collections.Generic;

using Zentient.Abstractions.Caching;
using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Harnesses.Abstractions;

namespace Zentient.Testing.Probes
{
    /// <summary>
    /// The concrete implementation of the cache probe.
    /// It captures and exposes information about cache policy usage.
    /// </summary>
    public sealed class CacheProbe : IProbe, ICacheDiagnostics
    {
        private readonly List<CachePolicyUsage> _usages = new();

        /// <summary>
        /// Gets a read-only collection of cache policy usages recorded during the test run.
        /// </summary>
        public IReadOnlyCollection<CachePolicyUsage> Usages => _usages.AsReadOnly();

        /// <inheritdoc />
        public void Initialize(IHarnessContext context)
        {
            // Logic to hook into the caching system
        }

        /// <inheritdoc />
        public void Finalize(IDiagnosticContainer diagnostics)
        {
            // Logic to add the collected data to the diagnostic container
        }
    }
}