// <copyright file="DIGraphProbe.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Harnesses.Abstractions;
using Zentient.Abstractions.DependencyInjection.Validation;
using Zentient.Abstractions.Results;
using Zentient.Abstractions.DependencyInjection.Results;

namespace Zentient.Testing.Probes
{
    public sealed class DIGraphProbe : IProbe, IDIGraphProbe
    {
        public Task<IResult> ValidateAsync(ServiceValidationOptions? options = null, CancellationToken cancellationToken = default) => Task.FromResult<IResult>(null!);
        public Task<IServiceDependencyGraph> AnalyzeAsync(Type? rootServiceType = null, CancellationToken cancellationToken = default) => Task.FromResult<IServiceDependencyGraph>(null!);
        public bool HasCircularDependencies() => false;
        public bool HasLifetimeMismatches() => false;
        public bool HasUnusedServices() => false;

        public void Initialize(IHarnessContext context) { }
        public void Finalize(IDiagnosticContainer diagnostics) { }
    }
}
