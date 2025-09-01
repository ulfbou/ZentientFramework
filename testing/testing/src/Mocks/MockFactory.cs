// <copyright file="MockFactory.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.Common.Definitions;
using Zentient.Testing.Mocks;

namespace Zentient.Testing.Mocks
{
    public sealed class MockFactory : IMockFactory
    {
        /// <inheritdoc />
        public IMockHandlerBuilder<TInput, TOutput, TDefinition> CreateHandler<TInput, TOutput, TDefinition>()
            where TDefinition : ITypeDefinition => null!;

        /// <inheritdoc />
        public IMockPolicyBuilder<TInput, TOutput, TDefinition> CreatePolicy<TInput, TOutput, TDefinition>()
            where TDefinition : ITypeDefinition => null!;
    }
}
