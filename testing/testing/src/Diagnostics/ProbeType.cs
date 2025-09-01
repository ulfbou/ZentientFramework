// <copyright file="ProbeType.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System;

namespace Zentient.Testing.Diagnostics
{
    /// <summary>
    /// Defines the types of diagnostic probes available in the Zentient.Testing framework.
    /// This enum is used to explicitly opt in to specific probes for a test run.
    /// </summary>
    [Flags]
    public enum ProbeType
    {
        /// <summary>
        /// No probes are enabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enables the cache policy probe to capture cache usage.
        /// </summary>
        Cache = 1 << 0,

        /// <summary>
        /// Enables the retry probe to capture retry attempts and failures.
        /// </summary>
        Retry = 1 << 1,
        DiConfiguration = 3,
        DiGraph = 4,
        Resolution = 5,

        // Add other probe types here
        // DiGraph = 1 << 2,
    }
}
