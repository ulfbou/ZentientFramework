// <copyright file="src/Zentient.Analyzers/Abstractions/IStubContext.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Provides context information for stub generation.
    /// </summary>
    public interface IStubContext
    {
        /// <summary>
        /// Gets the unique key for the stub context.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the domain for the stub context.
        /// </summary>
        string Domain { get; }
    }
}
