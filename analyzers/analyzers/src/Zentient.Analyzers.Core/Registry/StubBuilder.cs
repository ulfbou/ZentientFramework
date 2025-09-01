// <copyright file="src/Zentient.Analyzers/Registry/StubBuilder.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Analyzers.Abstractions;

namespace Zentient.Analyzers.Registry
{
    /// <summary>
    /// Implements <see cref="IStubBuilder"/> for fluent stub instruction creation.
    /// </summary>
    internal sealed class StubBuilder : IStubBuilder
    {
        private readonly RegistryBuilder _builder;
        private readonly string _key;
        private readonly string _domain;
        private StubMode _mode = StubMode.Full;
        private string[] _requires = Array.Empty<string>();
        private Func<IStubContext, ISourceUnit>? _emitter;

        /// <summary>
        /// Initializes a new instance of the <see cref="StubBuilder"/> class.
        /// </summary>
        /// <param name="builder">The registry builder.</param>
        /// <param name="key">The stub key.</param>
        /// <param name="domain">The stub domain.</param>
        public StubBuilder(RegistryBuilder builder, string key, string domain)
        {
            _builder = builder;
            _key = key;
            _domain = domain;
        }

        /// <inheritdoc/>
        public IStubBuilder WithMode(StubMode mode)
        {
            _mode = mode;
            return this;
        }

        /// <inheritdoc/>
        public IStubBuilder Requires(params string[] instructionKeys)
        {
            _requires = instructionKeys;
            return this;
        }

        /// <inheritdoc/>
        public IStubBuilder Emitter(Func<IStubContext, ISourceUnit> emitter)
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
            var instructions = new StubInstructions(_key, _domain, _requires, _mode, _emitter);
            _builder.Add(instructions);
            return _builder;
        }
    }
}
