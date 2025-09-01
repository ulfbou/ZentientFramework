// ========== src/Zentient.Analyzers/TemplateCompilationAnalyzer.cs ===================
// <copyright file="FileName.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>
// ========================================================

// == analyzers/TemplateCompilation/TemplateCompilationAnalyzer.cs ============
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

using Zentient.Analyzers.Diagnostics;

namespace Zentient.Analyzers.TemplateCompilation
{
    /// <summary>
    /// Scaffold: emits an error if the compilation already contains any C# diagnostics
    /// originating from generated files (heuristic: AdditionalFiles or hint names).
    /// In production, wire to your source-generator pipeline and feed back errors.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class TemplateCompilationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.ZT0100_TemplateCompilationError);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(AnalyzeCompilation);
        }

        private static void AnalyzeCompilation(CompilationAnalysisContext ctx)
        {
            foreach (Diagnostic diag in ctx.Compilation.GetDiagnostics())
            {
                if (diag.Severity == DiagnosticSeverity.Error && diag.Id.StartsWith("CS", System.StringComparison.Ordinal))
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.ZT0100_TemplateCompilationError,
                        diag.Location,
                        diag.ToString()));
                }
            }
        }
    }
}
