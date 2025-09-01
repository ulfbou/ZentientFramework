// <copyright file="src/Zentient.Analyzers.Testing/GoldenFixtureRegistry.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zentient.Analyzers.Testing.GoldenFixtures
{
    public static class GoldenFixtureRegistry
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static IReadOnlyList<GoldenFixture> Load(string json)
            => JsonSerializer.Deserialize<List<GoldenFixture>>(json, Options)!;
    }
}
