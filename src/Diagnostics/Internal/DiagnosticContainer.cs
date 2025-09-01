// <copyright file="DiagnosticContainer.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Collections.Generic;

using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Diagnostics.Internal;
using Zentient.Testing.Probes;

namespace Zentient.Testing.Diagnostics.Internal
{
    /// <summary>
    /// An immutable container that holds the final, structured diagnostic data
    /// collected by probes during a test run.
    /// </summary>
    internal sealed class DiagnosticContainer : IDiagnosticContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticContainer"/> class.
        /// </summary>
        /// <param name="diagnostics">A dictionary of key-value pairs representing the diagnostics for each probe.</param>
        internal DiagnosticContainer(IDictionary<string, object> diagnostics)
        {
            if (diagnostics.TryGetValue(nameof(ICacheDiagnostics), out var cacheDiagnostics))
            {
                Cache = (ICacheDiagnostics)cacheDiagnostics;
            }
        }

        /// <summary>
        /// Gets the diagnostics collected by the cache probe.
        /// </summary>
        public ICacheDiagnostics? Cache { get; }

        /// <summary>
        /// Gets the diagnostics collected by the retry probe.
        /// </summary>
        public IRetryDiagnostics? Retry { get; }

        // Add other probe diagnostics properties here
        // public IRetryDiagnostics? Retry { get; }
    }
}
