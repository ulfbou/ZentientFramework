// <copyright file="src/Zentient.Analyzers/Abstractions/ICodeBuilder.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Provides a fluent interface for building code artifacts.
    /// </summary>
    /// <typeparam name="TBuilder">The builder type implementing this interface.</typeparam>
    public interface ICodeBuilder<out TBuilder>
        where TBuilder : ICodeBuilder<TBuilder>
    {
        /// <summary>
        /// Specifies a required specification key for the code artifact.
        /// </summary>
        /// <param name="specKey">The specification key.</param>
        /// <returns>The builder instance.</returns>
        TBuilder Require(string specKey);

        /// <summary>
        /// Sets the namespace for the code artifact.
        /// </summary>
        /// <param name="ns">The namespace name.</param>
        /// <returns>The builder instance.</returns>
        TBuilder WithNamespace(string ns);

        /// <summary>
        /// Adds using directives to the code artifact.
        /// </summary>
        /// <param name="usings">The using directives.</param>
        /// <returns>The builder instance.</returns>
        TBuilder WithUsings(params string[] usings);

        /// <summary>
        /// Applies a mutation function to the code artifact.
        /// </summary>
        /// <param name="mutate">A function that mutates the code string.</param>
        /// <returns>The builder instance.</returns>
        TBuilder WithMutation(Func<string, string> mutate);

        /// <summary>
        /// Builds and returns the code artifact as a string.
        /// </summary>
        /// <returns>The generated code.</returns>
        string Build();
    }
}
