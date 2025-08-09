# 📚 Zentient.Abstractions API Reference

## Overview

This document provides comprehensive API documentation for Zentient.Abstractions 3.0.1, covering all namespaces, interfaces, and implementation patterns.

## 📖 Table of Contents

1. [Core Abstractions](#core-abstractions)
2. [Dependency Injection](#dependency-injection)
3. [Results & Envelopes](#results--envelopes)
4. [Configuration Management](#configuration-management)
5. [Caching](#caching)
6. [Messaging & CQRS](#messaging--cqrs)
7. [Validation](#validation)
8. [Diagnostics & Health Checks](#diagnostics--health-checks)
9. [Observability](#observability)
10. [Policies & Resilience](#policies--resilience)

---

## Core Abstractions

### IIdentifiable

Primary identification contract for all framework components.

```csharp
namespace Zentient.Abstractions.Common
{
    /// <summary>
    /// Defines a contract for objects that can be uniquely identified.
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// Gets the unique identifier for this object.
        /// </summary>
        /// <remarks>
        /// The ID should be unique within the context of the object type
        /// and should remain constant throughout the object's lifetime.
        /// </remarks>
        string Id { get; }
    }
}
```

**Implementation Guidelines:**
- Use meaningful, consistent ID formats (e.g., "UserService.v2", "Order.12345")
- Ensure IDs are immutable once assigned
- Consider including version information for evolving components

**Example Implementation:**
```csharp
public class UserService : IUserService, IIdentifiable
{
    public string Id => "UserService.v2.1.0";
    
    // Service implementation...
}
```

### IHasName

Provides human-readable naming for components.

```csharp
namespace Zentient.Abstractions.Common
{
    /// <summary>
    /// Defines a contract for objects that have a human-readable name.
    /// </summary>
    public interface IHasName
    {
        /// <summary>
        /// Gets the human-readable name of this object.
        /// </summary>
        string Name { get; }
    }
}
```

### IHasDescription

Adds descriptive information to components.

```csharp
namespace Zentient.Abstractions.Common
{
    /// <summary>
    /// Defines a contract for objects that can provide a description.
    /// </summary>
    public interface IHasDescription
    {
        /// <summary>
        /// Gets a detailed description of this object's purpose or functionality.
        /// </summary>
        string Description { get; }
    }
}
```

### IHasMetadata

Provides extensible metadata capabilities.

```csharp
namespace Zentient.Abstractions.Common
{
    /// <summary>
    /// Defines a contract for objects that carry metadata.
    /// </summary>
    public interface IHasMetadata
    {
        /// <summary>
        /// Gets the metadata associated with this object.
        /// </summary>
        IMetadata Metadata { get; }
    }
}
```

**Metadata Example:**
```csharp
public class OrderService : IOrderService, IHasMetadata
{
    public IMetadata Metadata => new MetadataCollection
    {
        ["Version"] = "2.1.0",
        ["Owner"] = "E-commerce Team",
        ["SLA"] = "99.95%",
        ["MaxThroughput"] = "10000 req/min",
        ["Dependencies"] = new[] { "PaymentService", "InventoryService" }
    };
}
```

---

## Dependency Injection

### IServiceRegistry

Core service registration interface.

```csharp
namespace Zentient.Abstractions.DependencyInjection
{
    /// <summary>
    /// Defines a contract for registering services with a dependency injection container.
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>
        /// Registers a service with the specified lifetime.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>A registration builder for fluent configuration.</returns>
        IServiceRegistrationBuilder Register<TService, TImplementation>(ServiceLifetime lifetime)
            where TImplementation : class, TService;

        /// <summary>
        /// Registers a service instance.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="instance">The service instance.</param>
        /// <returns>A registration builder for fluent configuration.</returns>
        IServiceRegistrationBuilder RegisterInstance<TService>(TService instance)
            where TService : class;

        /// <summary>
        /// Registers a factory function for creating service instances.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">The factory function.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>A registration builder for fluent configuration.</returns>
        IServiceRegistrationBuilder RegisterFactory<TService>(
            Func<IServiceResolver, TService> factory,
            ServiceLifetime lifetime) where TService : class;
    }
}
```

### ServiceRegistrationAttribute

Declarative service registration.

```csharp
namespace Zentient.Abstractions.DependencyInjection.Registration
{
    /// <summary>
    /// Marks a class for automatic service registration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceRegistrationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance with the specified lifetime.
        /// </summary>
        /// <param name="lifetime">The service lifetime.</param>
        public ServiceRegistrationAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        /// <summary>
        /// Initializes a new instance with service and implementation types.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        public ServiceRegistrationAttribute(
            Type serviceType, 
            Type implementationType, 
            ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }

        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public ServiceLifetime Lifetime { get; }
        public string Name { get; set; }
        public bool Replace { get; set; }
    }
}
```

**Usage Examples:**
```csharp
// Simple registration
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    // Implementation...
}

// Multiple interface registration
[ServiceRegistration(typeof(IUserService), typeof(UserService), ServiceLifetime.Scoped)]
[ServiceRegistration(typeof(IUserManager), typeof(UserService), ServiceLifetime.Scoped)]
public class UserService : IUserService, IUserManager
{
    // Implementation...
}

// Named registration
[ServiceRegistration(ServiceLifetime.Singleton, Name = "PrimaryCache")]
public class RedisCacheService : ICacheService
{
    // Implementation...
}
```

---

## Results & Envelopes

### IResult<T>

Basic result pattern for operations.

```csharp
namespace Zentient.Abstractions.Results
{
    /// <summary>
    /// Represents the result of an operation with a value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    public interface IResult<out T>
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        bool IsFailure { get; }

        /// <summary>
        /// Gets the result value if the operation was successful.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Gets the error information if the operation failed.
        /// </summary>
        string Error { get; }

        /// <summary>
        /// Gets additional metadata about the result.
        /// </summary>
        IMetadata Metadata { get; }
    }
}
```

### IEnvelope<TCode, TError>

Advanced envelope pattern with typed codes and errors.

```csharp
namespace Zentient.Abstractions.Envelopes
{
    /// <summary>
    /// Represents an envelope that wraps operation results with codes and errors.
    /// </summary>
    /// <typeparam name="TCode">The type of success codes.</typeparam>
    /// <typeparam name="TError">The type of error information.</typeparam>
    public interface IEnvelope<out TCode, out TError>
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        bool IsFailure { get; }

        /// <summary>
        /// Gets the success code when the operation succeeded.
        /// </summary>
        TCode Code { get; }

        /// <summary>
        /// Gets the collection of errors when the operation failed.
        /// </summary>
        IReadOnlyCollection<TError> Errors { get; }

        /// <summary>
        /// Gets the timestamp when the envelope was created.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Gets additional metadata about the operation.
        /// </summary>
        IMetadata Metadata { get; }
    }

    /// <summary>
    /// Represents an envelope that wraps operation results with a value.
    /// </summary>
    /// <typeparam name="TCode">The type of success codes.</typeparam>
    /// <typeparam name="TError">The type of error information.</typeparam>
    /// <typeparam name="TValue">The type of the wrapped value.</typeparam>
    public interface IEnvelope<out TCode, out TError, out TValue> : IEnvelope<TCode, TError>
    {
        /// <summary>
        /// Gets the wrapped value when the operation was successful.
        /// </summary>
        TValue Value { get; }

        /// <summary>
        /// Gets a value indicating whether this envelope contains a value.
        /// </summary>
        bool HasValue { get; }
    }
}
```

**Usage Patterns:**
```csharp
// Success envelope
public async Task<IEnvelope<UserCode, UserError>> CreateUserAsync(CreateUserRequest request)
{
    var user = await CreateUser(request);
    return Envelope.Success(UserCode.UserCreated, user);
}

// Error envelope
public async Task<IEnvelope<UserCode, UserError>> GetUserAsync(string userId)
{
    var user = await _repository.GetByIdAsync(userId);
    if (user == null)
    {
        return Envelope.NotFound<UserCode, UserError>(
            UserError.UserNotFound(userId)
        );
    }
    
    return Envelope.Success(UserCode.UserFound, user);
}

// Validation errors
public async Task<IEnvelope<OrderCode, OrderError>> ProcessOrderAsync(ProcessOrderRequest request)
{
    var validationResult = await _validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Envelope.ValidationError<OrderCode, OrderError>(
            validationResult.Errors.Select(e => OrderError.ValidationFailed(e.Message))
        );
    }
    
    // Process order...
}
```

---

## Configuration Management

### IConfiguration

Core configuration interface.

```csharp
namespace Zentient.Abstractions.Configuration
{
    /// <summary>
    /// Represents a set of key/value application configuration properties.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets or sets a configuration value for the specified key.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        string this[string key] { get; set; }

        /// <summary>
        /// Gets the immediate descendant configuration sub-sections.
        /// </summary>
        /// <returns>The configuration sub-sections.</returns>
        IEnumerable<IConfigurationSection> GetChildren();

        /// <summary>
        /// Returns a configuration sub-section with the specified key.
        /// </summary>
        /// <param name="key">The key of the configuration section.</param>
        /// <returns>The configuration section.</returns>
        IConfigurationSection GetSection(string key);

        /// <summary>
        /// Gets a configuration value for the specified key.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        string GetValue(string key);

        /// <summary>
        /// Gets a configuration value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value to return if no value is found.</param>
        /// <returns>The converted configuration value.</returns>
        T GetValue<T>(string key, T defaultValue = default);
    }
}
```

### ITypedConfiguration<TOptionsDefinition, TValue>

Strongly-typed configuration interface.

```csharp
namespace Zentient.Abstractions.Configuration
{
    /// <summary>
    /// Represents a strongly-typed configuration with validation and change notification.
    /// </summary>
    /// <typeparam name="TOptionsDefinition">The options definition type.</typeparam>
    /// <typeparam name="TValue">The configuration value type.</typeparam>
    public interface ITypedConfiguration<out TOptionsDefinition, out TValue> : IIdentifiable
        where TOptionsDefinition : IOptionsDefinition
    {
        /// <summary>
        /// Gets the options definition that describes this configuration.
        /// </summary>
        TOptionsDefinition Definition { get; }

        /// <summary>
        /// Gets the current configuration value.
        /// </summary>
        TValue Value { get; }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets validation errors if the configuration is invalid.
        /// </summary>
        IReadOnlyCollection<string> ValidationErrors { get; }

        /// <summary>
        /// Gets a change token that can be used to listen for configuration changes.
        /// </summary>
        IChangeToken ChangeToken { get; }
    }
}
```

**Configuration Examples:**
```csharp
// Options definition
public class DatabaseOptionsDefinition : IOptionsDefinition
{
    public string Id => "Database.Options.v1";
    public string Name => "Database Configuration";
    public string Description => "Database connection and performance settings";
    
    public IValidationRule[] ValidationRules => new[]
    {
        new RequiredValidationRule(nameof(ConnectionString)),
        new RangeValidationRule(nameof(CommandTimeout), 1, 300),
        new RangeValidationRule(nameof(MaxRetries), 0, 10)
    };
}

// Configuration model
public class DatabaseOptions
{
    public string ConnectionString { get; set; }
    public int CommandTimeout { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
    public bool EnableLogging { get; set; } = true;
}

// Usage in service
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserRepository : IUserRepository
{
    private readonly ITypedConfiguration<DatabaseOptionsDefinition, DatabaseOptions> _config;

    public UserRepository(ITypedConfiguration<DatabaseOptionsDefinition, DatabaseOptions> config)
    {
        _config = config;
    }

    public async Task<User> GetByIdAsync(string id)
    {
        using var connection = new SqlConnection(_config.Value.ConnectionString);
        using var command = new SqlCommand("SELECT * FROM Users WHERE Id = @Id", connection);
        
        command.CommandTimeout = _config.Value.CommandTimeout;
        // Implementation...
    }
}
```

---

## Caching

### ICache<TValue>

Generic caching interface.

```csharp
namespace Zentient.Abstractions.Caching
{
    /// <summary>
    /// Defines a contract for caching operations.
    /// </summary>
    /// <typeparam name="TValue">The type of values stored in the cache.</typeparam>
    public interface ICache<TValue>
    {
        /// <summary>
        /// Gets a value from the cache.
        /// </summary>
        /// <typeparam name="TKey">The type of the cache key.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The cached value if found, otherwise null.</returns>
        Task<ICacheItem<TValue>> GetAsync<TKey>(TKey key, CancellationToken cancellationToken = default)
            where TKey : ICacheKey<TValue>;

        /// <summary>
        /// Sets a value in the cache.
        /// </summary>
        /// <typeparam name="TKey">The type of the cache key.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="policy">The cache policy to apply.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetAsync<TKey>(
            TKey key, 
            TValue value, 
            CachePolicy policy = null,
            CancellationToken cancellationToken = default)
            where TKey : ICacheKey<TValue>;

        /// <summary>
        /// Removes a value from the cache.
        /// </summary>
        /// <typeparam name="TKey">The type of the cache key.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveAsync<TKey>(TKey key, CancellationToken cancellationToken = default)
            where TKey : ICacheKey<TValue>;

        /// <summary>
        /// Refreshes a value in the cache.
        /// </summary>
        /// <typeparam name="TKey">The type of the cache key.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RefreshAsync<TKey>(TKey key, CancellationToken cancellationToken = default)
            where TKey : ICacheKey<TValue>;
    }
}
```

### ICacheKey<TValue>

Cache key definition interface.

```csharp
namespace Zentient.Abstractions.Caching
{
    /// <summary>
    /// Defines a contract for cache keys.
    /// </summary>
    /// <typeparam name="TValue">The type of value this key can retrieve.</typeparam>
    public interface ICacheKey<out TValue> : IIdentifiable
    {
        /// <summary>
        /// Gets the cache region for this key.
        /// </summary>
        string Region { get; }

        /// <summary>
        /// Gets the cache tags associated with this key.
        /// </summary>
        IReadOnlyCollection<string> Tags { get; }

        /// <summary>
        /// Gets the cache key string representation.
        /// </summary>
        /// <returns>The cache key string.</returns>
        string ToString();
    }
}
```

**Caching Examples:**
```csharp
// Cache key implementation
public record UserCacheKey : ICacheKey<UserDto>
{
    public UserCacheKey(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; }
    public string Id => $"User:{UserId}";
    public string Region => "Users";
    public IReadOnlyCollection<string> Tags => new[] { "User", $"UserId:{UserId}" };

    public override string ToString() => Id;
}

// Service with caching
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    private readonly ICache<UserDto> _cache;
    private readonly IUserRepository _repository;

    public async Task<IEnvelope<UserCode, UserError>> GetUserAsync(string userId)
    {
        var cacheKey = new UserCacheKey(userId);
        
        // Try cache first
        var cached = await _cache.GetAsync(cacheKey);
        if (cached.HasValue)
        {
            return Envelope.Success(UserCode.UserFound, cached.Value);
        }

        // Load from repository
        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            return Envelope.NotFound<UserCode, UserError>(
                UserError.UserNotFound(userId)
            );
        }

        // Cache the result
        var cachePolicy = new CachePolicy
        {
            ExpiresAfter = TimeSpan.FromHours(1),
            Priority = CachePriority.High
        };
        
        await _cache.SetAsync(cacheKey, user, cachePolicy);

        return Envelope.Success(UserCode.UserFound, user);
    }
}
```

---

## Messaging & CQRS

### ICommand

Command pattern interface.

```csharp
namespace Zentient.Abstractions.Messaging
{
    /// <summary>
    /// Represents a command that can be executed.
    /// </summary>
    public interface ICommand : IIdentifiable, IHasTimestamp
    {
        /// <summary>
        /// Gets the correlation ID for tracking this command.
        /// </summary>
        string CorrelationId { get; }

        /// <summary>
        /// Gets the user ID of the command initiator.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Gets additional metadata for the command.
        /// </summary>
        IMetadata Metadata { get; }
    }

    /// <summary>
    /// Represents a command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the command result.</typeparam>
    public interface ICommand<out TResult> : ICommand
    {
    }
}
```

### IQuery<TResult>

Query pattern interface.

```csharp
namespace Zentient.Abstractions.Messaging
{
    /// <summary>
    /// Represents a query that returns data.
    /// </summary>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    public interface IQuery<out TResult> : IIdentifiable, IHasTimestamp
    {
        /// <summary>
        /// Gets the correlation ID for tracking this query.
        /// </summary>
        string CorrelationId { get; }

        /// <summary>
        /// Gets the user ID of the query initiator.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Gets additional metadata for the query.
        /// </summary>
        IMetadata Metadata { get; }
    }
}
```

### IEvent

Event pattern interface.

```csharp
namespace Zentient.Abstractions.Messaging
{
    /// <summary>
    /// Represents a domain event that has occurred.
    /// </summary>
    public interface IEvent : IIdentifiable, IHasTimestamp
    {
        /// <summary>
        /// Gets the correlation ID for tracking related events.
        /// </summary>
        string CorrelationId { get; }

        /// <summary>
        /// Gets the aggregate ID that this event relates to.
        /// </summary>
        string AggregateId { get; }

        /// <summary>
        /// Gets the version of the event schema.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets additional metadata for the event.
        /// </summary>
        IMetadata Metadata { get; }
    }
}
```

**CQRS Implementation Examples:**
```csharp
// Command implementation
public record CreateUserCommand : ICommand<UserId>
{
    public string Id => $"CreateUser.{CorrelationId}";
    public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    public string UserId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    
    public IMetadata Metadata => new MetadataCollection
    {
        ["Source"] = "WebAPI",
        ["UserAgent"] = UserAgent,
        ["IPAddress"] = IPAddress
    };
}

// Query implementation
public record GetUserByEmailQuery : IQuery<UserDto>
{
    public string Id => $"GetUserByEmail.{Email}.{Timestamp:yyyyMMddHHmmss}";
    public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    public string UserId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    
    public string Email { get; init; }
    public bool IncludeProfile { get; init; } = true;
    
    public IMetadata Metadata => new MetadataCollection
    {
        ["CacheEnabled"] = true,
        ["CacheDuration"] = "00:30:00"
    };
}

// Event implementation
public record UserCreatedEvent : IEvent
{
    public string Id => $"UserCreated.{AggregateId}.{Timestamp:yyyyMMddHHmmss}";
    public string CorrelationId { get; init; }
    public string AggregateId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string Version => "1.0";
    
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    
    public IMetadata Metadata => new MetadataCollection
    {
        ["EventType"] = "UserCreated",
        ["Source"] = "UserService",
        ["Schema"] = "https://schemas.company.com/events/user-created/v1.0.json"
    };
}
```

---

## Validation

### IValidator<T>

Validation interface for objects.

```csharp
namespace Zentient.Abstractions.Validation
{
    /// <summary>
    /// Defines a contract for validating objects.
    /// </summary>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    public interface IValidator<in T>
    {
        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The validation result.</returns>
        Task<IValidationResult> ValidateAsync(T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates the specified object synchronously.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <returns>The validation result.</returns>
        IValidationResult Validate(T value);
    }
}
```

### IValidationResult

Validation result interface.

```csharp
namespace Zentient.Abstractions.Validation
{
    /// <summary>
    /// Represents the result of a validation operation.
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether the validation was successful.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the collection of validation errors.
        /// </summary>
        IReadOnlyCollection<IValidationError> Errors { get; }

        /// <summary>
        /// Gets additional metadata about the validation.
        /// </summary>
        IMetadata Metadata { get; }
    }
}
```

**Validation Examples:**
```csharp
// Validator implementation
[ServiceRegistration(ServiceLifetime.Scoped)]
public class CreateUserCommandValidator : IValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public async Task<IValidationResult> ValidateAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<IValidationError>();

        // Email validation
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            errors.Add(new ValidationError("Email", "Email is required"));
        }
        else if (!IsValidEmail(command.Email))
        {
            errors.Add(new ValidationError("Email", "Email format is invalid"));
        }
        else
        {
            // Check for duplicate email
            var existingUser = await _userRepository.GetByEmailAsync(command.Email);
            if (existingUser != null)
            {
                errors.Add(new ValidationError("Email", "Email is already registered"));
            }
        }

        // Name validation
        if (string.IsNullOrWhiteSpace(command.FirstName))
        {
            errors.Add(new ValidationError("FirstName", "First name is required"));
        }

        if (string.IsNullOrWhiteSpace(command.LastName))
        {
            errors.Add(new ValidationError("LastName", "Last name is required"));
        }

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            Metadata = new MetadataCollection
            {
                ["ValidatedAt"] = DateTime.UtcNow,
                ["Validator"] = GetType().Name
            }
        };
    }
}

// Usage in command handler
[CommandHandler(typeof(CreateUserCommand))]
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserId>
{
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly IUserRepository _repository;

    public async Task<IEnvelope<CommandCode, CommandError>> HandleAsync(CreateUserCommand command)
    {
        // Validate command
        var validationResult = await _validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return Envelope.ValidationError<CommandCode, CommandError>(
                validationResult.Errors.Select(e => CommandError.ValidationFailed(e.Message))
            );
        }

        // Process command...
    }
}
```

---

## Diagnostics & Health Checks

### IDiagnosticCheck<TSubject, TCodeDefinition, TErrorDefinition>

Health check interface for system components.

```csharp
namespace Zentient.Abstractions.Diagnostics
{
    /// <summary>
    /// Defines a contract for performing diagnostic checks on system components.
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject being checked.</typeparam>
    /// <typeparam name="TCodeDefinition">The type of diagnostic codes.</typeparam>
    /// <typeparam name="TErrorDefinition">The type of error definitions.</typeparam>
    public interface IDiagnosticCheck<in TSubject, TCodeDefinition, TErrorDefinition> : IIdentifiable, IHasName
        where TCodeDefinition : ICodeDefinition
        where TErrorDefinition : IErrorDefinition
    {
        /// <summary>
        /// Performs a diagnostic check on the specified subject.
        /// </summary>
        /// <param name="subject">The subject to check.</param>
        /// <param name="context">The diagnostic context.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The diagnostic report.</returns>
        Task<IDiagnosticReport<TCodeDefinition, TErrorDefinition>> CheckAsync(
            TSubject subject,
            IDiagnosticContext context,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the timeout for this diagnostic check.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Gets the priority of this diagnostic check.
        /// </summary>
        Priority Priority { get; }

        /// <summary>
        /// Gets the tags associated with this diagnostic check.
        /// </summary>
        IReadOnlyCollection<string> Tags { get; }
    }
}
```

**Health Check Examples:**
```csharp
// Database health check
[DiagnosticCheck("Database.Connectivity")]
[ServiceRegistration(ServiceLifetime.Singleton)]
public class DatabaseHealthCheck : IDiagnosticCheck<DatabaseContext, HealthCode, HealthError>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public string Id => "Database.Connectivity.Check.v1";
    public string Name => "Database Connectivity Check";
    public TimeSpan Timeout => TimeSpan.FromSeconds(30);
    public Priority Priority => Priority.Critical;
    public IReadOnlyCollection<string> Tags => new[] { "Database", "Infrastructure", "Critical" };

    public async Task<IDiagnosticReport<HealthCode, HealthError>> CheckAsync(
        DatabaseContext subject,
        IDiagnosticContext context,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Test connectivity
            await subject.Database.CanConnectAsync(cancellationToken);
            
            // Test simple query
            var userCount = await subject.Users.CountAsync(cancellationToken);
            
            stopwatch.Stop();

            var metadata = new MetadataCollection
            {
                ["ResponseTime"] = stopwatch.ElapsedMilliseconds,
                ["UserCount"] = userCount,
                ["ServerVersion"] = subject.Database.GetDbConnection().ServerVersion
            };

            if (stopwatch.ElapsedMilliseconds > 5000)
            {
                return DiagnosticReport.Warning(
                    HealthCode.SlowResponse,
                    "Database response time is slow",
                    metadata
                );
            }

            return DiagnosticReport.Healthy(
                HealthCode.DatabaseHealthy,
                "Database is responding normally",
                metadata
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Database health check failed");
            
            return DiagnosticReport.Unhealthy(
                HealthError.DatabaseUnavailable(ex.Message),
                $"Database check failed: {ex.Message}",
                new MetadataCollection
                {
                    ["Exception"] = ex.GetType().Name,
                    ["ElapsedTime"] = stopwatch.ElapsedMilliseconds
                }
            );
        }
    }
}

// External service health check
[DiagnosticCheck("ExternalService.PaymentGateway")]
public class PaymentGatewayHealthCheck : IDiagnosticCheck<PaymentGatewayClient, HealthCode, HealthError>
{
    public string Id => "PaymentGateway.Health.Check.v1";
    public string Name => "Payment Gateway Health Check";
    public TimeSpan Timeout => TimeSpan.FromSeconds(15);
    public Priority Priority => Priority.High;
    public IReadOnlyCollection<string> Tags => new[] { "External", "Payment", "Business" };

    public async Task<IDiagnosticReport<HealthCode, HealthError>> CheckAsync(
        PaymentGatewayClient subject,
        IDiagnosticContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var healthResult = await subject.CheckHealthAsync(cancellationToken);
            
            return healthResult.IsHealthy
                ? DiagnosticReport.Healthy(HealthCode.ServiceHealthy, "Payment gateway is operational")
                : DiagnosticReport.Degraded(HealthCode.ServiceDegraded, healthResult.Message);
        }
        catch (HttpRequestException ex)
        {
            return DiagnosticReport.Unhealthy(
                HealthError.ServiceUnavailable("PaymentGateway", ex.Message),
                "Payment gateway is unreachable"
            );
        }
    }
}
```

---

## Observability

### ILogger<T>

Structured logging interface.

```csharp
namespace Zentient.Abstractions.Observability
{
    /// <summary>
    /// Defines a contract for structured logging.
    /// </summary>
    /// <typeparam name="T">The type whose name is used for the logger category.</typeparam>
    public interface ILogger<T> : IIdentifiable
    {
        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="args">The message arguments.</param>
        void Log(LogLevel level, string message, params object[] args);

        /// <summary>
        /// Writes a log entry with exception information.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The log message.</param>
        /// <param name="args">The message arguments.</param>
        void Log(LogLevel level, Exception exception, string message, params object[] args);

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>A disposable object that ends the logical operation scope on dispose.</returns>
        IDisposable BeginScope<TState>(TState state);

        /// <summary>
        /// Checks if the given log level is enabled.
        /// </summary>
        /// <param name="level">The log level to check.</param>
        /// <returns>True if enabled; false otherwise.</returns>
        bool IsEnabled(LogLevel level);
    }
}
```

### IMeter

Metrics collection interface.

```csharp
namespace Zentient.Abstractions.Observability
{
    /// <summary>
    /// Defines a contract for collecting application metrics.
    /// </summary>
    public interface IMeter : IIdentifiable, IDisposable
    {
        /// <summary>
        /// Creates a counter instrument.
        /// </summary>
        /// <typeparam name="T">The type of the counter value.</typeparam>
        /// <param name="name">The counter name.</param>
        /// <param name="unit">The unit of measurement.</param>
        /// <param name="description">The counter description.</param>
        /// <returns>A counter instrument.</returns>
        Counter<T> CreateCounter<T>(string name, string unit = null, string description = null)
            where T : struct;

        /// <summary>
        /// Creates a histogram instrument.
        /// </summary>
        /// <typeparam name="T">The type of the histogram value.</typeparam>
        /// <param name="name">The histogram name.</param>
        /// <param name="unit">The unit of measurement.</param>
        /// <param name="description">The histogram description.</param>
        /// <returns>A histogram instrument.</returns>
        Histogram<T> CreateHistogram<T>(string name, string unit = null, string description = null)
            where T : struct;

        /// <summary>
        /// Creates a gauge instrument.
        /// </summary>
        /// <typeparam name="T">The type of the gauge value.</typeparam>
        /// <param name="name">The gauge name.</param>
        /// <param name="unit">The unit of measurement.</param>
        /// <param name="description">The gauge description.</param>
        /// <returns>A gauge instrument.</returns>
        Gauge<T> CreateGauge<T>(string name, string unit = null, string description = null)
            where T : struct;
    }
}
```

### ITracer<T>

Distributed tracing interface.

```csharp
namespace Zentient.Abstractions.Observability
{
    /// <summary>
    /// Defines a contract for distributed tracing.
    /// </summary>
    /// <typeparam name="T">The type whose name is used for the tracer category.</typeparam>
    public interface ITracer<T> : IIdentifiable
    {
        /// <summary>
        /// Starts a new activity with the specified name.
        /// </summary>
        /// <param name="name">The activity name.</param>
        /// <returns>A new activity or null if no listeners are active.</returns>
        Activity StartActivity(string name);

        /// <summary>
        /// Starts a new activity with the specified name and kind.
        /// </summary>
        /// <param name="name">The activity name.</param>
        /// <param name="kind">The activity kind.</param>
        /// <returns>A new activity or null if no listeners are active.</returns>
        Activity StartActivity(string name, ActivityKind kind);

        /// <summary>
        /// Starts a new activity with the specified name, kind, and parent context.
        /// </summary>
        /// <param name="name">The activity name.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="parentContext">The parent activity context.</param>
        /// <returns>A new activity or null if no listeners are active.</returns>
        Activity StartActivity(string name, ActivityKind kind, ActivityContext parentContext);

        /// <summary>
        /// Gets the current activity.
        /// </summary>
        Activity Current { get; }
    }
}
```

**Observability Examples:**
```csharp
// Service with comprehensive observability
[ServiceRegistration(ServiceLifetime.Scoped)]
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IMeter _meter;
    private readonly ITracer<OrderService> _tracer;
    
    private readonly Counter<long> _orderCreations;
    private readonly Histogram<double> _orderProcessingTime;
    private readonly Gauge<int> _pendingOrders;

    public OrderService(
        ILogger<OrderService> logger,
        IMeter meter,
        ITracer<OrderService> tracer)
    {
        _logger = logger;
        _meter = meter;
        _tracer = tracer;
        
        // Initialize metrics
        _orderCreations = meter.CreateCounter<long>("orders_created_total", "count", "Total number of orders created");
        _orderProcessingTime = meter.CreateHistogram<double>("order_processing_duration", "ms", "Order processing time");
        _pendingOrders = meter.CreateGauge<int>("pending_orders_count", "count", "Number of pending orders");
    }

    public async Task<IEnvelope<OrderCode, OrderError>> CreateOrderAsync(CreateOrderRequest request)
    {
        using var activity = _tracer.StartActivity("CreateOrder");
        activity?.SetTag("order.customer_id", request.CustomerId);
        activity?.SetTag("order.item_count", request.Items.Count);
        
        var stopwatch = Stopwatch.StartNew();
        
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = request.CorrelationId,
            ["CustomerId"] = request.CustomerId,
            ["OrderType"] = request.OrderType
        });

        try
        {
            _logger.LogInformation("Creating order for customer {CustomerId} with {ItemCount} items", 
                request.CustomerId, request.Items.Count);

            // Validate request
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Validation failed");
                _logger.LogWarning("Order validation failed: {Errors}", 
                    string.Join(", ", validationResult.Errors.Select(e => e.Message)));
                
                return Envelope.ValidationError<OrderCode, OrderError>(
                    validationResult.Errors.Select(e => OrderError.ValidationFailed(e.Message))
                );
            }

            // Create order
            var order = await _repository.CreateAsync(new Order
            {
                Id = OrderId.NewId(),
                CustomerId = request.CustomerId,
                Items = request.Items,
                CreatedAt = DateTime.UtcNow
            });

            stopwatch.Stop();
            
            // Record metrics
            _orderCreations.Add(1, new TagList
            {
                ["customer_type"] = request.CustomerType,
                ["order_type"] = request.OrderType,
                ["success"] = "true"
            });
            
            _orderProcessingTime.Record(stopwatch.ElapsedMilliseconds, new TagList
            {
                ["operation"] = "create",
                ["outcome"] = "success"
            });

            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("order.id", order.Id.Value);

            _logger.LogInformation("Order {OrderId} created successfully in {ElapsedMs}ms", 
                order.Id, stopwatch.ElapsedMilliseconds);

            return Envelope.Success(OrderCode.OrderCreated, order);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Record error metrics
            _orderCreations.Add(1, new TagList
            {
                ["customer_type"] = request.CustomerType,
                ["order_type"] = request.OrderType,
                ["success"] = "false"
            });

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().Name);

            _logger.LogError(ex, "Failed to create order for customer {CustomerId}", request.CustomerId);

            return Envelope.Error<OrderCode, OrderError>(
                OrderError.CreationFailed(ex.Message)
            );
        }
    }
}
```

---

## Policies & Resilience

### IRetryable

Retry capability interface.

```csharp
namespace Zentient.Abstractions.Policies
{
    /// <summary>
    /// Defines a contract for operations that can be retried.
    /// </summary>
    public interface IRetryable
    {
        /// <summary>
        /// Gets a value indicating whether the operation can be retried.
        /// </summary>
        bool CanRetry { get; }

        /// <summary>
        /// Gets the maximum number of retry attempts.
        /// </summary>
        int MaxRetries { get; }

        /// <summary>
        /// Gets the current retry attempt number.
        /// </summary>
        int CurrentRetry { get; }

        /// <summary>
        /// Gets the delay between retry attempts.
        /// </summary>
        TimeSpan RetryDelay { get; }

        /// <summary>
        /// Determines whether the specified exception should trigger a retry.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        /// <returns>True if the operation should be retried; otherwise, false.</returns>
        bool ShouldRetry(Exception exception);
    }
}
```

### IPolicy<T>

Policy execution interface.

```csharp
namespace Zentient.Abstractions.Policies
{
    /// <summary>
    /// Defines a contract for executing operations with policy enforcement.
    /// </summary>
    /// <typeparam name="T">The type of the operation result.</typeparam>
    public interface IPolicy<T> : IIdentifiable, IHasName
    {
        /// <summary>
        /// Executes the specified operation with policy enforcement.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The operation result wrapped in an envelope.</returns>
        Task<IEnvelope<PolicyCode, PolicyError>> ExecuteAsync(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the specified operation with policy enforcement and context.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The operation result wrapped in an envelope.</returns>
        Task<IEnvelope<PolicyCode, PolicyError>> ExecuteAsync(
            Func<Context, CancellationToken, Task<T>> operation,
            Context context,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the policy configuration.
        /// </summary>
        IPolicyConfiguration Configuration { get; }
    }
}
```

**Policy Implementation Examples:**
```csharp
// Retry policy implementation
[ServiceRegistration(ServiceLifetime.Singleton)]
public class DatabaseRetryPolicy : IPolicy<DatabaseResult>, IRetryable
{
    public string Id => "Database.Retry.Policy.v1";
    public string Name => "Database Retry Policy";
    
    public bool CanRetry => CurrentRetry < MaxRetries;
    public int MaxRetries => 3;
    public int CurrentRetry { get; private set; }
    public TimeSpan RetryDelay => TimeSpan.FromMilliseconds(1000 * Math.Pow(2, CurrentRetry));

    public bool ShouldRetry(Exception exception)
    {
        return exception is SqlException sqlEx && IsTransientError(sqlEx.Number) ||
               exception is TimeoutException ||
               exception is TaskCanceledException;
    }

    public async Task<IEnvelope<PolicyCode, PolicyError>> ExecuteAsync(
        Func<CancellationToken, Task<DatabaseResult>> operation,
        CancellationToken cancellationToken = default)
    {
        var attempts = 0;
        Exception lastException = null;

        while (attempts <= MaxRetries)
        {
            try
            {
                CurrentRetry = attempts;
                var result = await operation(cancellationToken);
                
                return Envelope.Success(PolicyCode.OperationSucceeded, result);
            }
            catch (Exception ex) when (attempts < MaxRetries && ShouldRetry(ex))
            {
                lastException = ex;
                attempts++;
                
                var delay = TimeSpan.FromMilliseconds(1000 * Math.Pow(2, attempts - 1));
                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception ex)
            {
                return Envelope.Error<PolicyCode, PolicyError>(
                    PolicyError.OperationFailed(ex.Message)
                );
            }
        }

        return Envelope.Error<PolicyCode, PolicyError>(
            PolicyError.MaxRetriesExceeded(lastException?.Message)
        );
    }
}

// Circuit breaker policy
[ServiceRegistration(ServiceLifetime.Singleton)]
public class CircuitBreakerPolicy : IPolicy<ExternalServiceResult>
{
    private readonly ILogger<CircuitBreakerPolicy> _logger;
    private readonly object _lock = new object();
    
    private CircuitState _state = CircuitState.Closed;
    private int _failureCount = 0;
    private DateTime _lastFailureTime = DateTime.MinValue;
    private readonly int _failureThreshold = 5;
    private readonly TimeSpan _openTimeout = TimeSpan.FromMinutes(1);

    public string Id => "ExternalService.CircuitBreaker.Policy.v1";
    public string Name => "External Service Circuit Breaker";

    public async Task<IEnvelope<PolicyCode, PolicyError>> ExecuteAsync(
        Func<CancellationToken, Task<ExternalServiceResult>> operation,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_state == CircuitState.Open)
            {
                if (DateTime.UtcNow - _lastFailureTime < _openTimeout)
                {
                    return Envelope.Error<PolicyCode, PolicyError>(
                        PolicyError.CircuitBreakerOpen("External service circuit is open")
                    );
                }
                
                _state = CircuitState.HalfOpen;
            }
        }

        try
        {
            var result = await operation(cancellationToken);
            
            lock (_lock)
            {
                _failureCount = 0;
                _state = CircuitState.Closed;
            }
            
            return Envelope.Success(PolicyCode.OperationSucceeded, result);
        }
        catch (Exception ex)
        {
            lock (_lock)
            {
                _failureCount++;
                _lastFailureTime = DateTime.UtcNow;
                
                if (_failureCount >= _failureThreshold)
                {
                    _state = CircuitState.Open;
                    _logger.LogWarning("Circuit breaker opened due to {FailureCount} failures", _failureCount);
                }
            }
            
            return Envelope.Error<PolicyCode, PolicyError>(
                PolicyError.OperationFailed(ex.Message)
            );
        }
    }
}

// Usage in service
[ServiceRegistration(ServiceLifetime.Scoped)]
public class PaymentService : IPaymentService
{
    private readonly IPolicy<PaymentResult> _retryPolicy;
    private readonly IPolicy<PaymentResult> _circuitBreakerPolicy;
    private readonly IPaymentGateway _gateway;

    public async Task<IEnvelope<PaymentCode, PaymentError>> ProcessPaymentAsync(PaymentRequest request)
    {
        // Combine retry and circuit breaker policies
        var result = await _circuitBreakerPolicy.ExecuteAsync(async ct =>
        {
            return await _retryPolicy.ExecuteAsync(async retryToken =>
            {
                return await _gateway.ProcessPaymentAsync(request, retryToken);
            }, ct);
        });

        if (result.IsFailure)
        {
            return Envelope.Error<PaymentCode, PaymentError>(
                PaymentError.ProcessingFailed(result.Errors.First().Message)
            );
        }

        return Envelope.Success(PaymentCode.PaymentProcessed, result.Value);
    }
}
```

---

## 🔗 Related Documentation

- **[Getting Started Guide](./guides/getting-started.md)** - Quick start tutorial
- **[Best Practices](./guides/best-practices.md)** - Recommended patterns and practices
- **[Implementation Roadmap](./implementation-roadmap.md)** - Comprehensive implementation guide
- **[Enterprise Architecture](./enterprise-architecture.md)** - Enterprise-scale implementation patterns
- **[Migration Guide](./guides/migration-guide.md)** - Upgrading from previous versions

---

**Need help?** Check our [GitHub Discussions](https://github.com/Zentient/Zentient.Abstractions/discussions) or [open an issue](https://github.com/Zentient/Zentient.Abstractions/issues) for support.
