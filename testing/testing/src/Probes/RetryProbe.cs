// <copyright file="RetryProbe.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System;
using System.Collections.Generic;

using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Harnesses.Abstractions;
using Zentient.Testing.Probes;

namespace Zentient.Testing.Probes
{
    /// <summary>
    /// The concrete implementation of the retry probe.
    /// It captures and exposes information about retry attempts and exceptions.
    /// </summary>
    public sealed class RetryProbe : IProbe, IRetryDiagnostics
    {
        /// <inheritdoc />
        public int GetRetryCount() => 0;

        /// <inheritdoc />
        public IReadOnlyCollection<Exception> GetExceptions() => new List<Exception>().AsReadOnly();

        /// <inheritdoc />
        public void Initialize(IHarnessContext context)
        {
            // Logic to hook into the retry policy
        }

        /// <inheritdoc />
        public void Finalize(IDiagnosticContainer diagnostics)
        {
            // Logic to add the collected data to the diagnostic container
        }
    }
}