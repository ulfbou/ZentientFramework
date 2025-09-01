// <copyright file="src/Zentient.Analyzers/INamedTypeSymbolExtensions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;

using Zentient.Analyzers.Diagnostics;

namespace Zentient.Analyzers.Internal
{
    /// <summary>Extension methods for <see cref="INamedTypeSymbol"/>.</summary>
    internal static class INamedTypeSymbolExtensions
    {
        /// <summary>
        /// Determines whether the <paramref name="type"/> is assignable to the specified <paramref name="returnType"/>.
        /// </summary>
        /// <param name="type">The source named type symbol.</param>
        /// <param name="returnType">The target type symbol.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="type"/> is assignable to <paramref name="returnType"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsAssignableTo(this INamedTypeSymbol type, ITypeSymbol returnType)
            => type.AllInterfaces.Contains(returnType, SymbolEqualityComparer.Default) ||
               type.BaseType?.Equals(returnType, SymbolEqualityComparer.Default) == true ||
               SymbolEqualityComparer.Default.Equals(type, returnType);

        /// <summary>
        /// Determines whether the specified <see cref="INamedTypeSymbol"/> implements any of the well-known immutable abstractions.
        /// </summary>
        /// <param name="type">The named type symbol to check.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="type"/> implements any interface listed in <see cref="WellKnown.ImmutableAbstractions"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsResultAbstraction(this INamedTypeSymbol type)
            => WellKnown.ImplementsAnyOf(
                type: type,
                interfaceNames: WellKnown.ImmutableAbstractions.ToArray());

        /// <summary>
        /// Determines whether the specified type implements any of the well-known immutable abstractions.
        /// </summary>
        /// <param name="type">The type symbol to check.</param>
        /// <returns>
        /// <c>true</c> if the type implements any immutable abstraction; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsImmutableAbstraction(this INamedTypeSymbol type)
            => WellKnown.ImplementsAnyOf(type, WellKnown.ImmutableAbstractions.ToArray());

        /// <summary>
        /// Determines whether the specified type is a validation context.
        /// </summary>
        /// <param name="type">The type symbol to check.</param>
        /// <returns>
        /// <c>true</c> if the type is a validation context; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidationContext(this INamedTypeSymbol type)
            => type.ToDisplayString() == WellKnown.IValidationContext;
    }
}
