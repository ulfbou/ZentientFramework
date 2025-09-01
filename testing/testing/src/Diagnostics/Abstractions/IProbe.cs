// <copyright file="IProbe.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Testing.Configuration;
using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Diagnostics.Internal;
using Zentient.Testing.Harnesses.Abstractions;

namespace Zentient.Testing.Diagnostics.Abstractions
{
    public interface IProbe
    {
        /// <summary>
        /// Initializes the probe, providing it with a context for the current test run.
        /// This method is called once when the harness is constructed.
        /// </summary>
        void Initialize(IHarnessContext context);

        /// <summary>
        /// Captures the probe's final state and transfers it to the diagnostic container.
        /// This method is called once by the harness after the test has completed.
        /// </summary>
        void Finalize(IDiagnosticContainer diagnostics);
    }
}
