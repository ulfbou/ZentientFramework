// <copyright file="ITestHarness.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Envelopes;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Probes;

namespace Zentient.Testing.Harnesses.Abstractions
{
    public interface ITestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition>
        where TDefinition : ITypeDefinition
        where TCodeDefinition : ICodeDefinition
        where TErrorDefinition : IErrorDefinition
    {
        Task<IEnvelope<TCodeDefinition, TErrorDefinition, TOutput>> RunAsync(
            TInput input,
            CancellationToken cancellationToken = default);

        ICacheDiagnostics CacheProbe { get; }
        IRetryDiagnostics RetryProbe { get; }
        IDIGraphProbe DiGraphProbe { get; }
        IDIConfigurationProbe DiConfigurationProbe { get; }
        IResolutionProbe ResolutionProbe { get; }
    }

    public interface IHandlerTestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition> : ITestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition>
        where TDefinition : ITypeDefinition
        where TCodeDefinition : ICodeDefinition
        where TErrorDefinition : IErrorDefinition
    {
    }

    public interface IPolicyTestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition> : ITestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition>
        where TDefinition : ITypeDefinition
        where TCodeDefinition : ICodeDefinition
        where TErrorDefinition : IErrorDefinition
    {
    }
}
