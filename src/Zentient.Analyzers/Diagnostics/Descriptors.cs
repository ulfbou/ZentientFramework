// <copyright file="src/Zentient.Analyzers/Descriptors.cs" author="Ulf Bourelius">
// Copyright (c) 2025 2025 Zentient Framework Team. All rights reserved.
// See LICENSE in the project root for license information.
// </copyright>

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace Zentient.Analyzers.Diagnostics
{
    internal static class Descriptors
    {
        // RS2000: All supported analyzer diagnostic IDs should be part of an analyzer release
        public static ImmutableArray<DiagnosticDescriptor> AllDescriptors { get; } =
            ImmutableArray.Create(ZT0001_MissingMember!, ZT0002_SignatureMismatch!, ZT0100_TemplateCompilationError!);

        public static readonly DiagnosticDescriptor ZT0001_MissingMember = new(
            id: "ZT0001",
            title: "Contract member not implemented",
            messageFormat: "Type '{0}' does not implement contract member '{1}' from '{2}'",
            category: "ContractFidelity",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Ensures stubs fully implement members defined by their target interfaces.");

        public static readonly DiagnosticDescriptor ZT0002_SignatureMismatch = new(
            id: "ZT0002",
            title: "Contract signature mismatch",
            messageFormat: "Member '{0}' on '{1}' does not match signature of '{2}.{0}'",
            category: "ContractFidelity",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor ZT0100_TemplateCompilationError = new(
            id: "ZT0100",
            title: "Template compilation error",
            messageFormat: "Generated template produced compiler error: {0}",
            category: "TemplateCompilation",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.CompilationEnd); // FIX: Added CompilationEnd custom tag
    }
}
