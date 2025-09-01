// <copyright file="src/Zentient.Analyzers/Registry/TemplateBuilder.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Analyzers.Abstractions;

namespace Zentient.Analyzers.Registry
{
    /// <summary>
    /// Implements <see cref="ITemplateBuilder"/> for fluent template instruction creation.
    /// </summary>
    internal sealed class TemplateBuilder : ITemplateBuilder
    {
        private readonly RegistryBuilder _builder;
        private readonly string _key;
        private readonly string _domain;
        private string[] _requires = Array.Empty<string>();
        private Func<ITemplateContext, IReadOnlyList<ISourceUnit>, ISourceUnit>? _emitter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateBuilder"/> class.
        /// </summary>
        /// <param name="builder">The registry builder.</param>
        /// <param name="key">The template key.</param>
        /// <param name="domain">The template domain.</param>
        public TemplateBuilder(RegistryBuilder builder, string key, string domain)
        {
            _builder = builder;
            _key = key;
            _domain = domain;
        }

        /// <inheritdoc/>
        public ITemplateBuilder Requires(params string[] instructionKeys)
        {
            _requires = instructionKeys;
            return this;
        }

        /// <inheritdoc/>
        public ITemplateBuilder Emitter(Func<ITemplateContext, IReadOnlyList<ISourceUnit>, ISourceUnit> emitter)
        {
            _emitter = emitter;
            return this;
        }

        /// <inheritdoc/>
        public IRegistryBuilder Add()
        {
            if (_emitter == null)
            {
                throw new InvalidOperationException("Emitter function must be set.");
            }
            var instructions = new TemplateInstructions(_key, _domain, _requires, _emitter);
            _builder.Add(instructions);
            return _builder;
        }
    }
}
