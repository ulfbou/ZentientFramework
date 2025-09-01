// <copyright file="IEnvelopeAssertions.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Envelopes;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Results;

namespace Zentient.Testing.FluentAssertions
{
    public interface IEnvelopeAssertions<TCodeDefinition, TErrorDefinition, TResult>
        where TCodeDefinition : ICodeDefinition
        where TErrorDefinition : IErrorDefinition
    {
        IEnvelope<TCodeDefinition, TErrorDefinition, TResult> Subject { get; }
        IEnvelopeAssertions<TCodeDefinition, TErrorDefinition, TResult> BeSuccess();
        IEnvelopeAssertions<TCodeDefinition, TErrorDefinition, TResult> BeFailure();
        IResultValueAssertions<TResult> HaveValue();
        void HaveValue(TResult expectedValue);
        void HaveMetadata(string key, string expectedValue);
    }

    public interface IResultValueAssertions<TResult>
    {
        TResult Subject { get; }
        void Be(TResult expectedValue);
    }
}
