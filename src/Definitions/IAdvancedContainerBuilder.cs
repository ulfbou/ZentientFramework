using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Zentient.Abstractions.DependencyInjection.Definitions;

namespace Zentient.Abstractions.DependencyInjection.Builders
{
    /// <summary>
    /// Represents a comprehensive, container-agnostic builder for orchestrating service registrations.
    /// Provides a fluent API for building dependency injection containers with advanced features
    /// including conditional registration, assembly scanning, validation, and lifecycle management.
    /// </summary>
    /// <remarks>
    /// This builder supports multiple registration patterns:
    /// - Attribute-based automatic registration
    /// - Fluent manual registration  
    /// - Conditional and environment-based registration
    /// - Assembly scanning and type discovery
    /// - Instance and factory registration
    /// - Validation and diagnostics
    /// </remarks>
    public interface IAdvancedContainerBuilder
    {
        // ========================================================================================
        // CORE REGISTRATION METHODS
        // ========================================================================================

        /// <summary>
        /// Registers a service implementation by discovering its metadata via attributes.
        /// This is the primary method for attribute-driven registration.
        /// </summary>
        /// <typeparam name="TImplementation">The service implementation type.</typeparam>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.Register&lt;EmailService&gt;();
        /// </code>
        /// </example>
        IAdvancedContainerBuilder Register<TImplementation>() 
            where TImplementation : class;

        /// <summary>
        /// Registers a service implementation by discovering its metadata via attributes.
        /// </summary>
        /// <param name="implementationType">The service implementation type.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IAdvancedContainerBuilder Register(Type implementationType);

        /// <summary>
        /// Registers a service using a fluent definition builder.
        /// This provides maximum control over service registration.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <typeparam name="TImplementation">The service implementation type.</typeparam>
        /// <param name="configure">Delegate to configure the registration.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.Register&lt;IEmailService, EmailService&gt;(reg => reg
        ///     .WithLifetime(ServiceLifetime.Scoped)
        ///     .WithMetadata("Provider", "SendGrid"));
        /// </code>
        /// </example>
        IAdvancedContainerBuilder Register<TContract, TImplementation>(
            Action<IFluentRegistrationBuilder<TContract, TImplementation>>? configure = null)
            where TImplementation : class, TContract;

        // ========================================================================================
        // INSTANCE REGISTRATION
        // ========================================================================================

        /// <summary>
        /// Registers a singleton instance of a service.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="instance">The service instance.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// var logger = new ConsoleLogger();
        /// builder.RegisterInstance&lt;ILogger&gt;(logger);
        /// </code>
        /// </example>
        IAdvancedContainerBuilder RegisterInstance<TService>(TService instance)
            where TService : class;

        /// <summary>
        /// Registers a singleton instance with additional configuration.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="instance">The service instance.</param>
        /// <param name="configure">Delegate to configure additional metadata.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IAdvancedContainerBuilder RegisterInstance<TService>(
            TService instance,
            Action<IInstanceRegistrationBuilder<TService>> configure)
            where TService : class;

        // ========================================================================================
        // FACTORY REGISTRATION
        // ========================================================================================

        /// <summary>
        /// Registers a service using a factory delegate.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">Factory delegate to create the service.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.RegisterFactory&lt;IEmailService&gt;(
        ///     resolver => new EmailService(resolver.GetService&lt;IConfiguration&gt;()),
        ///     ServiceLifetime.Scoped);
        /// </code>
        /// </example>
        IAdvancedContainerBuilder RegisterFactory<TService>(
            Func<IServiceProvider, TService> factory,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class;

        /// <summary>
        /// Registers an asynchronous service factory.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">Async factory delegate to create the service.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IAdvancedContainerBuilder RegisterAsyncFactory<TService>(
            Func<IServiceProvider, Task<TService>> factory,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class;

        // ========================================================================================
        // ASSEMBLY SCANNING AND DISCOVERY
        // ========================================================================================

        /// <summary>
        /// Scans and registers all services from the specified assemblies using attributes.
        /// </summary>
        /// <param name="assemblies">Assemblies to scan for services.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.RegisterFromAssemblies(
        ///     Assembly.GetExecutingAssembly(),
        ///     Assembly.GetAssembly(typeof(SomeType)));
        /// </code>
        /// </example>
        IAdvancedContainerBuilder RegisterFromAssemblies(params Assembly[] assemblies);

        /// <summary>
        /// Scans and registers services from assemblies with advanced filtering options.
        /// </summary>
        /// <param name="configure">Delegate to configure assembly scanning options.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.RegisterFromAssemblies(scan => scan
        ///     .FromAssemblyContaining&lt;Program&gt;()
        ///     .Where(type => type.Namespace?.StartsWith("MyApp.Services") == true)
        ///     .ExcludeTypes(typeof(BaseService)));
        /// </code>
        /// </example>
        IAdvancedContainerBuilder RegisterFromAssemblies(
            Action<IAssemblyScanningBuilder> configure);

        /// <summary>
        /// Registers services from the current assembly.
        /// </summary>
        /// <returns>The builder instance for fluent chaining.</returns>
        IAdvancedContainerBuilder RegisterFromCurrentAssembly();

        // ========================================================================================
        // CONDITIONAL REGISTRATION
        // ========================================================================================

        /// <summary>
        /// Registers services conditionally based on a predicate.
        /// </summary>
        /// <param name="condition">Condition that must be true for registration.</param>
        /// <param name="configure">Delegate to configure conditional registrations.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.RegisterIf(
        ///     () => Environment.GetEnvironmentVariable("FEATURE_EMAIL") == "true",
        ///     conditional => conditional.Register&lt;AdvancedEmailService&gt;());
        /// </code>
        /// </example>
        IAdvancedContainerBuilder RegisterIf(
            Func<bool> condition,
            Action<IConditionalRegistrationBuilder> configure);

        /// <summary>
        /// Registers services conditionally based on environment.
        /// </summary>
        /// <param name="environmentNames">Environment names to match.</param>
        /// <param name="configure">Delegate to configure environment-specific registrations.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.RegisterForEnvironments(
        ///     new[] { "Development", "Testing" },
        ///     env => env.Register&lt;MockEmailService&gt;());
        /// </code>
        /// </example>
        IAdvancedContainerBuilder RegisterForEnvironments(
            string[] environmentNames,
            Action<IConditionalRegistrationBuilder> configure);

        // ========================================================================================
        // MODULE MANAGEMENT
        // ========================================================================================

        /// <summary>
        /// Registers a service module that contains multiple related service registrations.
        /// </summary>
        /// <typeparam name="TModule">The module type.</typeparam>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.AddModule&lt;EmailModule&gt;();
        /// </code>
        /// </example>
        IAdvancedContainerBuilder AddModule<TModule>()
            where TModule : IServiceModule, new();

        /// <summary>
        /// Registers a service module instance.
        /// </summary>
        /// <param name="module">The module instance to register.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IAdvancedContainerBuilder AddModule(IServiceModule module);

        // ========================================================================================
        // VALIDATION AND DIAGNOSTICS
        // ========================================================================================

        /// <summary>
        /// Validates the container configuration and returns validation results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the validation process.</param>
        /// <returns>A task containing validation results.</returns>
        /// <example>
        /// <code>
        /// var results = await builder.ValidateAsync();
        /// if (!results.IsValid)
        /// {
        ///     foreach (var error in results.Errors)
        ///         Console.WriteLine($"Validation Error: {error.Message}");
        /// }
        /// </code>
        /// </example>
        Task<IContainerValidationReport> ValidateAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets diagnostic information about the current container configuration.
        /// </summary>
        /// <returns>Container diagnostic information.</returns>
        IContainerDiagnostics GetDiagnostics();

        // ========================================================================================
        // CONTAINER BUILDING AND LIFECYCLE
        // ========================================================================================

        /// <summary>
        /// Builds and returns the finalized service provider.
        /// </summary>
        /// <returns>The configured service provider.</returns>
        /// <exception cref="InvalidOperationException">Thrown when container configuration is invalid.</exception>
        IServiceProvider Build();

        /// <summary>
        /// Builds the container asynchronously with validation.
        /// </summary>
        /// <param name="validateFirst">Whether to validate before building.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task containing the configured service provider.</returns>
        Task<IServiceProvider> BuildAsync(
            bool validateFirst = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a child builder that inherits from this builder's configuration.
        /// </summary>
        /// <returns>A new child container builder.</returns>
        IAdvancedContainerBuilder CreateChild();

        // ========================================================================================
        // CONFIGURATION AND PROPERTIES
        // ========================================================================================

        /// <summary>
        /// Gets or sets whether to automatically register common framework services.
        /// </summary>
        bool AutoRegisterFrameworkServices { get; set; }

        /// <summary>
        /// Gets or sets whether to validate service registrations immediately upon registration.
        /// </summary>
        bool ValidateOnRegistration { get; set; }

        /// <summary>
        /// Gets or sets whether to allow duplicate service registrations.
        /// </summary>
        bool AllowDuplicateRegistrations { get; set; }

        /// <summary>
        /// Gets the current container configuration settings.
        /// </summary>
        IContainerConfiguration Configuration { get; }

        /// <summary>
        /// Gets a read-only view of currently registered services.
        /// </summary>
        IReadOnlyCollection<Type> RegisteredServiceTypes { get; }
    }

    // ========================================================================================
    // SUPPORTING INTERFACES
    // ========================================================================================

    /// <summary>
    /// Builder for configuring service registrations with fluent syntax.
    /// </summary>
    /// <typeparam name="TContract">The service contract type.</typeparam>
    /// <typeparam name="TImplementation">The service implementation type.</typeparam>
    public interface IFluentRegistrationBuilder<TContract, TImplementation>
        where TImplementation : class, TContract
    {
        /// <summary>Sets the service lifetime.</summary>
        IFluentRegistrationBuilder<TContract, TImplementation> WithLifetime(ServiceLifetime lifetime);
        
        /// <summary>Adds metadata to the service registration.</summary>
        IFluentRegistrationBuilder<TContract, TImplementation> WithMetadata(string key, object value);
        
        /// <summary>Adds tags to the service registration.</summary>
        IFluentRegistrationBuilder<TContract, TImplementation> WithTags(params string[] tags);
        
        /// <summary>Sets the service as primary for its contract.</summary>
        IFluentRegistrationBuilder<TContract, TImplementation> AsPrimary();
        
        /// <summary>Sets a service key for keyed registration.</summary>
        IFluentRegistrationBuilder<TContract, TImplementation> WithKey(string key);
        
        /// <summary>Sets the service category.</summary>
        IFluentRegistrationBuilder<TContract, TImplementation> InCategory(string category);
        
        /// <summary>Sets the service description.</summary>
        IFluentRegistrationBuilder<TContract, TImplementation> WithDescription(string description);
    }

    /// <summary>
    /// Builder for configuring instance registrations.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    public interface IInstanceRegistrationBuilder<TService>
        where TService : class
    {
        /// <summary>Adds metadata to the instance registration.</summary>
        IInstanceRegistrationBuilder<TService> WithMetadata(string key, object value);
        
        /// <summary>Adds tags to the instance registration.</summary>
        IInstanceRegistrationBuilder<TService> WithTags(params string[] tags);
        
        /// <summary>Sets the instance as primary for its type.</summary>
        IInstanceRegistrationBuilder<TService> AsPrimary();
        
        /// <summary>Sets a service key for keyed registration.</summary>
        IInstanceRegistrationBuilder<TService> WithKey(string key);
    }

    /// <summary>
    /// Builder for configuring assembly scanning options.
    /// </summary>
    public interface IAssemblyScanningBuilder
    {
        /// <summary>Adds an assembly to scan.</summary>
        IAssemblyScanningBuilder FromAssembly(Assembly assembly);
        
        /// <summary>Adds an assembly containing the specified type.</summary>
        IAssemblyScanningBuilder FromAssemblyContaining<T>();
        
        /// <summary>Filters types using a predicate.</summary>
        IAssemblyScanningBuilder Where(Func<Type, bool> predicate);
        
        /// <summary>Excludes specific types from scanning.</summary>
        IAssemblyScanningBuilder ExcludeTypes(params Type[] types);
        
        /// <summary>Only includes types from specific namespaces.</summary>
        IAssemblyScanningBuilder FromNamespaces(params string[] namespaces);
        
        /// <summary>Only includes types implementing specific interfaces.</summary>
        IAssemblyScanningBuilder ImplementingInterface<TInterface>();
        
        /// <summary>Only includes types inheriting from specific base class.</summary>
        IAssemblyScanningBuilder InheritingFrom<TBaseClass>();
    }

    /// <summary>
    /// Builder for conditional registrations.
    /// </summary>
    public interface IConditionalRegistrationBuilder
    {
        /// <summary>Registers a service implementation conditionally.</summary>
        IConditionalRegistrationBuilder Register<TImplementation>() 
            where TImplementation : class;
        
        /// <summary>Registers a service with fluent configuration conditionally.</summary>
        IConditionalRegistrationBuilder Register<TContract, TImplementation>(
            Action<IFluentRegistrationBuilder<TContract, TImplementation>>? configure = null)
            where TImplementation : class, TContract;
    }

    /// <summary>
    /// Represents a service module containing related service registrations.
    /// </summary>
    public interface IServiceModule
    {
        /// <summary>
        /// Gets the module name for identification and diagnostics.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the module version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Configures services in this module.
        /// </summary>
        /// <param name="builder">The container builder to configure.</param>
        void ConfigureServices(IAdvancedContainerBuilder builder);
    }

    /// <summary>
    /// Container configuration settings.
    /// </summary>
    public interface IContainerConfiguration
    {
        /// <summary>Gets whether framework services are auto-registered.</summary>
        bool AutoRegisterFrameworkServices { get; }
        
        /// <summary>Gets whether validation occurs on registration.</summary>
        bool ValidateOnRegistration { get; }
        
        /// <summary>Gets whether duplicate registrations are allowed.</summary>
        bool AllowDuplicateRegistrations { get; }
        
        /// <summary>Gets custom configuration properties.</summary>
        IReadOnlyDictionary<string, object> Properties { get; }
    }

    /// <summary>
    /// Container validation report containing validation results.
    /// </summary>
    public interface IContainerValidationReport
    {
        /// <summary>Gets whether the validation was successful.</summary>
        bool IsValid { get; }
        
        /// <summary>Gets validation errors if any.</summary>
        IReadOnlyCollection<ValidationError> Errors { get; }
        
        /// <summary>Gets validation warnings if any.</summary>
        IReadOnlyCollection<ValidationWarning> Warnings { get; }
        
        /// <summary>Gets the validation summary.</summary>
        string Summary { get; }
        
        /// <summary>Gets the total number of registrations validated.</summary>
        int TotalRegistrations { get; }
    }

    /// <summary>
    /// Container diagnostic information.
    /// </summary>
    public interface IContainerDiagnostics
    {
        /// <summary>Gets the total number of registered services.</summary>
        int TotalRegistrations { get; }
        
        /// <summary>Gets registrations grouped by lifetime.</summary>
        IReadOnlyDictionary<ServiceLifetime, int> RegistrationsByLifetime { get; }
        
        /// <summary>Gets the most commonly registered contracts.</summary>
        IReadOnlyCollection<ContractStatistics> ContractStatistics { get; }
        
        /// <summary>Gets potential configuration issues.</summary>
        IReadOnlyCollection<string> PotentialIssues { get; }
        
        /// <summary>Gets modules registered in the container.</summary>
        IReadOnlyCollection<string> RegisteredModules { get; }
    }

    /// <summary>
    /// Statistics about a specific contract type.
    /// </summary>
    public interface ContractStatistics
    {
        /// <summary>Gets the contract type.</summary>
        Type ContractType { get; }
        
        /// <summary>Gets the number of implementations.</summary>
        int ImplementationCount { get; }
        
        /// <summary>Gets whether there's a primary implementation.</summary>
        bool HasPrimary { get; }
        
        /// <summary>Gets the lifetime distribution for this contract.</summary>
        IReadOnlyDictionary<ServiceLifetime, int> LifetimeDistribution { get; }
    }

    /// <summary>
    /// Represents a validation error.
    /// </summary>
    public class ValidationError
    {
        /// <summary>Gets or sets the error message.</summary>
        public string Message { get; init; } = string.Empty;
        
        /// <summary>Gets or sets the service type related to the error.</summary>
        public string? ServiceType { get; init; }
        
        /// <summary>Gets or sets additional error details.</summary>
        public string? Details { get; init; }
        
        /// <summary>Gets or sets the error severity level.</summary>
        public ValidationSeverity Severity { get; init; } = ValidationSeverity.Error;
    }

    /// <summary>
    /// Represents a validation warning.
    /// </summary>
    public class ValidationWarning
    {
        /// <summary>Gets or sets the warning message.</summary>
        public string Message { get; init; } = string.Empty;
        
        /// <summary>Gets or sets the service type related to the warning.</summary>
        public string? ServiceType { get; init; }
        
        /// <summary>Gets or sets suggestions to resolve the warning.</summary>
        public string? Suggestion { get; init; }
    }

    /// <summary>
    /// Validation severity levels.
    /// </summary>
    public enum ValidationSeverity
    {
        /// <summary>Informational message.</summary>
        Information,
        
        /// <summary>Warning that should be addressed.</summary>
        Warning,
        
        /// <summary>Error that must be fixed.</summary>
        Error,
        
        /// <summary>Critical error that prevents container from functioning.</summary>
        Critical
    }

    /// <summary>
    /// Service lifetime enumeration.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>A new instance is created every time it's requested.</summary>
        Transient = 0,
        
        /// <summary>A single instance per scope (e.g., per HTTP request).</summary>
        Scoped = 1,
        
        /// <summary>A single instance for the entire application lifetime.</summary>
        Singleton = 2
    }
}
