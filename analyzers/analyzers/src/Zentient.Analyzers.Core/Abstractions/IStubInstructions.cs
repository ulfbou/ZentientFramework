// <copyright file="src/Zentient.Analyzers/Abstractions/IStubInstructions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Represents instructions for stub code generation.
    /// </summary>
    public interface IStubInstructions : ICodeInstructions
    {
        /// <summary>
        /// Gets the stub mode.
        /// </summary>
        StubMode Mode { get; }

        /// <summary>
        /// Emits the stub as a source unit.
        /// </summary>
        /// <returns>The generated source unit.</returns>
        ISourceUnit Emit();
    }
}
