using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Zentient.Metadata.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingInterfaceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ZMD003";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Missing required interface for attributed type",
            "Type with {0} must implement {1}",
            "Usage",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var decl = (TypeDeclarationSyntax)context.Node;
            var attrs = decl.AttributeLists.SelectMany(a => a.Attributes);
            foreach (var attr in attrs)
            {
                var name = attr.Name.ToString();
                if (name.Contains("BehaviorDefinition"))
                {
                    var symbol = context.SemanticModel.GetDeclaredSymbol(decl);
                    if (symbol != null && !symbol.AllInterfaces.Any(i => i.Name == "IBehaviorDefinition"))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, attr.GetLocation(), "BehaviorDefinitionAttribute", "IBehaviorDefinition"));
                    }
                }
                if (name.Contains("CategoryDefinition"))
                {
                    var symbol = context.SemanticModel.GetDeclaredSymbol(decl);
                    if (symbol != null && !symbol.AllInterfaces.Any(i => i.Name == "ICategoryDefinition"))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, attr.GetLocation(), "CategoryDefinitionAttribute", "ICategoryDefinition"));
                    }
                }
            }
        }
    }
}
