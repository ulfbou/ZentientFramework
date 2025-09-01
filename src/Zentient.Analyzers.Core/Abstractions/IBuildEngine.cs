// <copyright file="src/Zentient.Analyzers/Abstractions/IBuildEngine.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Collections.Immutable;

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Defines the build engine responsible for emitting <see cref="ISourceUnit"/>s
    /// from registered <see cref="ICodeInstructions"/> seeds.
    /// </summary>
    public interface IBuildEngine
    {
        /// <summary>
        /// Raised immediately before emitting a source unit for the given instructions.
        /// Provides the <see cref="ICodeInstructions"/> that will be used to emit.
        /// </summary>
        event Action<ICodeInstructions>? BeforeEmit;

        /// <summary>
        /// Raised after a source unit has been emitted.
        /// Provides the concrete <see cref="ISourceUnit"/> that was produced.
        /// </summary>
        event Action<ISourceUnit>? AfterEmit;

        /// <summary>Builds source units from the specified seed keys.</summary>
        /// <param name="instructionKeys">The seed instruction keys to build from.</param>
        /// <param name="includeDependencies">
        /// If true (default), also emit all transitive dependencies of the seeds; if false, only the seeds are emitted.
        /// </param>
        /// <returns>
        /// A read-only list of all emitted <see cref="ISourceUnit"/> instances in a valid topological order.
        /// </returns>
        IReadOnlyList<ISourceUnit> Build(IEnumerable<string> instructionKeys, bool includeDependencies = true);
    }
}
