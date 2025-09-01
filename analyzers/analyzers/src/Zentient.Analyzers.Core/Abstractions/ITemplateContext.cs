// <copyright file="src/Zentient.Analyzers/Abstractions/ITemplateContext.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Provides context information for template generation.
    /// </summary>
    public interface ITemplateContext
    {
        /// <summary>
        /// Gets the unique key for the template context.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the domain for the template context.
        /// </summary>
        string Domain { get; }
    }
}
