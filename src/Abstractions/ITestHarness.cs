// <copyright file="ITestHarness.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Linq.Expressions;

using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Envelopes;
using Zentient.Abstractions.Errors.Definitions;

namespace Zentient.Testing.Abstractions
{
    public interface ITestHarness<TCodeDefinition, TErrorDefinition, TInput, TResult, TDefinition> : IDisposable, IAsyncDisposable
        where TCodeDefinition : ICodeDefinition
        where TErrorDefinition : IErrorDefinition
        where TInput : notnull
        where TResult : notnull
        where TDefinition : ITypeDefinition
    {
        IReadOnlyCollection<object> Dependencies { get; }

        Task<IEnvelope<TCodeDefinition, TErrorDefinition>> Run(TInput input, CancellationToken cancellationToken = default);
    }
}
