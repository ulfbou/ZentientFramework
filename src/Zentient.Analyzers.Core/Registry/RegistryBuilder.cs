// <copyright file="src/Zentient.Analyzers/Registry/RegistryBuilder.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Analyzers.Abstractions;

namespace Zentient.Analyzers.Registry
{
    /// <summary>
    /// Implements <see cref="IRegistryBuilder"/> to build a registry of code instructions, stubs, and templates.
    /// </summary>
    internal sealed class RegistryBuilder : IRegistryBuilder
    {
        private readonly List<ICodeInstructions> _instructions = new();

        /// <inheritdoc/>
        public IStubBuilder AddStub(string key, string domain)
        {
            return new StubBuilder(this, key, domain);
        }

        /// <inheritdoc/>
        public ITemplateBuilder AddTemplate(string key, string domain)
        {
            return new TemplateBuilder(this, key, domain);
        }

        /// <inheritdoc/>
        public IRegistry Build()
        {
            return new Registry(_instructions.ToImmutableArray());
        }

        /// <summary>
        /// Adds instructions to the registry.
        /// </summary>
        /// <param name="instructions">The instructions to add.</param>
        internal void Add(ICodeInstructions instructions)
        {
            _instructions.Add(instructions);
        }
    }
}
