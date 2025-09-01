// <copyright file="src/Zentient.Analyzers/WellKnown.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace Zentient.Analyzers.Diagnostics
{
    public static partial class WellKnown
    {
        // C# Language Types & Collections
        public const string IReadOnlyCollectionOfT = "System.Collections.Generic.IReadOnlyCollection`1";
        public const string IReadOnlyListOfT = "System.Collections.Generic.IReadOnlyList`1";
        public const string Stream = "System.IO.Stream";

        // Result & Envelope Doctrine
        public const string IResult = "Zentient.Abstractions.Results.IResult";
        public const string IResultGeneric = "Zentient.Abstractions.Results.IResult`1";
        public const string IEnvelope = "Zentient.Abstractions.Results.IEnvelope";
        public const string IEnvelopeGeneric = "Zentient.Abstractions.Results.IEnvelope`1";
        public const string IHeaderedEnvelope = "Zentient.Abstractions.Results.IHeaderedEnvelope";
        public const string IStreamableEnvelope = "Zentient.Abstractions.Results.IStreamableEnvelope";

        // Code Doctrine
        public const string ICode = "Zentient.Abstractions.Codes.ICode";
        public const string ICodeGeneric = "Zentient.Abstractions.Codes.ICode`1";

        // Error Doctrine
        public const string IError = "Zentient.Abstractions.Errors.IError";
        public const string IErrorInfo = "Zentient.Abstractions.Errors.IErrorInfo";
        public const string IErrorInfoGeneric = "Zentient.Abstractions.Errors.IErrorInfo`1";
        public const string IErrorInfoBuilder = "Zentient.Abstractions.Errors.IErrorInfoBuilder";

        // Validation Doctrine
        public const string IValidationContext = "Zentient.Abstractions.Validation.IValidationContext";
        public const string IValidationError = "Zentient.Abstractions.Validation.IValidationError";
        public const string IValidationErrorGeneric = "Zentient.Abstractions.Validation.IValidationError`1";
        public const string IValidationDefinition = "Zentient.Abstractions.Validation.Definitions.IValidationDefinition";
        public const string IValidator = "Zentient.Abstractions.Validation.IValidator";
        public const string IValidatorGeneric = "Zentient.Abstractions.Validation.IValidator`1";

        // Metadata Doctrine
        public const string IMetadata = "Zentient.Abstractions.Metadata.IMetadata";
        public const string IMetadataReader = "Zentient.Abstractions.Metadata.Readers.IMetadataReader";
        public const string IMetadataBuilder = "Zentient.Abstractions.Metadata.Builders.IMetadataBuilder";

        // Metadata Definitions
        public const string IMetadataDefinition = "Zentient.Abstractions.Metadata.Definitions.IMetadataDefinition";
        public const string IBehaviorDefinition = "Zentient.Abstractions.Metadata.Definitions.IBehaviorDefinition";
        public const string ICategoryDefinition = "Zentient.Abstractions.Metadata.Definitions.ICategoryDefinition";
        public const string IMetadataTagDefinition = "Zentient.Abstractions.Metadata.Definitions.IMetadataTagDefinition";

        // Set of all core immutable abstractions
        public static readonly ImmutableHashSet<string> ImmutableAbstractions = ImmutableHashSet.Create(
            IResult,
            IEnvelope,
            ICode,
            IError,
            IErrorInfo,
            IMetadata,
            IMetadataReader,
            IValidationContext
        );

        /// <summary>
        /// Checks if a type symbol implements any of the specified interfaces.
        /// </summary>
        /// <param name="type">The type symbol to check.</param>
        /// <param name="interfaceNames">The names of the interfaces to check against.</param>
        /// <returns>
        /// <see langword="true"/> if the type implements any of the specified interfaces; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool ImplementsAnyOf(
            INamedTypeSymbol type,
            params string[] interfaceNames)
        {
            var namesSet = interfaceNames.Length > 3 ? new HashSet<string>(interfaceNames) : null;

            if (namesSet != null)
            {
                if (type.AllInterfaces
                    .Select(iface => iface.ToDisplayString())
                    .Any(ifaceName => namesSet.Any(name => ifaceName.StartsWith(name, StringComparison.Ordinal))))
                {
                    return true;
                }
            }
            else
            {
                if (type.AllInterfaces
                    .Select(iface => iface.ToDisplayString())
                    .Any(ifaceName => interfaceNames.Any(name => ifaceName.StartsWith(name, StringComparison.Ordinal))))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a type symbol implements all of the specified interfaces.
        /// </summary>
        /// <param name="type">The type symbol to check.</param>
        /// <param name="interfaceNames">The names of the interfaces to check against.</param>
        /// <returns>
        /// <see langword="true"/> if the type implements all of the specified interfaces;otherwise, <see langword="false"/>.
        /// </returns>
        public static bool ImplementsAllOf(
            INamedTypeSymbol type,
            params string[] interfaceNames)
        {
            var namesSet = interfaceNames.Length > 3 ? new HashSet<string>(interfaceNames) : null;
            if (namesSet != null)
            {
                return type.AllInterfaces
                    .Select(iface => iface.ToDisplayString())
                    .All(ifaceName => namesSet.Any(name => ifaceName.StartsWith(name, StringComparison.Ordinal)));
            }
            else
            {
                return type.AllInterfaces
                    .Select(iface => iface.ToDisplayString())
                    .All(ifaceName => interfaceNames.Any(name => ifaceName.StartsWith(name, StringComparison.Ordinal)));
            }
        }
    }
}
