// <copyright file="HandlerTestHarness.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Threading;
using System.Threading.Tasks;

using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Envelopes;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Testing.Configuration;
using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Harnesses.Abstractions;
using Zentient.Testing.Probes;

namespace Zentient.Testing.Harnesses
{
    public sealed class HandlerTestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition> : IHandlerTestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition>
        where TDefinition : ITypeDefinition
        where TCodeDefinition : ICodeDefinition
        where TErrorDefinition : IErrorDefinition
    {
        public ICacheDiagnostics CacheProbe => null!;
        public IRetryDiagnostics RetryProbe => null!;
        public IDIGraphProbe DiGraphProbe => null!;
        public IDIConfigurationProbe DiConfigurationProbe => null!;
        public IResolutionProbe ResolutionProbe => null!;

        public HandlerTestHarness(ITestHarnessConfiguration config) { }

        public Task<IEnvelope<TCodeDefinition, TErrorDefinition, TOutput>> RunAsync(TInput input, CancellationToken cancellationToken = default) => Task.FromResult<IEnvelope<TCodeDefinition, TErrorDefinition, TOutput>>(null!);
    }
}
