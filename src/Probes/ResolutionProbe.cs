// <copyright file="ResolutionProbe.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Harnesses.Abstractions;
using Zentient.Abstractions.DependencyInjection.Results;
using Zentient.Abstractions.Metadata;

namespace Zentient.Testing.Probes
{
    public sealed class ResolutionProbe : IProbe, IResolutionProbe
    {
        public IReadOnlyCollection<IServiceResolutionResult<object>> ResolutionLog { get; } = new List<IServiceResolutionResult<object>>().AsReadOnly();
        public IDisposable UseScope(string? scopeId = null, IMetadata? metadata = null) => null!;
        public IServiceResolutionResult<TService>? GetLastResolutionFor<TService>() => null;
        public IEnumerable<IServiceResolutionResult<TService>> GetAllResolutionsFor<TService>() => null!;

        public void Initialize(IHarnessContext context) { }
        public void Finalize(IDiagnosticContainer diagnostics) { }
    }
}
