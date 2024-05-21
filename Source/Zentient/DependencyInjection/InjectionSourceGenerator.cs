//
// class: InjectionSourceGenerator
//
// Description:
// 
// 
// MIT License
//
// Copyright (c) 2024 Ulf Bourelius
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Zentient.DependencyInjection;

[Generator]
public class InjectionSourceGenerator : ISourceGenerator
{
    public InjectionSourceGenerator()
    {
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a syntax receiver that will be created for each generation pass
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Retrieve the populated receiver
        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            return;

        // Iterate over the candidate methods and generate the necessary overrides
        foreach (var method in receiver.CandidateMethods)
        {
            var methodSymbol = context.Compilation.GetSemanticModel(method.SyntaxTree).GetDeclaredSymbol(method) as IMethodSymbol;

            if (methodSymbol == null)
                continue;

            var classSymbol = methodSymbol.ContainingType;
            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            var className = classSymbol.Name;
            var baseClassName = classSymbol?.BaseType?.Name;

            var methodName = methodSymbol.Name;
            var parameters = methodSymbol.Parameters;

            var injectedParameters = parameters
                .Where(p => p.GetAttributes().Any(attr => attr.AttributeClass.Name == nameof(InjectAttribute)))
                .ToList();

            var sourceBuilder = new StringBuilder($@"
namespace {namespaceName}
{{
    public partial class {className} : {baseClassName}
    {{
        public override void {methodName}({string.Join(", ", parameters.Select(p => $"{p.Type} {p.Name}"))})
        {{
            var serviceProvider = GetServiceProvider();
");

            foreach (var parameter in injectedParameters)
            {
                sourceBuilder.AppendLine($"            var {parameter.Name} = serviceProvider.GetService<{parameter.Type}>();");
            }

            sourceBuilder.AppendLine($@"
            base.{methodName}({string.Join(", ", parameters.Select(p => p.Name))});
        }}

        private IServiceProvider GetServiceProvider()
        {{
            // Implement logic to retrieve the service provider
            throw new NotImplementedException();
        }}
    }}
}}");

            context.AddSource($"{className}_Generated.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }

    // Define a receiver to collect candidate methods for generation
    class SyntaxReceiver : ISyntaxReceiver
    {
        public List<MethodDeclarationSyntax> CandidateMethods { get; } = new List<MethodDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Collect methods with the [Inject] attribute
            if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax)
            {
                if (methodDeclarationSyntax.ParameterList.Parameters
                    .Any(p => p.AttributeLists
                        .SelectMany(al => al.Attributes)
                        .Any(a => a.Name.ToString() == nameof(InjectAttribute))))
                {
                    CandidateMethods.Add(methodDeclarationSyntax);
                }
            }
        }
    }
}
