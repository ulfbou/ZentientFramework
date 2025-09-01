// <copyright file="src/Zentient.Analyzers/Abstractions/ITemplateInstructions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Represents instructions for template code generation.
    /// </summary>
    public interface ITemplateInstructions : ICodeInstructions
    {
        /// <summary>
        /// Emits the template as a source unit, given a list of stubs.
        /// </summary>
        /// <param name="stubs">The stubs to include in the template.</param>
        /// <returns>The generated source unit.</returns>
        ISourceUnit Emit(IReadOnlyList<ISourceUnit> stubs);
    }
}
