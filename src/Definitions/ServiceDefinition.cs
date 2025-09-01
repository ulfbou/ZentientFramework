using System;

namespace Zentient.Abstractions.DependencyInjection.Registration
{
    /// <summary>Specifies the contract that a service implementation provides.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ProvidesContractAttribute : Attribute
    {
        public ProvidesContractAttribute(Type contractType)
        {
            ContractType = contractType;
        }

        public Type ContractType { get; }
    }
}
using System;

namespace Zentient.Abstractions.DependencyInjection.Registration
{
    /// <summary>
    /// Marks a class as a service to be automatically registered in the dependency injection
    /// container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceRegistrationAttribute : Attribute
    {
        public ServiceRegistrationAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        public ServiceLifetime Lifetime { get; }
    }
}
using System;
using System.Threading.Tasks;
using Zentient.Abstractions.DependencyInjection.Definitions;
using Zentient.Abstractions.DependencyInjection.Resolution;

namespace Zentient.Abstractions.DependencyInjection.Registration
{
    /// <summary>
    /// Describes a registered service in the dependency injection container.
    /// </summary>
    public interface IServiceDescriptor
    {
        /// <summary>Gets the type-safe definition for the service.</summary>
        IServiceDefinition Definition { get; }

        /// <summary>Gets the CLR type of the implementation being registered.</summary>
        Type ImplementationType { get; }

        /// <summary>
        /// Gets the asynchronous factory delegate used to create the service instance.
        /// </summary>
        Func<IServiceResolver, Task<object>> Factory { get; }

        /// <summary>Gets the lifetime of the registered service.</summary>
        ServiceLifetime Lifetime { get; }
    }
}
namespace Zentient.Abstractions.DependencyInjection
{
    /// <summary>Defines the possible lifetimes for a registered service.</summary>
    public enum ServiceLifetime
    {
        Transient,
        Scoped,
        Singleton
    }
}
using System;
using System.Threading.Tasks;
using Zentient.Abstractions.DependencyInjection.Definitions;
using Zentient.Abstractions.DependencyInjection.Registration;

namespace Zentient.Abstractions.DependencyInjection
{
    /// <summary>
    /// Fluent builder for registering a service and its full configuration.
    /// </summary>
    /// <typeparam name="TDefinition">The associated service definition type.</typeparam>
    public interface IServiceRegistrationBuilder<out TDefinition>
        where TDefinition : IServiceDefinition
    {
        /// <summary>Gets the definition being configured.</summary>
        TDefinition Definition { get; }

        /// <summary>Specifies the service implementation type.</summary>
        IServiceRegistrationBuilder<TDefinition> WithImplementation<TImplementation>()
            where TImplementation : class;

        /// <summary>Specifies the service implementation type.</summary>
        IServiceRegistrationBuilder<TDefinition> WithImplementation(Type implementationType);

        /// <summary>Specifies an async factory function to create the service instance.</summary>
        IServiceRegistrationBuilder<TDefinition> WithFactory(Func<IServiceResolver, Task<object>> factory);

        /// <summary>Specifies the lifetime of the service in the container.</summary>
        IServiceRegistrationBuilder<TDefinition> WithLifetime(ServiceLifetime lifetime);
    }
}
using System;
using Zentient.Abstractions.DependencyInjection.Definitions;

namespace Zentient.Abstractions.DependencyInjection
{
    /// <summary>Container-agnostic builder for orchestrating service registrations.</summary>
    public interface IContainerBuilder
    {
        /// <summary>
        /// Registers a service implementation by discovering its metadata via attributes.
        /// </summary>
        /// <typeparam name="TImplementation">The service implementation type.</typeparam>
        IContainerBuilder Register<TImplementation>() where TImplementation : class;

        /// <summary>
        /// Registers a service and its configuration using a definition.
        /// This is used for manual overrides or third-party registrations.
        /// </summary>
        /// <typeparam name="TDefinition">The definition type.</typeparam>
        /// <param name="definition">The definition instance.</param>
        /// <param name="configure">Delegate to configure the registration.</param>
        void Register<TDefinition>(
            TDefinition definition,
            Action<IServiceRegistrationBuilder<TDefinition>> configure)
            where TDefinition : IServiceDefinition;
    }
}
namespace Zentient.Abstractions.DependencyInjection.Container
{
    public interface IContainerBuilder
    {
        /// <summary>
        /// Registers a service implementation by discovering its metadata via attributes.
        /// </summary>
        /// <typeparam name="TImplementation">The service implementation type.</typeparam>
        IContainerBuilder Register<TImplementation>() where TImplementation : class;

        /// <summary>
        /// Registers a service and its configuration using a definition.
        /// This is used for manual overrides or third-party registrations.
        /// </summary>
        /// <typeparam name="TDefinition">The definition type.</typeparam>
        /// <param name="definition">The definition instance.</param>
        /// <param name="configure">Delegate to configure the registration.</param>
        void Register<TDefinition>(
            TDefinition definition,
            Action<IServiceRegistrationBuilder<TDefinition>> configure)
            where TDefinition : IServiceDefinition;
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zentient.Abstractions.Results;
using Zentient.Abstractions.DependencyInjection.Registration;
using Zentient.Abstractions.DependencyInjection.Scopes;
using Zentient.Abstractions.DependencyInjection.Predicates;

namespace Zentient.Abstractions.DependencyInjection
{
    /// <summary>
    /// Represents a DI-agnostic resolver for retrieving services from the container.
    /// It is decoupled from any specific underlying provider like IServiceProvider.
    /// </summary>
    public interface IServiceResolver
    {
        /// <summary>Gets the factory for creating new service scopes.</summary>
        IServiceScopeFactory ScopeFactory { get; }

        /// <summary>
        /// Resolves a single instance of the specified contract using an optional metadata-based
        /// predicate.
        /// </summary>
        Task<IResult<TContract>> ResolveSingle<TContract>(
            Func<IServiceDescriptor, bool>? predicate = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves a single instance of the specified contract using a structured metadata
        /// predicate.
        /// </summary>
        Task<IResult<TContract>> ResolveSingle<TContract>(
            IMetadataPredicate predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves all registered instances of the specified contract asynchronously,
        /// using an optional metadata-based predicate.
        /// </summary>
        IAsyncEnumerable<TContract> ResolveMany<TContract>(
            Func<IServiceDescriptor, bool>? predicate = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves all registered instances of the specified contract asynchronously,
        /// using a structured metadata predicate.
        /// </summary>
        IAsyncEnumerable<TContract> ResolveMany<TContract>(
            IMetadataPredicate predicate,
            CancellationToken cancellationToken = default);
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zentient.Abstractions.Results;
using Zentient.Abstractions.DependencyInjection.Registration;
using Zentient.Abstractions.DependencyInjection.Scopes;
using Zentient.Abstractions.DependencyInjection.Predicates;

namespace Zentient.Abstractions.DependencyInjection
{
    /// <summary>
    /// Represents a DI-agnostic resolver for retrieving services from the container.
    /// It is decoupled from any specific underlying provider like IServiceProvider.
    /// </summary>
    public interface IServiceResolver
    {
        /// <summary>Gets the factory for creating new service scopes.</summary>
        IServiceScopeFactory ScopeFactory { get; }

        /// <summary>
        /// Resolves a single instance of the specified contract using an optional metadata-based
        /// predicate.
        /// </summary>
        Task<IResult<TContract>> ResolveSingle<TContract>(
            Func<IServiceDescriptor, bool>? predicate = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves a single instance of the specified contract using a structured metadata
        /// predicate.
        /// </summary>
        Task<IResult<TContract>> ResolveSingle<TContract>(
            IMetadataPredicate predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves all registered instances of the specified contract asynchronously,
        /// using an optional metadata-based predicate.
        /// </summary>
        IAsyncEnumerable<TContract> ResolveMany<TContract>(
            Func<IServiceDescriptor, bool>? predicate = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves all registered instances of the specified contract asynchronously,
        /// using a structured metadata predicate.
        /// </summary>
        IAsyncEnumerable<TContract> ResolveMany<TContract>(
            IMetadataPredicate predicate,
            CancellationToken cancellationToken = default);
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zentient.Abstractions.Results;
using Zentient.Abstractions.DependencyInjection.Registration;
using Zentient.Abstractions.DependencyInjection.Scopes;
using Zentient.Abstractions.DependencyInjection.Predicates;

namespace Zentient.Abstractions.DependencyInjection
{
    /// <summary>
    /// Represents a DI-agnostic resolver for retrieving services from the container.
    /// It is decoupled from any specific underlying provider like IServiceProvider.
    /// </summary>
    public interface IServiceResolver
    {
        /// <summary>Gets the factory for creating new service scopes.</summary>
        IServiceScopeFactory ScopeFactory { get; }

        /// <summary>
        /// Resolves a single instance of the specified contract using an optional metadata-based
        /// predicate.
        /// </summary>
        Task<IResult<TContract>> ResolveSingle<TContract>(
            Func<IServiceDescriptor, bool>? predicate = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves a single instance of the specified contract using a structured metadata
        /// predicate.
        /// </summary>
        Task<IResult<TContract>> ResolveSingle<TContract>(
            IMetadataPredicate predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves all registered instances of the specified contract asynchronously,
        /// using an optional metadata-based predicate.
        /// </summary>
        IAsyncEnumerable<TContract> ResolveMany<TContract>(
            Func<IServiceDescriptor, bool>? predicate = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves all registered instances of the specified contract asynchronously,
        /// using a structured metadata predicate.
        /// </summary>
        IAsyncEnumerable<TContract> ResolveMany<TContract>(
            IMetadataPredicate predicate,
            CancellationToken cancellationToken = default);
    }
}


using System;
using System.Collections.Generic;
using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Common.Metadata;
using Zentient.Abstractions.DependencyInjection.Definitions;

namespace Zentient.DependencyInjection.Definitions
{
    /// <summary>
    /// An immutable implementation of the IServiceDefinition contract.
    /// This object is created by builders and contains all the metadata
    /// discovered for a service.
    /// </summary>
    public sealed class ServiceDefinition : IServiceDefinition
    {
        /// <summary>Gets the unique identifier of the service.</summary>
        /// <value>
        /// A non-null, non-empty string representing the unique identifier of the service.
        /// This is typically a GUID or a similar unique identifier.
        /// </value>
        public string Id { get; }

        /// <summary>Gets the name of the service.</summary>
        /// <value>
        /// A non-null, non-empty string representing the name of the service.
        /// This is used to identify the service in a human-readable format.
        /// </value>
        public string Name { get; }

        /// <summary>Gets the version of the service.</summary>
        /// <value>
        /// A non-null, non-empty string representing the version of the service.
        /// This is useful for managing compatibility and updates to the service.
        /// </value>
        public string Version { get; }

        /// <summary>Gets the description of the service.</summary>
        /// <value>
        /// A non-null, non-empty string providing a description of the service.
        /// This description can include details about the service's purpose, functionality, and usage.
        /// </value>
        public string Description { get; }

        /// <summary>Gets the category name of the service.</summary>
        /// <value>
        /// A non-null, non-empty string representing the category name of the service.
        /// This is used to classify the service into a specific category for better organization and discovery.
        /// </value>
        public string CategoryName { get; }

        /// <summary>Gets the relations associated with the service.</summary>
        /// <value>
        /// A read-only dictionary containing relations associated with the service.
        /// The keys are relation names, and the values are objects representing the related entities.
        /// This allows for flexible and extensible relationships between services and other entities.
        /// </value>
        public IReadOnlyDictionary<string, object?> Relations { get; }

        /// <summary>Gets the metadata associated with the service.</summary>
        /// <value>
        /// An instance of <see cref="IMetadata"/> representing the immutable metadata associated with the service.
        /// This metadata can include additional information such as tags, attributes, or other relevant data that describes the service.
        /// </value>
        public IMetadata Metadata { get; }

        /// <summary>Initializes a new instance of the <see cref="ServiceDefinition"/> class.</summary>
        /// <param name="id">The unique identifier of the service.</param>
        /// <param name="name">The name of the service.</param>
        /// <param name="version">The version of the service.</param>
        /// <param name="description">A description of the service.</param>
        /// <param name="categoryName">The category to which the service belongs.</param>
        /// <param name="relations">A read-only dictionary containing relations associated with the service.</param>
        /// <param name="metadata">Metadata associated with the service.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null or empty.</exception>
        public ServiceDefinition(
            string id,
            string name,
            string version,
            string description,
            string categoryName,
            IReadOnlyDictionary<string, object?> relations,
            IMetadata metadata)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            CategoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
            Relations = relations ?? throw new ArgumentNullException(nameof(relations));
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }
    }
}
using System;
using Zentient.Abstractions.Common.Builders;
using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.DependencyInjection.Definitions;

namespace Zentient.Abstractions.DependencyInjection.Builders
{
    /// <summary>
    /// The base interface for building IServiceDefinition instances.
    /// </summary>
    public interface IServiceDefinitionBuilder : ITypeDefinitionBuilder<IServiceDefinition> { }

    /// <summary>
    /// Fluent builder for registering a service and its full configuration.
    /// </summary>
    /// <typeparam name="TDefinition">The associated service definition type.</typeparam>
    public interface IServiceRegistrationBuilder<out TDefinition>
        where TDefinition : IServiceDefinition
    {
        /// <summary>Gets the definition being configured.</summary>
        /// <value>
        /// The <see cref="IServiceDefinition"/> instance that is being built.
        /// This contains all the metadata and configuration for the service.
        /// </value>
        /// <remarks>
        /// This property provides access to the definition being constructed,
        /// allowing further configuration or inspection of the service's properties.
        /// </remarks>
        TDefinition Definition { get; }

        /// <summary>Specifies the service implementation type.</summary>
        /// <typeparam name="TImplementation">The type of the service implementation.</typeparam>
        /// <returns>
        /// An instance of <see cref="IServiceRegistrationBuilder{TDefinition}"/> for further configuration.
        /// </returns>
        /// <remarks>
        /// This method allows you to specify the concrete implementation type that will be registered
        /// for the service defined by <typeparamref name="TDefinition"/>.
        /// </remarks>
        IServiceRegistrationBuilder<TDefinition> WithImplementation<TImplementation>() where TImplementation : class;

        /// <summary>Specifies the service implementation type.</summary>
        /// <param name="implementationType">The type of the service implementation.</param>
        /// <returns>
        /// An instance of <see cref="IServiceRegistrationBuilder{TDefinition}"/> for further configuration.
        /// </returns>
        /// <remarks>
        /// This method allows you to specify the concrete implementation type that will be registered
        /// for the service defined by <typeparamref name="TDefinition"/>.
        /// </remarks>
        IServiceRegistrationBuilder<TDefinition> WithImplementation(Type implementationType);

        /// <summary>Specifies an asynchronous factory function to create the service instance.</summary>
        IServiceRegistrationBuilder<TDefinition> WithFactory(Func<IServiceResolver, object> factory);

        /// <summary>Specifies the lifetime of the service in the container.</summary>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <returns>
        /// An instance of <see cref="IServiceRegistrationBuilder{TDefinition}"/> for further configuration.
        /// </returns>
        /// <remarks>
        /// This method allows you to specify how long the service should be kept alive in the container,
        /// such as Singleton, Scoped, or Transient.
        /// </remarks>
        IServiceRegistrationBuilder<TDefinition> WithLifetime(ServiceLifetime lifetime);
    }
}
using Zentient.Abstractions.DependencyInjection.Definitions;

namespace Zentient.Abstractions.Common
{
    /// <summary>Represents an entity or definition that has a category.</summary>
    public interface IHasCategory
    {
        /// <summary>Gets the category name of the entity or definition.</summary>
        /// <value>A non-null, non-empty string representing the category name.</value>
        string CategoryName { get; }

        /// <summary>
        /// Represents the definition of a service, including its identity, version, description, category, relations, and metadata.
        /// </summary>
        /// <param name="id">The unique identifier of the service.</param>
        /// <param name="name">The name of the service.</param>
        /// <param name="version">The version of the service.</param>
        /// <param name="description">A description of the service.</param>
        /// <param name="categoryName">The category to which the service belongs.</param>
        /// <param name="relations">A read-only dictionary containing relations associated with the service.</param>
        /// <param name="metadata">Metadata associated with the service.</param>
    }
}
namespace Zentient.DependencyInjection.Registration
{
    /// <summary>
    /// An immutable implementation of the IServiceDescriptor contract.
    /// It links a discovered service definition to its runtime configuration.
    /// </summary>
    public sealed class ServiceDescriptor : IServiceDescriptor
    {
        /// <summary>
        /// Gets the service definition associated with this descriptor.
        /// </summary>
        /// <value>
        /// The <see cref="IServiceDefinition"/> instance representing the service definition.
        /// </value>
        public IServiceDefinition Definition { get; }

        /// <summary>Gets the type of the service implementation.</summary>
        /// <value>
        /// The type of the service implementation, which must implement the service interface defined in <see cref="Definition"/>.
        /// </value>
        public Type ImplementationType { get; }

        /// <summary>Gets the factory function for creating service instances.</summary>
        /// <value>
        /// A function that takes an <see cref="IServiceResolver"/> and returns an instance of the service.
        /// This can be null if the service is instantiated directly using <see cref="ImplementationType"/>.
        /// </value>
        public Func<IServiceResolver, object>? Factory { get; }

        /// <summary>Gets the lifetime of the service.</summary>
        /// <value>
        /// The <see cref="ServiceLifetime"/> indicating how long the service should be kept alive.
        /// This can be <see cref="ServiceLifetime.Singleton"/>, <see cref="ServiceLifetime.Scoped"/>, or <see cref="ServiceLifetime.Transient"/>.
        /// </value>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDescriptor"/> class.
        /// </summary>
        /// <param name="definition">The service definition associated with this descriptor.</param>
        /// <param name="implementationType">The type of the service implementation.</param>
        /// <param name="factory">An optional factory function to create instances of the service.</param>
        /// <param name="lifetime">The lifetime of the service, indicating how long the service should be kept alive.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="definition"/> or <paramref name="implementationType"/> is null.</exception>
        /// <remarks>
        /// This class is used to register services in the dependency injection container,
        /// providing a way to specify how services are created and managed.
        /// </remarks>
        public ServiceDescriptor(
            IServiceDefinition definition,
            Type implementationType,
            Func<IServiceResolver, object>? factory,
            ServiceLifetime lifetime)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            Factory = factory;
            Lifetime = lifetime;
        }
    }
}
using System;
using System.Collections.Generic;
using Zentient.Abstractions.DependencyInjection.Registration;

namespace Zentient.DependencyInjection.Builders
{
    /// <summary>
    /// Contract for a builder that constructs one or more IServiceDescriptor instances
    /// from a given service implementation type.
    /// </summary>
    public interface IServiceDescriptorBuilder
    {
        /// <summary>
        /// Builds service descriptors by inspecting attributes on the provided type.
        /// </summary>
        /// <param name="implementationType">The type to inspect for registration attributes.</param>
        /// <returns>An enumerable of service descriptors.</returns>
        IEnumerable<IServiceDescriptor> BuildFromAttributes(Type implementationType);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zentient.Abstractions.Common.Metadata;
using Zentient.Abstractions.DependencyInjection.Definitions;
using Zentient.Abstractions.DependencyInjection.Registration;
using Zentient.DependencyInjection.Definitions;

namespace Zentient.DependencyInjection.Builders
{
    /// <summary>
    /// A builder that creates service descriptors by inspecting a service
    /// implementation class for Zentient's registration and metadata attributes.
    /// </summary>
    public sealed class AttributeServiceDescriptorBuilder : IServiceDescriptorBuilder
    {
        /// <summary>
        /// Builds service descriptors from the attributes of the specified implementation type.
        /// </summary>
        /// <param name="implementationType">The type to inspect for registration attributes.</param>
        /// <returns>
        /// An enumerable of <see cref="IServiceDescriptor"/> instances created from the attributes found on the type.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="implementationType"/> is null.</exception>
        /// <remarks>
        /// This method looks for the <see cref="ServiceRegistrationAttribute"/> to determine if the type should be registered as a service.
        /// It also inspects for <see cref="ProvidesContractAttribute"/> to determine the contracts under which the service should be registered.
        /// Additional metadata attributes like <see cref="DefinitionCategoryAttribute"/> and <see cref="DefinitionTagAttribute"/> are used to populate the service definition.
        /// </remarks>
        public IEnumerable<IServiceDescriptor> BuildFromAttributes(Type implementationType)
        {
            if (implementationType is null) throw new ArgumentNullException(nameof(implementationType));

            // Discover the primary registration attribute
            var registrationAttribute = implementationType
                .GetCustomAttribute<ServiceRegistrationAttribute>(inherit: false);

            if (registrationAttribute is null)
            {
                // A type without this attribute is not a service to be automatically registered.
                yield break;
            }

            // Discover contract(s) to register under
            var contractAttributes = implementationType.GetCustomAttributes<ProvidesContractAttribute>(inherit: false).ToList();
            if (!contractAttributes.Any())
            {
                // If no contract is specified, register under its own type.
                contractAttributes.Add(new ProvidesContractAttribute(implementationType));
            }

            // Read all other metadata attributes to populate the definition
            var categoryAttribute = implementationType.GetCustomAttribute<DefinitionCategoryAttribute>(inherit: false);
            var tagAttributes = implementationType.GetCustomAttributes<DefinitionTagAttribute>(inherit: false);

            var metadata = new Dictionary<string, object?>();
            if (tagAttributes.Any())
            {
                metadata["tags"] = tagAttributes.SelectMany(a => a.Tags).ToArray();
            }

            // Build an IServiceDefinition for each contract
            foreach (var contractAttribute in contractAttributes)
            {
                // Create the immutable definition using metadata from attributes
                var definition = new ServiceDefinition(
                    id: implementationType.Name, // A better ID generation strategy could be implemented here
                    name: implementationType.Name,
                    version: "1.0", // A version attribute could be added for this
                    description: string.Empty, // A description attribute could be added
                    categoryName: categoryAttribute?.CategoryName ?? "Default",
                    relations: new Dictionary<string, object?>
                    {
                        { "provides", contractAttribute.ContractType.FullName }
                    },
                    metadata: new Metadata(metadata)
                );

                // Yield the final service descriptor
                yield return new ServiceDescriptor(
                    definition: definition,
                    implementationType: implementationType,
                    factory: null, // This builder doesn't handle factories
                    lifetime: registrationAttribute.Lifetime
                );
            }
        }
    }
}
using System.Collections.Generic;
using Zentient.Abstractions.DependencyInjection.Registration;

namespace Zentient.DependencyInjection.Runtime
{
    /// <summary>
    /// A central, read-only registry for tracking all discovered
    /// service descriptors and their associated metadata.
    /// </summary>
    public interface IServiceDescriptorRegistry
    {
        /// <summary>
        /// Registers a service descriptor with the registry.
        /// </summary>
        /// <param name="descriptor">The service descriptor to register.</param>
        void Register(IServiceDescriptor descriptor);

        /// <summary>
        /// Gets a read-only collection of all registered service descriptors.
        /// </summary>
        IReadOnlyCollection<IServiceDescriptor> All { get; }
    }
}
using System;
using System.Collections.Generic;
using Zentient.Abstractions.DependencyInjection.Registration;
using Zentient.DependencyInjection.Runtime;

namespace Zentient.DependencyInjection.Runtime
{
    /// <summary>
    /// A concrete, thread-safe implementation of IServiceDescriptorRegistry.
    /// It provides a central store for service descriptors, designed for
    /// runtime introspection and resolution.
    /// </summary>
    public sealed class ServiceDescriptorRegistry : IServiceDescriptorRegistry
    {
        private readonly List<IServiceDescriptor> _descriptors = new();
        private readonly object _lock = new();

        /// <summary>
        /// Registers a service descriptor with the registry.
        /// </summary>
        /// <param name="descriptor">The service descriptor to register.</param>
        public void Register(IServiceDescriptor descriptor)
        {
            if (descriptor is null) throw new ArgumentNullException(nameof(descriptor));

            lock (_lock)
            {
                _descriptors.Add(descriptor);
            }
        }

        /// <summary>
        /// Gets a read-only collection of all registered service descriptors.
        /// </summary>
        public IReadOnlyCollection<IServiceDescriptor> Descriptors => _descriptors.AsReadOnly();
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zentient.Abstractions.Results;
using Zentient.Abstractions.DependencyInjection.Registration;
using Zentient.Abstractions.DependencyInjection.Scopes;
using Zentient.Abstractions.DependencyInjection.Predicates;

namespace Zentient.Abstractions.DependencyInjection
{
    /// <summary>
    /// Represents a DI-agnostic resolver for retrieving services from the container.
    /// It is decoupled from any specific underlying provider like IServiceProvider.
    /// </summary>
    public interface IServiceResolver
    {
        /// <summary>Gets the factory for creating new service scopes.</summary>
        IServiceScopeFactory ScopeFactory { get; }

        /// <summary>
        /// Resolves a single instance of the specified contract using an optional metadata-based
        /// predicate.
        /// </summary>
        Task<IResult<TContract>> ResolveSingle<TContract>(
            Func<IServiceDescriptor, bool>? predicate = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves a single instance of the specified contract using a structured metadata
        /// predicate.
        /// </summary>
        Task<IResult<TContract>> ResolveSingle<TContract>(
            IMetadataPredicate predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves all registered instances of the specified contract asynchronously,
        /// using an optional metadata-based predicate.
        /// </summary>
        IAsyncEnumerable<TContract> ResolveMany<TContract>(
            Func<IServiceDescriptor, bool>? predicate = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves all registered instances of the specified contract asynchronously,
        /// using a structured metadata predicate.
        /// </summary>
        IAsyncEnumerable<TContract> ResolveMany<TContract>(
            IMetadataPredicate predicate,
            CancellationToken cancellationToken = default);
    }
}
using System;
using System.Collections.Generic;
using Zentient.Abstractions.Common.Context;

namespace Zentient.Abstractions.DependencyInjection.Resolution
{
    /// <summary>
    /// Type-safe, container-agnostic service resolver interface.
    /// Resolution is kept synchronous as it is an inherently fast, in-memory operation.
    /// </summary>
    public interface IServiceResolver
    {
        /// <summary>
        /// Resolves a single service instance of the specified type.
        /// </summary>
        /// <typeparam name="TService">The service type to resolve.</typeparam>
        TService Resolve<TService>();

        /// <summary>
        /// Resolves a single service instance of the specified type.
        /// </summary>
        /// <param name="serviceType">The service type to resolve.</param>
        object Resolve(Type serviceType);

        /// <summary>
        /// Resolves a single service instance with an ambient context.
        /// </summary>
        /// <typeparam name="TService">The service type to resolve.</typeparam>
        /// <param name="context">The ambient context for resolution.</param>
        TService Resolve<TService>(IContext context);

        /// <summary>
        /// Resolves all service instances of the specified type.
        /// </summary>
        /// <typeparam name="TService">The service type to resolve.</typeparam>
        IEnumerable<TService> ResolveAll<TService>();
    }
}
namespace Zentient.DependencyInjection.Resolution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Zentient.Abstractions.DependencyInjection.Registration;
    using Zentient.Abstractions.DependencyInjection.Resolution;
    using Zentient.Abstractions.Results;
    using Zentient.DependencyInjection.Runtime;

    /// <summary>
    /// An Async-First, metadata-aware resolver that uses the service descriptor
    /// registry to perform predicate-based resolution.
    /// </summary>
    public sealed class MetadataAwareServiceResolver : IServiceResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceDescriptorRegistry _registry;

        /// <summary>Initializes a new instance of the <see cref="MetadataAwareServiceResolver"/> class.</summary>
        /// <param name="serviceProvider">The underlying service provider used for resolving concrete instances.</param>
        /// <param name="registry">The service descriptor registry containing all registered services.</param>
        /// <exception cref="ArgumentNullException">Thrown if either <paramref name="serviceProvider"/> or <paramref name="registry"/> is null.</exception>
        /// <remarks>
        /// This resolver is designed to work with a registry of service descriptors,
        /// allowing for flexible and dynamic resolution of services based on metadata.
        /// It supports both single and multiple resolutions, with optional predicates
        /// for filtering based on service metadata.
        /// </remarks>
        public MetadataAwareServiceResolver(IServiceProvider serviceProvider, IServiceDescriptorRegistry registry)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        /// <inheritdoc />
        public Task<IResult<TContract>> ResolveSingle<TContract>(
            Func<IServiceDescriptor, bool>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            // Apply the predicate (if any) and find the first matching descriptor
            var descriptors = _registry.All
                .Where(d => typeof(TContract).IsAssignableFrom(d.ImplementationType))
                .Where(predicate ?? (_ => true));

            var match = descriptors.FirstOrDefault();

            if (match is null)
            {
                return Task.FromResult(Result.Failure<TContract>("No service found matching the criteria."));
            }

            // Use the underlying service provider to resolve the concrete implementation
            var instance = _serviceProvider.GetService(match.ImplementationType);

            return instance is TContract typedInstance
                ? Task.FromResult(Result.Success(typedInstance))
                : Task.FromResult(Result.Failure<TContract>("Resolved instance could not be cast to the contract type."));
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TContract> ResolveMany<TContract>(
            Func<IServiceDescriptor, bool>? predicate = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Apply the predicate to find all matching descriptors
            var matches = _registry.All
                .Where(d => typeof(TContract).IsAssignableFrom(d.ImplementationType))
                .Where(predicate ?? (_ => true));

            foreach (var descriptor in matches)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Use the underlying provider to resolve the concrete implementation
                var instance = _serviceProvider.GetService(descriptor.ImplementationType);

                if (instance is TContract typedInstance)
                {
                    yield return typedInstance;
                }
            }
        }
    }
}
