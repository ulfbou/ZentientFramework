// <copyright file="IMockFactory.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.Common.Definitions;

namespace Zentient.Testing.Mocks
{
    public interface IMockFactory
    {
        IMockHandlerBuilder<TInput, TOutput, TDefinition> CreateHandler<TInput, TOutput, TDefinition>()
            where TDefinition : ITypeDefinition;

        IMockPolicyBuilder<TInput, TOutput, TDefinition> CreatePolicy<TInput, TOutput, TDefinition>()
            where TDefinition : ITypeDefinition;
    }
}
