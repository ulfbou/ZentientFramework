// <copyright file="src/Zentient.Analyzers/Abstractions/ICodeInstructions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Represents instructions for code generation.
    /// </summary>
    public interface ICodeInstructions
    {
        /// <summary>
        /// Gets the unique key for the instructions.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the domain for the instructions.
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// Gets the list of required instruction keys.
        /// </summary>
        IReadOnlyList<string> Requires { get; }
    }
}
