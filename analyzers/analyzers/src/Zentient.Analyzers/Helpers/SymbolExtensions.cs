// <copyright file="src/Zentient.Analyzers/SymbolExtensions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Linq;
using Microsoft.CodeAnalysis;

namespace Zentient.Analyzers.Helpers
{
    public static class SymbolExtensions
    {
        public static bool TryGetAttributeCtorArg(this INamedTypeSymbol symbol, string attributeMetadataName, out TypedConstant ctorArg)
        {
            ctorArg = default;
            var attr = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == attributeMetadataName);
            if (attr != null && attr.ConstructorArguments.Length > 0)
            {
                ctorArg = attr.ConstructorArguments[0];
                return true;
            }
            return false;
        }
    }
}

