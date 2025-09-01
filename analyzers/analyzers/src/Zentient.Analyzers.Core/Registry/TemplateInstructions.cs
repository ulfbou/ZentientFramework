// <copyright file="src/Zentient.Analyzers/Registry/TemplateInstructions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Analyzers.Abstractions;
using Zentient.Analyzers.Registry.Contexts;

namespace Zentient.Analyzers.Registry
{
    /// <summary>
    /// Concrete implementation of <see cref="ITemplateInstructions"/>.
    /// </summary>
    /// <param name="Key">The template key.</param>
    /// <param name="Domain">The template domain.</param>
    /// <param name="Requires">Required instruction keys.</param>
    /// <param name="Emitter">The emitter function.</param>
    internal sealed record TemplateInstructions(
        string Key,
        string Domain,
        IReadOnlyList<string> Requires,
        Func<ITemplateContext, IReadOnlyList<ISourceUnit>, ISourceUnit> Emitter) : ITemplateInstructions
    {
        /// <inheritdoc/>
        public ISourceUnit Emit(IReadOnlyList<ISourceUnit> stubs) => Emitter(new TemplateContext(Key, Domain), stubs);
    }
}
