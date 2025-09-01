namespace Zentient.Abstractions.DependencyInjection.Definitions
{
    /// <summary>
    /// Defines a service to be registered in the DI container.
    /// </summary>
    public interface IServiceDefinition : ITypeDefinition { }

    /// <summary>
    /// Defines a logical module that groups related services.
    /// </summary>
    public interface IModuleDefinition : ITypeDefinition { }

    /// <summary>
    /// Defines an adapter for a third-party or external system.
    /// </summary>
    public interface IAdapterDefinition : ITypeDefinition { }
}

namespace Zentient.Abstractions.Clients.Definitions
{
    /// <summary>
    /// Defines an external client used for communication (e.g., HTTP, gRPC).
    /// </summary>
    public interface IClientDefinition : ITypeDefinition { }
}
namespace Zentient.DependencyInjection.Registrations
{
    /// <summary>Service lifetime enumeration for dependency injection.</summary>
    public enum ServiceLifetime
    {
        /// <summary>A new instance is created every time it's requested.</summary>
        Transient = 0,
        
        /// <summary>A single instance per scope (e.g., per HTTP request).</summary>
        Scoped = 1,
        
        /// <summary>A single instance for the entire application lifetime.</summary>
        Singleton = 2
    }

    /// <summary>
    /// Base class for all service registration attributes providing common functionality.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public abstract class ServiceAttributeBase : Attribute
    {
        /// <summary>
        /// Gets or sets the priority for this service registration.
        /// Higher values indicate higher priority.
        /// </summary>
        public int Priority { get; set; } = 0;

        /// <summary>Gets or sets additional metadata key-value pairs.</summary>
        public string[]? Metadata { get; set; }

        /// <summary>Gets or sets tags for semantic filtering and grouping.</summary>
        public string[]? Tags { get; set; }

        /// <summary>Gets or sets the category name for grouping services.</summary>
        public string? Category { get; set; }

        /// <summary>Gets or sets a description of the service.</summary>
        public string? Description { get; set; }

        /// <summary>Gets the metadata as a dictionary for easier access.</summary>
        public IReadOnlyDictionary<string, string> MetadataDictionary =>
            Metadata?.Where(m => m.Contains('=')).ToDictionary(
                m => m.Split('=')[0].Trim(),
                m => m.Split('=', 2)[1].Trim()) ?? 
            new Dictionary<string, string>();

        /// <summary>Validates the attribute configuration.</summary>
        /// <param name="targetType">The type this attribute is applied to.</param>
        public virtual void Validate(Type targetType)
        {
            // Base validation - can be overridden by derived classes
        }
    }

    /// <summary>
    /// Marks a class for automatic service registration with comprehensive configuration options.
    /// </summary>
    /// <example>
    /// <code>
    /// [Service(ServiceLifetime.Scoped, Priority = 10)]
    /// [ProvidesContract(typeof(IEmailService))]
    /// [ServiceCondition("Email:Enabled", "true")]
    /// public class EmailService : IEmailService { }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceAttribute : ServiceAttributeBase
    {
        /// <summary>Initializes a new instance of the ServiceAttribute class.</summary>
        /// <param name="lifetime">The service lifetime.</param>
        public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            Lifetime = lifetime;
        }

        /// <summary>Gets the service lifetime.</summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// Gets or sets whether this service should replace existing registrations.
        /// </summary>
        public bool Replace { get; set; } = false;

        /// <summary>
        /// Gets or sets the implementation factory type (must implement IServiceFactory&lt;T&gt;).
        /// </summary>
        public Type? FactoryType { get; set; }

        /// <summary>
        /// Gets or sets whether to register as self (the concrete type) in addition to contracts.
        /// </summary>
        public bool RegisterSelf { get; set; } = true;

        /// <inheritdoc />
        public override void Validate(Type targetType)
        {
            if (!targetType.IsClass || targetType.IsAbstract)
            {
                throw new InvalidOperationException(
                    $"ServiceAttribute can only be applied to concrete classes. Type: {targetType.FullName}");
            }

            if (FactoryType != null)
            {
                var factoryInterface = typeof(IServiceFactory<>).MakeGenericType(targetType);
                if (!factoryInterface.IsAssignableFrom(FactoryType))
                {
                    throw new InvalidOperationException(
                        $"FactoryType {FactoryType.FullName} must implement {factoryInterface.FullName}");
                }
            }

            base.Validate(targetType);
        }
    }

    /// <summary>
    /// Specifies that a service implementation provides a specific contract.
    /// Can be applied multiple times for multiple contracts.
    /// </summary>
    /// <example>
    /// <code>
    /// [ProvidesContract(typeof(IEmailService), Primary = true)]
    /// [ProvidesContract(typeof(INotificationService))]
    /// public class EmailService : IEmailService, INotificationService { }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ProvidesContractAttribute : ServiceAttributeBase
    {
        /// <summary>Initializes a new instance of the ProvidesContractAttribute class.</summary>
        /// <param name="contractType">The contract type this service provides.</param>
        public ProvidesContractAttribute(Type contractType)
        {
            ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        }

        /// <summary>Gets the contract type this service provides.</summary>
        public Type ContractType { get; }

        /// <summary>
        /// Gets or sets whether this is the primary implementation for the contract.
        /// Primary implementations are preferred during resolution.
        /// </summary>
        public bool Primary { get; set; } = false;

        /// <summary>Gets or sets the service key for keyed registrations.</summary>
        public string? ServiceKey { get; set; }

        /// <inheritdoc />
        public override void Validate(Type targetType)
        {
            if (!ContractType.IsInterface && !ContractType.IsAbstract)
            {
                throw new ArgumentException(
                    $"Contract type must be an interface or abstract class. Type: {ContractType.FullName}",
                    nameof(ContractType));
            }

            if (!ContractType.IsAssignableFrom(targetType))
            {
                throw new InvalidOperationException(
                    $"Type {targetType.FullName} does not implement contract {ContractType.FullName}");
            }

            base.Validate(targetType);
        }
    }

    /// <summary>
    /// Specifies conditional registration based on configuration values or environment conditions.
    /// </summary>
    /// <example>
    /// <code>
    /// [ServiceCondition("Features:EmailEnabled", "true")]
    /// [ServiceCondition("Environment", "Production", "Staging")]
    /// public class ProductionEmailService : IEmailService { }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ServiceConditionAttribute : Attribute
    {
        /// <summary>Initializes a new instance for configuration-based conditions.</summary>
        /// <param name="configurationKey">The configuration key to check.</param>
        /// <param name="expectedValues">The expected values (OR condition).</param>
        public ServiceConditionAttribute(string configurationKey, params string[] expectedValues)
        {
            ConfigurationKey = configurationKey
              ?? throw new ArgumentNullException(nameof(configurationKey));
            ExpectedValues = expectedValues
              ?? throw new ArgumentNullException(nameof(expectedValues));
            
            if (expectedValues.Length == 0)
            {
                throw new ArgumentException("At least one expected value must be provided.", nameof(expectedValues));
            }
        }

        /// <summary>Initializes a new instance for environment-based conditions.</summary>
        /// <param name="environmentNames">The environment names to match.</param>
        public ServiceConditionAttribute(params string[] environmentNames)
        {
            EnvironmentNames = environmentNames
              ?? throw new ArgumentNullException(nameof(environmentNames));
            
            if (environmentNames.Length == 0)
            {
                throw new ArgumentException("At least one environment name must be provided.", nameof(environmentNames));
            }
        }

        /// <summary>Gets the configuration key to evaluate.</summary>
        public string? ConfigurationKey { get; }

        /// <summary>Gets the expected configuration values.</summary>
        public string[]? ExpectedValues { get; }

        /// <summary>Gets the environment names to match.</summary>
        public string[]? EnvironmentNames { get; }

        /// <summary>Gets or sets whether the condition should be negated.</summary>
        public bool Negate { get; set; } = false;
    }

    /// <summary>
    /// Marks a service as a decorator for another service.
    /// Decorators wrap existing service implementations.
    /// </summary>
    /// <example>
    /// <code>
    /// [ServiceDecorator(typeof(IEmailService), Order = 1)]
    /// public class LoggingEmailDecorator : IEmailService
    /// {
    ///     public LoggingEmailDecorator(IEmailService inner) { ... }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceDecoratorAttribute : ServiceAttributeBase
    {
        /// <summary>Initializes a new instance of the ServiceDecoratorAttribute class.</summary>
        /// <param name="serviceType">The service type being decorated.</param>
        public ServiceDecoratorAttribute(Type serviceType)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        }

        /// <summary>Gets the service type being decorated.</summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Gets or sets the decoration order. Lower values are applied first (closer to original).
        /// </summary>
        public int Order { get; set; } = 0;

        /// <inheritdoc />
        public override void Validate(Type targetType)
        {
            if (!ServiceType.IsAssignableFrom(targetType))
            {
                throw new InvalidOperationException(
                    $"Decorator {targetType.FullName} must implement the service type {ServiceType.FullName}");
            }

            // Check if the decorator has a constructor that accepts the service type
            var constructors = targetType.GetConstructors();
            var hasValidConstructor = constructors.Any(c => 
                c.GetParameters().Any(p => ServiceType.IsAssignableFrom(p.ParameterType)));

            if (!hasValidConstructor)
            {
                throw new InvalidOperationException(
                    $"Decorator {targetType.FullName} must have a constructor that accepts {ServiceType.FullName}");
            }

            base.Validate(targetType);
        }
    }

    /// <summary>Marks a service as a factory for creating other services.</summary>
    /// <example>
    /// <code>
    /// [ServiceFactory(typeof(IEmailService))]
    /// public class EmailServiceFactory : IServiceFactory&lt;IEmailService&gt;
    /// {
    ///     public IEmailService Create(IServiceProvider provider) { ... }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ServiceFactoryAttribute : ServiceAttributeBase
    {
        /// <summary>Initializes a new instance of the ServiceFactoryAttribute class.</summary>
        /// <param name="serviceType">The type of service this factory creates.</param>
        public ServiceFactoryAttribute(Type serviceType)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        }

        /// <summary>Gets the type of service this factory creates.</summary>
        public Type ServiceType { get; }

        /// <inheritdoc />
        public override void Validate(Type targetType)
        {
            var factoryInterface = typeof(IServiceFactory<>).MakeGenericType(ServiceType);
            if (!factoryInterface.IsAssignableFrom(targetType))
            {
                throw new InvalidOperationException(
                    $"Factory {targetType.FullName} must implement {factoryInterface.FullName}");
            }

            base.Validate(targetType);
        }
    }

    /// <summary>Interface for service factories.</summary>
    /// <typeparam name="T">The type of service to create.</typeparam>
    public interface IServiceFactory<out T>
    {
        /// <summary>Creates a service instance.</summary>
        /// <param name="serviceProvider">The service provider for dependency resolution.</param>
        /// <returns>The created service instance.</returns>
        T Create(IServiceProvider serviceProvider);
    }

    /// <summary>
    /// Excludes a service from automatic registration scanning.
    /// Useful for abstract base classes or services that should be manually registered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class ExcludeFromRegistrationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ExcludeFromRegistrationAttribute class.
        /// </summary>
        /// <param name="reason">Optional reason for exclusion (for documentation).</param>
        public ExcludeFromRegistrationAttribute(string? reason = null)
        {
            Reason = reason;
        }

        /// <summary>Gets the reason for exclusion.</summary>
        public string? Reason { get; }
    }
}
