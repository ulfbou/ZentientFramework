// <copyright file="src/Zentient.Analyzers/Abstractions/IRegistry.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Runtime.CompilerServices;
using System.Text;

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Represents a registry of code instructions, stubs, and templates.
    /// </summary>
    public interface IRegistry
    {
        /// <summary>Gets all registered instruction keys.</summary>
        IReadOnlyCollection<string> Keys { get; }

        /// <summary>Gets stub instructions by key.</summary>
        /// <param name="key">The stub key.</param>
        /// <returns>The stub instructions.</returns>
        IStubInstructions GetStub(string key);

        /// <summary>Gets template instructions by key.</summary>
        /// <param name="key">The template key.</param>
        /// <returns>The template instructions.</returns>
        ITemplateInstructions GetTemplate(string key);

        /// <summary>Gets code instructions by key.</summary>
        /// <param name="key">The instruction key.</param>
        /// <returns>The code instructions.</returns>
        ICodeInstructions GetInstructions(string key);
    }
}
