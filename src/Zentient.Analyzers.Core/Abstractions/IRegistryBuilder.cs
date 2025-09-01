// <copyright file="src/Zentient.Analyzers/Abstractions/IRegistryBuilder.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Provides a fluent interface for building a registry of code instructions, stubs, and templates.
    /// </summary>
    public interface IRegistryBuilder
    {
        /// <summary>
        /// Adds stub instructions to the registry.
        /// </summary>
        /// <param name="key">The stub key.</param>
        /// <param name="domain">The stub domain.</param>
        /// <returns>The stub builder instance.</returns>
        IStubBuilder AddStub(string key, string domain);

        /// <summary>
        /// Adds template instructions to the registry.
        /// </summary>
        /// <param name="key">The template key.</param>
        /// <param name="domain">The template domain.</param>
        /// <returns>The template builder instance.</returns>
        ITemplateBuilder AddTemplate(string key, string domain);

        /// <summary>
        /// Builds and returns the registry.
        /// </summary>
        /// <returns>The registry instance.</returns>
        IRegistry Build();
    }
}
