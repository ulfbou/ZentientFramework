// <copyright file="IMockHandlerBuilder.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.Common.Definitions;

namespace Zentient.Testing.Mocks
{
    public interface IMockHandlerBuilder<TInput, TOutput, TDefinition> : IMockBuilder<TInput, TOutput, TDefinition>
        where TDefinition : ITypeDefinition
    {
    }
}
