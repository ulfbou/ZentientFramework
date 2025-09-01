// <copyright file="DIConfigurationProbe.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System;

using Zentient.Testing.Diagnostics.Abstractions;
using Zentient.Testing.Harnesses.Abstractions;
using Zentient.Abstractions.DependencyInjection;
using Zentient.Abstractions.DependencyInjection.Registration;

namespace Zentient.Testing.Probes
{
    public sealed class DIConfigurationProbe : IProbe, IDIConfigurationProbe
    {
        public bool HasService<TService>() => false;
        public bool HasService(Type serviceContract) => false;
        public bool HasLifetime<TService>(ServiceLifetime lifetime) => false;
        public bool HasMetadata<TService>(string key, object? expectedValue) => false;
        public bool IsDecorated<TService, TDecorator>() => false;
        public bool IsIntercepted<TService, TInterceptor>() => false;
        public IServiceRegistry Registry { get; } = null!;

        public void Initialize(IHarnessContext context) { }
        public void Finalize(IDiagnosticContainer diagnostics) { }
    }
}
