// <copyright file="src/Zentient.Analyzers/Registry/StubContext.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Analyzers.Abstractions;

namespace Zentient.Analyzers.Registry.Contexts
{
    /// <summary>Provides context information for stub generation.</summary>
    /// <param name="Key">The unique key for the stub context.</param>
    /// <param name="Domain">The domain for the stub context.</param>
    internal sealed record StubContext(string Key, string Domain) : IStubContext;
}
