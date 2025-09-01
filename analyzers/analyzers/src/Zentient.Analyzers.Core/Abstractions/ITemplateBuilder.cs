// <copyright file="src/Zentient.Analyzers/Abstractions/ITemplateBuilder.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Provides a fluent interface for building template instructions.
    /// </summary>
    public interface ITemplateBuilder
    {
        /// <summary>
        /// Specifies required instruction keys for the template.
        /// </summary>
        /// <param name="instructionKeys">The required instruction keys.</param>
        /// <returns>The template builder instance.</returns>
        ITemplateBuilder Requires(params string[] instructionKeys);

        /// <summary>
        /// Sets the emitter function for the template.
        /// </summary>
        /// <param name="emitter">The emitter function.</param>
        /// <returns>The template builder instance.</returns>
        ITemplateBuilder Emitter(Func<ITemplateContext, IReadOnlyList<ISourceUnit>, ISourceUnit> emitter);

        /// <summary>
        /// Adds the template to the registry builder.
        /// </summary>
        /// <returns>The registry builder instance.</returns>
        IRegistryBuilder Add();
    }
}
