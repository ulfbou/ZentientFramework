// <copyright file="src/Zentient.Analyzers.Testing/AnalyzerVerifier{TAnalyzer}.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// MIT License. See LICENSE in the project root for license information.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using System.Threading.Tasks;

namespace Zentient.Analyzers.Testing.Verification
{
    public static class AnalyzerVerifier
    {
        public static async Task VerifyAnalyzerAsync<TAnalyzer>(string source, params DiagnosticResult[] expected)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
            {
                TestCode = source,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            };

            test.TestState.Sources.Add("namespace Zentient.Abstractions { public sealed class ZentientStubAttribute : System.Attribute { public ZentientStubAttribute(System.Type t) {} } }");
            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync();
        }
    }
}
