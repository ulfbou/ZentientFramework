// <copyright file="src/Zentient.Analyzers.Testing/GoldenDiagnostic.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Testing.GoldenFixtures
{
    // Use a class instead of a record for .NET Standard 2.0 compatibility

    public sealed class GoldenDiagnostic
    {
        public string Id { get; }
        public string Message { get; }
        public string Severity { get; }
        public string File { get; }
        public int Start { get; }
        public int Length { get; }

        public GoldenDiagnostic(string id, string message, string severity, string file, int start, int length)
        {
            Id = id;
            Message = message;
            Severity = severity;
            File = file;
            Start = start;
            Length = length;
        }
    }
}
