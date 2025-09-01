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
    /// - Options pattern integration
    /// - Validation and diagnostics
    /// </remarks>
    public interface IContainerBuilder
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
        IContainerBuilder Register<TImplementation>() 
            where TImplementation : class;

        /// <summary>
        /// Registers a service implementation by discovering its metadata via attributes.
        /// </summary>
        /// <param name="implementationType">The service implementation type.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IContainerBuilder Register(Type implementationType);

        /// <summary>
        /// Registers a service using a fluent definition builder.
        /// This provides maximum control over service registration.
        /// </summary>
        /// <typeparam name="TDefinition">The service definition type.</typeparam>
        /// <param name="configure">Delegate to configure the registration.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// builder.Register&lt;EmailServiceDefinition&gt;(reg => reg
        ///     .As&lt;IEmailService&gt;()
        ///     .WithLifetime(ServiceLifetime.Scoped)
        ///     .WithMetadata("Provider", "SendGrid"));
        /// </code>
        /// </example>
        IContainerBuilder Register<TDefinition>(
            Action<IServiceRegistrationBuilder<TDefinition>> configure)
            where TDefinition : IServiceDefinition, new();

        /// <summary>
        /// Registers a service using an existing definition instance with configuration.
        /// </summary>
        /// <typeparam name="TDefinition">The service definition type.</typeparam>
        /// <param name="definition">The service definition instance.</param>
        /// <param name="configure">Delegate to configure the registration.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IContainerBuilder Register<TDefinition>(
            TDefinition definition,
            Action<IServiceRegistrationBuilder<TDefinition>> configure)
            where TDefinition : IServiceDefinition;

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
        IContainerBuilder RegisterInstance<TService>(TService instance)
            where TService : class;

        /// <summary>
        /// Registers a singleton instance with metadata.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="instance">The service instance.</param>
        /// <param name="configure">Delegate to configure additional metadata.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IContainerBuilder RegisterInstance<TService>(
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
        ///     resolver => new EmailService(resolver.Resolve&lt;IConfiguration&gt;()),
        ///     ServiceLifetime.Scoped);
        /// </code>
        /// </example>
        IContainerBuilder RegisterFactory<TService>(
            Func<IServiceResolver, TService> factory,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class;

        /// <summary>
        /// Registers an asynchronous service factory.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">Async factory delegate to create the service.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IContainerBuilder RegisterAsyncFactory<TService>(
            Func<IServiceResolver, Task<TService>> factory,
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
        IContainerBuilder RegisterFromAssemblies(params Assembly[] assemblies);

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
        IContainerBuilder RegisterFromAssemblies(
            Action<IAssemblyScanningBuilder> configure);

        /// <summary>
        /// Registers services from the current assembly.
        /// </summary>
        /// <returns>The builder instance for fluent chaining.</returns>
        IContainerBuilder RegisterFromCurrentAssembly();

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
        IContainerBuilder RegisterIf(
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
        IContainerBuilder RegisterForEnvironments(
            string[] environmentNames,
            Action<IConditionalRegistrationBuilder> configure);

        // ========================================================================================
        // OPTIONS PATTERN INTEGRATION
        // ========================================================================================

        /// <summary>
        /// Registers an options object as a singleton service.
        /// </summary>
        /// <typeparam name="TDefinition">The options definition type.</typeparam>
        /// <typeparam name="TOptions">The concrete options type.</typeparam>
        /// <param name="options">The options instance to register.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IContainerBuilder AddOptions<TDefinition, TOptions>(TOptions options)
            where TDefinition : IOptionsDefinition
            where TOptions : class;

        /// <summary>
        /// Registers an options object using a factory delegate.
        /// </summary>
        /// <typeparam name="TDefinition">The options definition type.</typeparam>
        /// <typeparam name="TOptions">The concrete options type.</typeparam>
        /// <param name="factory">Factory to create the options value.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IContainerBuilder AddOptions<TDefinition, TOptions>(
            Func<IServiceResolver, TOptions> factory)
            where TDefinition : IOptionsDefinition
            where TOptions : class;

        // ========================================================================================
        // REGISTRY AND MODULE MANAGEMENT
        // ========================================================================================

        /// <summary>
        /// Adds a pre-built service registry to the container.
        /// </summary>
        /// <param name="registry">The service registry to add.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IContainerBuilder AddRegistry(IServiceRegistry registry);

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
        IContainerBuilder AddModule<TModule>()
            where TModule : IServiceModule, new();

        /// <summary>
        /// Registers a service module instance.
        /// </summary>
        /// <param name="module">The module instance to register.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        IContainerBuilder AddModule(IServiceModule module);

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
        /// if (!results.IsSuccess)
        /// {
        ///     foreach (var error in results.Errors)
        ///         Console.WriteLine($"Validation Error: {error.Message}");
        /// }
        /// </code>
        /// </example>
        Task<IResult<IContainerValidationReport>> ValidateAsync(
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
        /// Builds and returns the finalized service resolver.
        /// </summary>
        /// <returns>The configured service resolver.</returns>
        /// <exception cref="InvalidOperationException">Thrown when container configuration is invalid.</exception>
        IServiceResolver Build();

        /// <summary>
        /// Builds the container asynchronously with validation.
        /// </summary>
        /// <param name="validateFirst">Whether to validate before building.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task containing the configured service resolver.</returns>
        Task<IServiceResolver> BuildAsync(
            bool validateFirst = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a child builder that inherits from this builder's configuration.
        /// </summary>
        /// <returns>A new child container builder.</returns>
        IContainerBuilder CreateChild();

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
        IReadOnlyCollection<IServiceDescriptor> RegisteredServices { get; }
    }

    // ========================================================================================
    // SUPPORTING INTERFACES
    // ========================================================================================

    /// <summary>
    /// Builder for configuring service registrations with fluent syntax.
    /// </summary>
    /// <typeparam name="TDefinition">The service definition type.</typeparam>
    public interface IServiceRegistrationBuilder<TDefinition>
        where TDefinition : IServiceDefinition
    {
        /// <summary>Specifies the contract type for this service.</summary>
        IServiceRegistrationBuilder<TDefinition> As<TContract>();
        
        /// <summary>Specifies the implementation type for this service.</summary>
        IServiceRegistrationBuilder<TDefinition> ImplementedBy<TImplementation>()
            where TImplementation : class;
        
        /// <summary>Sets the service lifetime.</summary>
        IServiceRegistrationBuilder<TDefinition> WithLifetime(ServiceLifetime lifetime);
        
        /// <summary>Adds metadata to the service registration.</summary>
        IServiceRegistrationBuilder<TDefinition> WithMetadata(string key, object value);
        
        /// <summary>Adds tags to the service registration.</summary>
        IServiceRegistrationBuilder<TDefinition> WithTags(params string[] tags);
        
        /// <summary>Sets the service as primary for its contract.</summary>
        IServiceRegistrationBuilder<TDefinition> AsPrimary();
        
        /// <summary>Sets a service key for keyed registration.</summary>
        IServiceRegistrationBuilder<TDefinition> WithKey(string key);
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
        IConditionalRegistrationBuilder Register<TDefinition>(
            Action<IServiceRegistrationBuilder<TDefinition>> configure)
            where TDefinition : IServiceDefinition, new();
    }

    /// <summary>
    /// Represents a service module containing related service registrations.
    /// </summary>
    public interface IServiceModule
    {
        /// <summary>
        /// Configures services in this module.
        /// </summary>
        /// <param name="builder">The container builder to configure.</param>
        void ConfigureServices(IContainerBuilder builder);
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
    }

    /// <summary>
    /// Represents a validation error.
    /// </summary>
    public class ValidationError
    {
        public string Message { get; init; } = string.Empty;
        public string? ServiceType { get; init; }
        public string? Details { get; init; }
    }

    /// <summary>
    /// Represents a validation warning.
    /// </summary>
    public class ValidationWarning
    {
        public string Message { get; init; } = string.Empty;
        public string? ServiceType { get; init; }
        public string? Suggestion { get; init; }
    }
}
