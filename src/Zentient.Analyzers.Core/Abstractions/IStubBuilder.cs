// <copyright file="src/Zentient.Analyzers/Abstractions/IStubBuilder.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Provides a fluent interface for building stub instructions.
    /// </summary>
    public interface IStubBuilder
    {
        /// <summary>
        /// Sets the stub mode.
        /// </summary>
        /// <param name="mode">The stub mode.</param>
        /// <returns>The stub builder instance.</returns>
        IStubBuilder WithMode(StubMode mode);

        /// <summary>
        /// Specifies required instruction keys for the stub.
        /// </summary>
        /// <param name="instructionKeys">The required instruction keys.</param>
        /// <returns>The stub builder instance.</returns>
        IStubBuilder Requires(params string[] instructionKeys);

        /// <summary>
        /// Sets the emitter function for the stub.
        /// </summary>
        /// <param name="emitter">The emitter function.</param>
        /// <returns>The stub builder instance.</returns>
        IStubBuilder Emitter(Func<IStubContext, ISourceUnit> emitter);

        /// <summary>
        /// Adds the stub to the registry builder.
        /// </summary>
        /// <returns>The registry builder instance.</returns>
        IRegistryBuilder Add();
    }
}
