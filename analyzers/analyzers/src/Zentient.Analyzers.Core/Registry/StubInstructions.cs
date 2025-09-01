// <copyright file="src/Zentient.Analyzers/Registry/StubInstructions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>


using Zentient.Analyzers.Abstractions;
using Zentient.Analyzers.Registry.Contexts;

namespace Zentient.Analyzers.Registry
{
    /// <summary>
    /// Concrete implementation of <see cref="IStubInstructions"/>.
    /// </summary>
    /// <param name="Key">The stub key.</param>
    /// <param name="Domain">The stub domain.</param>
    /// <param name="Requires">Required instruction keys.</param>
    /// <param name="Mode">The stub mode.</param>
    /// <param name="Emitter">The emitter function.</param>
    internal sealed record StubInstructions(
        string Key,
        string Domain,
        IReadOnlyList<string> Requires,
        StubMode Mode,
        Func<IStubContext, ISourceUnit> Emitter) : IStubInstructions
    {
        /// <inheritdoc/>
        public ISourceUnit Emit() => Emitter(new StubContext(Key, Domain));
    }
}
