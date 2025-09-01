// <copyright file="src/Zentient.Analyzers/ContractFidelityAnalyzer.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using Zentient.Analyzers.Diagnostics;
using Zentient.Analyzers.Helpers;

namespace Zentient.Analyzers.ContractFidelity
{
    /// <summary>
    /// Enforces that any class marked with [ZentientStub(typeof(IMyContract))]
    /// fully implements the interface contract.
    /// This is a narrow, scaffolded starting point that can be generalized to your
    /// real stub discovery mechanism.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ContractFidelityAnalyzer : DiagnosticAnalyzer
    {
        private const string StubAttributeMetadataName = "Zentient.Abstractions.ZentientStubAttribute";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.ZT0001_MissingMember, Descriptors.ZT0002_SignatureMismatch);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClass(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.Node is not ClassDeclarationSyntax cds)
            {
                return;
            }

            INamedTypeSymbol? classSymbol = ctx.SemanticModel.GetDeclaredSymbol(cds);
            if (classSymbol == null)
            {
                return;
            }

            if (!classSymbol.TryGetAttributeCtorArg(StubAttributeMetadataName, out TypedConstant ctorArg))
            {
                return;
            }

            if (!(ctorArg.Value is INamedTypeSymbol targetInterface) || targetInterface.TypeKind != TypeKind.Interface)
            {
                return;
            }

            var requiredMethods = targetInterface.GetMembers().OfType<IMethodSymbol>()
                .Where(m => m.MethodKind == MethodKind.Ordinary).ToList();
            var requiredProps = targetInterface.GetMembers().OfType<IPropertySymbol>().ToList();

            foreach (IMethodSymbol req in requiredMethods)
            {
                IMethodSymbol? impl = classSymbol.FindImplementationForInterfaceMember(req) as IMethodSymbol;
                if (impl == null)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.ZT0001_MissingMember,
                        cds.Identifier.GetLocation(),
                        classSymbol.Name, req.Name, targetInterface.Name));
                    continue;
                }

                bool sigMismatch = impl.Parameters.Length != req.Parameters.Length || !SymbolEqualityComparer.Default.Equals(impl.ReturnType, req.ReturnType);
                if (sigMismatch)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.ZT0002_SignatureMismatch,
                        impl.Locations.FirstOrDefault(),
                        req.Name, classSymbol.Name, targetInterface.Name));
                }
            }

            foreach (IPropertySymbol req in requiredProps)
            {
                IPropertySymbol? impl = classSymbol.FindImplementationForInterfaceMember(req) as IPropertySymbol;
                if (impl == null)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.ZT0001_MissingMember,
                        cds.Identifier.GetLocation(),
                        classSymbol.Name, req.Name, targetInterface.Name));
                }
                else
                {
                    bool typeMismatch = !SymbolEqualityComparer.Default.Equals(impl.Type, req.Type);
                    if (typeMismatch)
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(
                            Descriptors.ZT0002_SignatureMismatch,
                            impl.Locations.FirstOrDefault(),
                            req.Name, classSymbol.Name, targetInterface.Name));
                    }
                }
            }
        }
    }
}
