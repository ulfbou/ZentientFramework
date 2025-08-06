using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Zentient.Abstractions.DependencyInjection.Definitions;

namespace Zentient.Abstractions.DependencyInjection.Builders
{
    /// <summary>
    /// Represents the most versatile and DX-friendly container builder interface.
    /// Combines the best aspects of all container builder patterns with excellent developer experience.
    /// </summary>
    /// <remarks>
    /// This interface provides a comprehensive, fluent API for dependency injection container configuration
    /// with support for:
    /// - Attribute-driven automatic registration
    /// - Fluent manual registration with type safety
    /// - Assembly scanning with advanced filtering
    /// - Conditional registration based on environment/configuration
    /// - Instance and factory registration patterns
    /// - Comprehensive validation and diagnostics
    /// - Module-based organization
    /// - Async-aware building with cancellation support
    /// </remarks>
    public interface IModernContainerBuilder
    {
        // ================================================================================
        // CORE REGISTRATION API - Type-safe and fluent
        // ================================================================================

        /// <summary>
        /// Registers a service implementation using attribute-driven discovery.
        /// This is the primary registration method for services decorated with attributes.
        /// </summary>
        /// <typeparam name="TImplementation">The service implementation type.</typeparam>
        /// <returns>The builder for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// // Registers EmailService by discovering [Service] and [ProvidesContract] attributes
        /// builder.Register&lt;EmailService&gt;();
        /// </code>
        /// </example>
        IModernContainerBuilder Register<TImplementation>() 
            where TImplementation : class;

        /// <summary>
        /// Registers a service with explicit contract and implementation types.
        /// Provides full type safety and fluent configuration options.
        /// </summary>
        /// <typeparam name="TContract">The service contract type (interface/abstract class).</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        /// <param name="configure">Optional configuration for the registration.</param>
        /// <returns>The builder for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.Register&lt;IEmailService, EmailService&gt;(config => config
        ///     .AsScoped()
        ///     .WithMetadata("Provider", "SendGrid")
        ///     .WithTags("communication", "external"));
        /// </code>
        /// </example>
        IModernContainerBuilder Register<TContract, TImplementation>(
            Action<IServiceRegistrationConfig<TContract, TImplementation>>? configure = null)
            where TImplementation : class, TContract;

        /// <summary>
        /// Registers a service using a factory function.
        /// Ideal for services requiring complex initialization or runtime dependencies.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="factory">Factory function to create the service instance.</param>
        /// <param name="configure">Optional configuration for the registration.</param>
        /// <returns>The builder for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.RegisterFactory&lt;IEmailService&gt;(
        ///     provider => new EmailService(provider.GetRequiredService&lt;IConfiguration&gt;()),
        ///     config => config.AsScoped().WithMetadata("Type", "Factory"));
        /// </code>
        /// </example>
        IModernContainerBuilder RegisterFactory<TService>(
            Func<IServiceProvider, TService> factory,
            Action<IFactoryRegistrationConfig<TService>>? configure = null)
            where TService : class;

        /// <summary>
        /// Registers a singleton instance of a service.
        /// The instance will be used for all future resolutions.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="instance">The singleton instance to register.</param>
        /// <param name="configure">Optional configuration for the registration.</param>
        /// <returns>The builder for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// var logger = new ConsoleLogger();
        /// builder.RegisterInstance&lt;ILogger&gt;(logger, config => config
        ///     .WithMetadata("Type", "Console")
        ///     .AsPrimary());
        /// </code>
        /// </example>
        IModernContainerBuilder RegisterInstance<TService>(
            TService instance,
            Action<IInstanceRegistrationConfig<TService>>? configure = null)
            where TService : class;

        // ================================================================================
        // ASSEMBLY SCANNING - Powerful and flexible
        // ================================================================================

        /// <summary>
        /// Scans the specified assemblies for services with registration attributes.
        /// Automatically registers all services found based on their attribute configuration.
        /// </summary>
        /// <param name="assemblies">Assemblies to scan for attributed services.</param>
        /// <returns>The builder for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.ScanAssemblies(
        ///     Assembly.GetExecutingAssembly(),
        ///     typeof(SomeService).Assembly);
        /// </code>
        /// </example>
        IModernContainerBuilder ScanAssemblies(params Assembly[] assemblies);

        /// <summary>
        /// Scans assemblies with advanced filtering and configuration options.
        /// Provides fine-grained control over what gets registered and how.
        /// </summary>
        /// <param name="configure">Configuration for assembly scanning behavior.</param>
        /// <returns>The builder for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.ScanAssemblies(scan => scan
        ///     .FromAssemblyContaining&lt;Program&gt;()
        ///     .IncludeNamespaces("MyApp.Services", "MyApp.Infrastructure")
        ///     .ExcludeTypes(type => type.IsAbstract)
        ///     .WithDefaultLifetime(ServiceLifetime.Scoped));
        /// </code>
        /// </example>
        IModernContainerBuilder ScanAssemblies(Action<IAssemblyScanConfig> configure);

        /// <summary>
        /// Convenience method to scan the calling assembly for services.
        /// </summary>
        /// <returns>The builder for fluent chaining.</returns>
        IModernContainerBuilder ScanCurrentAssembly();

        // ================================================================================
        // CONDITIONAL REGISTRATION - Environment and configuration aware
        // ================================================================================

        /// <summary>
        /// Registers services conditionally based on a runtime predicate.
        /// Services are only registered if the condition evaluates to true.
        /// </summary>
        /// <param name="condition">Condition that determines if registration should occur.</param>
        /// <param name="configure">Configuration for conditional services.</param>
        /// <returns>The builder for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.RegisterWhen(
        ///     () => Environment.GetEnvironmentVariable("USE_MOCK_SERVICES") == "true",
        ///     conditional => conditional
        ///         .Register&lt;IMockEmailService, MockEmailService&gt;()
        ///         .Register&lt;IMockPaymentService, MockPaymentService&gt;());
        /// </code>
        /// </example>
        IModernContainerBuilder RegisterWhen(
            Func<bool> condition,
            Action<IConditionalRegistration> configure);

        /// <summary>
        /// Registers services for specific environments only.
        /// Useful for development, testing, or production-specific services.
        /// </summary>
        /// <param name="environments">Environment names that enable these registrations.</param>
        /// <param name="configure">Configuration for environment-specific services.</param>
        /// <returns>The builder for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.RegisterForEnvironments(
        ///     new[] { "Development", "Testing" },
        ///     env => env.Register&lt;IEmailService, MockEmailService&gt;());
        /// </code>
        /// </example>
        IModernContainerBuilder RegisterForEnvironments(
            string[] environments,
            Action<IConditionalRegistration> configure);

        // ================================================================================
        // MODULE SYSTEM - Organized and reusable
        // ================================================================================

        /// <summary>
        /// Registers a service module containing related service configurations.
        /// Modules provide a way to organize and reuse service registrations.
        /// </summary>
        /// <typeparam name="TModule">The module type to register.</typeparam>
        /// <returns>The builder for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.UseModule&lt;EmailModule&gt;()
        ///        .UseModule&lt;DatabaseModule&gt;()
        ///        .UseModule&lt;LoggingModule&gt;();
        /// </code>
        /// </example>
        IModernContainerBuilder UseModule<TModule>() where TModule : IContainerModule, new();

        /// <summary>
        /// Registers a service module instance.
        /// </summary>
        /// <param name="module">The module instance to register.</param>
        /// <returns>The builder for fluent chaining.</returns>
        IModernContainerBuilder UseModule(IContainerModule module);

        // ================================================================================
        // VALIDATION AND DIAGNOSTICS - Developer-friendly feedback
        // ================================================================================

        /// <summary>
        /// Validates the current container configuration asynchronously.
        /// Returns detailed information about potential issues and misconfigurations.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the validation operation.</param>
        /// <returns>A validation report containing errors, warnings, and suggestions.</returns>
        /// <example>
        /// <code>
        /// var report = await builder.ValidateAsync();
        /// if (!report.IsValid)
        /// {
        ///     Console.WriteLine($"Found {report.Errors.Count} errors:");
        ///     foreach (var error in report.Errors)
        ///         Console.WriteLine($"  - {error.Message}");
        /// }
        /// </code>
        /// </example>
        Task<IValidationReport> ValidateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets comprehensive diagnostic information about the container configuration.
        /// Useful for debugging, optimization, and understanding service dependencies.
        /// </summary>
        /// <returns>Diagnostic information about the container.</returns>
        IDiagnosticInfo GetDiagnostics();

        // ================================================================================
        // BUILDING AND LIFECYCLE - Robust and async-aware
        // ================================================================================

        /// <summary>
        /// Builds the container and returns a service provider.
        /// Validates the configuration and throws descriptive exceptions for issues.
        /// </summary>
        /// <returns>The configured service provider.</returns>
        /// <exception cref="ContainerConfigurationException">Thrown when configuration is invalid.</exception>
        IServiceProvider Build();

        /// <summary>
        /// Builds the container asynchronously with optional validation.
        /// Provides better error reporting and supports cancellation.
        /// </summary>
        /// <param name="validateFirst">Whether to validate configuration before building.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The configured service provider.</returns>
        Task<IServiceProvider> BuildAsync(
            bool validateFirst = true, 
            CancellationToken cancellationToken = default);

        // ================================================================================
        // CONFIGURATION PROPERTIES - Behavior customization
        // ================================================================================

        /// <summary>
        /// Gets or sets whether to automatically register framework services (IServiceProvider, etc.).
        /// </summary>
        bool AutoRegisterFrameworkServices { get; set; }

        /// <summary>
        /// Gets or sets whether to validate registrations immediately when they're added.
        /// </summary>
        bool EagerValidation { get; set; }

        /// <summary>
        /// Gets or sets whether to allow multiple registrations for the same service type.
        /// </summary>
        bool AllowMultipleRegistrations { get; set; }

        /// <summary>
        /// Gets a read-only collection of currently registered service types.
        /// Useful for diagnostics and debugging.
        /// </summary>
        IReadOnlyCollection<Type> RegisteredTypes { get; }
    }

    // ================================================================================
    // SUPPORTING CONFIGURATION INTERFACES - Fluent and type-safe
    // ================================================================================

    /// <summary>
    /// Configuration interface for service registrations with full type safety.
    /// </summary>
    /// <typeparam name="TContract">The service contract type.</typeparam>
    /// <typeparam name="TImplementation">The service implementation type.</typeparam>
    public interface IServiceRegistrationConfig<TContract, TImplementation>
        where TImplementation : class, TContract
    {
        /// <summary>Sets the service as a transient (new instance each time).</summary>
        IServiceRegistrationConfig<TContract, TImplementation> AsTransient();
        
        /// <summary>Sets the service as scoped (one instance per scope).</summary>
        IServiceRegistrationConfig<TContract, TImplementation> AsScoped();
        
        /// <summary>Sets the service as a singleton (one instance for lifetime).</summary>
        IServiceRegistrationConfig<TContract, TImplementation> AsSingleton();
        
        /// <summary>Adds metadata to the service registration.</summary>
        IServiceRegistrationConfig<TContract, TImplementation> WithMetadata(string key, object value);
        
        /// <summary>Adds semantic tags for filtering and discovery.</summary>
        IServiceRegistrationConfig<TContract, TImplementation> WithTags(params string[] tags);
        
        /// <summary>Marks this as the primary implementation for the contract.</summary>
        IServiceRegistrationConfig<TContract, TImplementation> AsPrimary();
        
        /// <summary>Sets a key for this registration (for keyed services).</summary>
        IServiceRegistrationConfig<TContract, TImplementation> WithKey(string key);
    }

    /// <summary>
    /// Configuration interface for factory registrations.
    /// </summary>
    /// <typeparam name="TService">The service type being registered.</typeparam>
    public interface IFactoryRegistrationConfig<TService> where TService : class
    {
        /// <summary>Sets the service as a transient.</summary>
        IFactoryRegistrationConfig<TService> AsTransient();
        
        /// <summary>Sets the service as scoped.</summary>
        IFactoryRegistrationConfig<TService> AsScoped();
        
        /// <summary>Sets the service as a singleton.</summary>
        IFactoryRegistrationConfig<TService> AsSingleton();
        
        /// <summary>Adds metadata to the registration.</summary>
        IFactoryRegistrationConfig<TService> WithMetadata(string key, object value);
        
        /// <summary>Adds tags to the registration.</summary>
        IFactoryRegistrationConfig<TService> WithTags(params string[] tags);
    }

    /// <summary>
    /// Configuration interface for instance registrations.
    /// </summary>
    /// <typeparam name="TService">The service type being registered.</typeparam>
    public interface IInstanceRegistrationConfig<TService> where TService : class
    {
        /// <summary>Adds metadata to the registration.</summary>
        IInstanceRegistrationConfig<TService> WithMetadata(string key, object value);
        
        /// <summary>Adds tags to the registration.</summary>
        IInstanceRegistrationConfig<TService> WithTags(params string[] tags);
        
        /// <summary>Marks this as the primary instance.</summary>
        IInstanceRegistrationConfig<TService> AsPrimary();
    }

    /// <summary>
    /// Configuration interface for assembly scanning operations.
    /// </summary>
    public interface IAssemblyScanConfig
    {
        /// <summary>Includes the specified assembly in the scan.</summary>
        IAssemblyScanConfig FromAssembly(Assembly assembly);
        
        /// <summary>Includes the assembly containing the specified type.</summary>
        IAssemblyScanConfig FromAssemblyContaining<T>();
        
        /// <summary>Includes only types from the specified namespaces.</summary>
        IAssemblyScanConfig IncludeNamespaces(params string[] namespaces);
        
        /// <summary>Excludes types matching the predicate.</summary>
        IAssemblyScanConfig ExcludeTypes(Func<Type, bool> predicate);
        
        /// <summary>Sets the default lifetime for scanned services without explicit lifetime.</summary>
        IAssemblyScanConfig WithDefaultLifetime(ServiceLifetime lifetime);
    }

    /// <summary>
    /// Interface for conditional service registration.
    /// </summary>
    public interface IConditionalRegistration
    {
        /// <summary>Conditionally registers a service implementation.</summary>
        IConditionalRegistration Register<TImplementation>() where TImplementation : class;
        
        /// <summary>Conditionally registers a service with explicit types.</summary>
        IConditionalRegistration Register<TContract, TImplementation>(
            Action<IServiceRegistrationConfig<TContract, TImplementation>>? configure = null)
            where TImplementation : class, TContract;
    }

    /// <summary>
    /// Interface for container modules that group related services.
    /// </summary>
    public interface IContainerModule
    {
        /// <summary>Gets the module name for identification.</summary>
        string Name { get; }
        
        /// <summary>Gets the module version.</summary>
        string Version { get; }
        
        /// <summary>Configures the services provided by this module.</summary>
        void Configure(IModernContainerBuilder builder);
    }

    /// <summary>
    /// Comprehensive validation report for container configuration.
    /// </summary>
    public interface IValidationReport
    {
        /// <summary>Gets whether the configuration is valid.</summary>
        bool IsValid { get; }
        
        /// <summary>Gets all validation errors found.</summary>
        IReadOnlyList<ValidationIssue> Errors { get; }
        
        /// <summary>Gets all validation warnings found.</summary>
        IReadOnlyList<ValidationIssue> Warnings { get; }
        
        /// <summary>Gets the total number of services validated.</summary>
        int ServicesValidated { get; }
        
        /// <summary>Gets a human-readable summary of the validation.</summary>
        string Summary { get; }
    }

    /// <summary>
    /// Diagnostic information about the container configuration.
    /// </summary>
    public interface IDiagnosticInfo
    {
        /// <summary>Gets the total number of registered services.</summary>
        int TotalRegistrations { get; }
        
        /// <summary>Gets the distribution of services by lifetime.</summary>
        IReadOnlyDictionary<ServiceLifetime, int> LifetimeDistribution { get; }
        
        /// <summary>Gets statistics about registered contracts.</summary>
        IReadOnlyList<ServiceStatistic> ServiceStatistics { get; }
        
        /// <summary>Gets the list of registered modules.</summary>
        IReadOnlyList<string> RegisteredModules { get; }
    }

    /// <summary>
    /// Statistics about a registered service.
    /// </summary>
    public class ServiceStatistic
    {
        /// <summary>Gets the service type.</summary>
        public Type ServiceType { get; init; } = typeof(object);
        
        /// <summary>Gets the number of implementations registered.</summary>
        public int ImplementationCount { get; init; }
        
        /// <summary>Gets whether there's a primary implementation.</summary>
        public bool HasPrimary { get; init; }
        
        /// <summary>Gets the service lifetime.</summary>
        public ServiceLifetime Lifetime { get; init; }
    }

    /// <summary>
    /// Represents a validation issue (error or warning).
    /// </summary>
    public class ValidationIssue
    {
        /// <summary>Gets the issue message.</summary>
        public string Message { get; init; } = string.Empty;
        
        /// <summary>Gets the affected service type, if applicable.</summary>
        public Type? ServiceType { get; init; }
        
        /// <summary>Gets additional details about the issue.</summary>
        public string? Details { get; init; }
        
        /// <summary>Gets the severity level.</summary>
        public IssueSeverity Severity { get; init; }
    }

    /// <summary>
    /// Severity levels for validation issues.
    /// </summary>
    public enum IssueSeverity
    {
        /// <summary>Informational message.</summary>
        Info,
        
        /// <summary>Warning that should be addressed.</summary>
        Warning,
        
        /// <summary>Error that must be fixed.</summary>
        Error,
        
        /// <summary>Critical error preventing container operation.</summary>
        Critical
    }

    /// <summary>
    /// Exception thrown when container configuration is invalid.
    /// </summary>
    public class ContainerConfigurationException : Exception
    {
        /// <summary>Initializes a new instance with a message.</summary>
        public ContainerConfigurationException(string message) : base(message) { }
        
        /// <summary>Initializes a new instance with a message and inner exception.</summary>
        public ContainerConfigurationException(string message, Exception innerException) 
            : base(message, innerException) { }
        
        /// <summary>Gets the validation report that caused this exception.</summary>
        public IValidationReport? ValidationReport { get; init; }
    }
}
