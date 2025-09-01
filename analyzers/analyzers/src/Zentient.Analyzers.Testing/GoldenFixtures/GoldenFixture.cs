// <copyright file="src/Zentient.Analyzers.Testing/GoldenFixture.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zentient.Analyzers.Testing.GoldenFixtures
{
    public sealed record GoldenFixture(
        string Id,
        string Source,
        GoldenDiagnostic[] ExpectedDiagnostics);
}
