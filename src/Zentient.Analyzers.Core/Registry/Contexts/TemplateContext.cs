// <copyright file="src/Zentient.Analyzers/Registry/TemplateContext.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Registry.Contexts
{
    using Zentient.Analyzers.Abstractions;

    /// <summary>Provides context information for template generation.</summary>
    /// <param name="Key">The unique key for the template context.</param>
    /// <param name="Domain">The domain for the template context.</param>
    internal sealed record TemplateContext(string Key, string Domain) : ITemplateContext;
}
