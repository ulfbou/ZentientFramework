// <copyright file="IDependencyRegistrar.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.Common.Definitions;

namespace Zentient.Testing.Abstractions
{
    public interface IDependencyRegistrar
    {
        IDependencyRegistrar WithDependency<TService>(TService instance) where TService : class;

        IDependencyRegistrar WithDependency<TService, TImplementation>()
            where TService : class
            where TImplementation : TService;

        IDependencyRegistrar RegisterMock<TService, TDefinition>(IMockDefinition<TService> mockDefinition)
            where TService : class
            where TDefinition : ITypeDefinition;
    }
}
