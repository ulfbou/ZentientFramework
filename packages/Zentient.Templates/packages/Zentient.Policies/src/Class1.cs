// <copyright file="PackageService.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Zentient.Template
{
    /// <summary>
    /// Provides core functionalities and information for the Zentient package template.
    /// This class serves as an initial entry point and can be extended or replaced
    /// with actual business logic for your new package.
    /// </summary>
    public class PackageService
    {
        private readonly string _packageName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageService"/> class.
        /// </summary>
        /// <param name="packageName">Optional: The name of the package. Defaults to the assembly name if not provided.</param>
        public PackageService(string? packageName = null)
        {
            // Use provided name, otherwise fall back to the assembly name for dynamic behavior.
            _packageName = packageName ?? Assembly.GetExecutingAssembly().GetName().Name ?? "Zentient.Template.Unknown";
        }

        /// <summary>
        /// Gets the name of the Zentient package.
        /// This property is initialized during construction and reflects the package's identity.
        /// </summary>
        /// <value>A <see cref="string"/> representing the name of the package.</value>
        public string PackageName => _packageName;

        /// <summary>
        /// Gets a simple greeting message from this Zentient package.
        /// This method demonstrates basic functionality and string interpolation.
        /// </summary>
        /// <returns>A <see cref="string"/> containing a greeting message.</returns>
        [SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "Method form is used to demonstrate a template for future expansion and maintain consistency with other service methods.")]
        public string GetGreeting()
            => $"Hello from {PackageName}!";

        /// <summary>
        /// Demonstrates a placeholder method for future logic.
        /// </summary>
        /// <param name="input">A string input.</param>
        /// <returns>The input string transformed.</returns>
        public string ProcessInput(string input)
        {
            // This is a placeholder for actual business logic.
            // You can replace this with your package's core functionality.
            return $"Processed: {input} by {PackageName}";
        }
    }
}