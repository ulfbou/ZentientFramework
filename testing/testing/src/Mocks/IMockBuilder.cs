// <copyright file="IMockBuilder.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.Common.Definitions;

namespace Zentient.Testing.Mocks
{
    public interface IMockBuilder<TInput, TOutput, TDefinition>
        where TDefinition : ITypeDefinition
    {
        IMockBuilder<TInput, TOutput, TDefinition> WithDefinition(TDefinition definition);
        IMockBuilder<TInput, TOutput, TDefinition> OnExecute(Func<TInput, Task<TOutput>> executeFunc);
        IMockBuilder<TInput, TOutput, TDefinition> OnExecute(Func<TInput, CancellationToken, Task<TOutput>> executeFunc);
        IMockBuilder<TInput, TOutput, TDefinition> When(Func<TInput, bool> predicate);
        IMockBuilder<TInput, TOutput, TDefinition> Returns(TOutput value);
        IMockBuilder<TInput, TOutput, TDefinition> Throws(Exception exception);
        object Build(out IMockVerifier verifier);
    }
}
