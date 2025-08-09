# 📨 CQRS & Messaging Architecture

## Overview

Zentient.Abstractions provides a comprehensive CQRS (Command Query Responsibility Segregation) and messaging infrastructure that enables building scalable, maintainable applications with clear separation of concerns. The framework offers type-safe command/query definitions, automatic handler discovery, and powerful envelope-based error handling.

## 🎯 Core Concepts

### **Commands & Queries as First-Class Citizens**

Commands and queries are not just data transfer objects - they are architectural components with rich metadata:

```csharp
[CommandDefinition("UserManagement.CreateUser", Version = "2.1.0")]
public record CreateUserCommand : ICommand<CreateUserCommand, UserCode, UserError>
{
    public string Id => $"CreateUser_{Email}_{Timestamp:yyyyMMddHHmmss}";
    public string Name => "Create User Command";
    public string Description => "Creates a new user account with profile information";
    
    // Command data
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime DateOfBirth { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    
    // Rich metadata for observability and routing
    public IMetadata Metadata => new MetadataCollection
    {
        ["Domain"] = "UserManagement",
        ["Subdomain"] = "Registration",
        ["BusinessProcess"] = "UserOnboarding",
        ["RequiredPermissions"] = new[] { "Users.Create" },
        ["AuditLevel"] = "High",
        ["RetentionPeriod"] = "7 years",
        ["PII"] = true,
        ["ExpectedProcessingTime"] = "< 500ms",
        ["BusinessImpact"] = "High"
    };
}

[QueryDefinition("UserManagement.GetUser", Version = "2.1.0")]
public record GetUserQuery : IQuery<GetUserQuery, UserDto, UserCode, UserError>
{
    public string Id => $"GetUser_{UserId}";
    public string Name => "Get User Query";
    public string Description => "Retrieves user information by user ID";
    
    public required string UserId { get; init; }
    public bool IncludeProfile { get; init; } = true;
    public bool IncludePreferences { get; init; } = false;
    
    public IMetadata Metadata => new MetadataCollection
    {
        ["Domain"] = "UserManagement",
        ["Subdomain"] = "Retrieval",
        ["CacheKey"] = $"User:{UserId}",
        ["CacheDuration"] = "PT15M", // 15 minutes
        ["RequiredPermissions"] = new[] { "Users.Read" },
        ["DataClassification"] = "Personal",
        ["ExpectedProcessingTime"] = "< 100ms"
    };
}
```

### **Type-Safe Handler Definitions**

Handlers implement specific interfaces that provide compile-time safety and runtime validation:

```csharp
[HandlerRegistration(ServiceLifetime.Scoped)]
public class CreateUserCommandHandler : 
    ICommandHandler<CreateUserCommand, UserCode, UserError>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    
    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }
    
    public async Task<IEnvelope<UserCode, UserError>> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        using var activity = Activity.StartActivity("CreateUser");
        activity?.SetTag("user.email", command.Email);
        
        try
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(command.Email);
            if (existingUser.IsSuccess)
            {
                return Envelope.Conflict<UserCode, UserError>(
                    UserError.EmailAlreadyExists(command.Email)
                );
            }
            
            // Create new user
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = command.Email,
                FirstName = command.FirstName,
                LastName = command.LastName,
                PhoneNumber = command.PhoneNumber,
                DateOfBirth = command.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            var createResult = await _userRepository.CreateAsync(user);
            if (!createResult.IsSuccess)
            {
                return createResult.ConvertError<UserCode, UserError>();
            }
            
            // Send welcome email
            var emailResult = await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);
            if (!emailResult.IsSuccess)
            {
                _logger.LogWarning("Failed to send welcome email to {Email}: {Error}", 
                    user.Email, emailResult.Error);
                // Continue - don't fail the command due to email issues
            }
            
            _logger.LogInformation("Created user {UserId} with email {Email}", user.Id, user.Email);
            
            return Envelope.Success(UserCode.Created, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email {Email}", command.Email);
            return Envelope.Error<UserCode, UserError>(
                UserError.CreationFailed($"Failed to create user: {ex.Message}")
            );
        }
    }
}

[HandlerRegistration(ServiceLifetime.Scoped)]
public class GetUserQueryHandler : 
    IQueryHandler<GetUserQuery, UserDto, UserCode, UserError>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ICache<UserDto> _cache;
    
    public async Task<IEnvelope<UserDto, UserCode, UserError>> HandleAsync(
        GetUserQuery query,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"User:{query.UserId}";
        
        // Try cache first
        var cached = await _cache.GetAsync(cacheKey);
        if (cached.IsSuccess)
        {
            return Envelope.Success(UserCode.Retrieved, cached.Value);
        }
        
        // Get from repository
        var userResult = await _userRepository.GetByIdAsync(query.UserId);
        if (!userResult.IsSuccess)
        {
            return userResult.ConvertError<UserDto, UserCode, UserError>();
        }
        
        // Map to DTO
        var dto = _mapper.Map<UserDto>(userResult.Value);
        
        // Cache for future requests
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15));
        
        return Envelope.Success(UserCode.Retrieved, dto);
    }
}
```

## 🚀 Message Dispatch & Routing

### **Automatic Handler Discovery**

Handlers are automatically discovered and registered:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddZentientMessaging(builder =>
    {
        // Scan assemblies for handlers
        builder
            .ScanAssemblies(
                typeof(CreateUserCommandHandler).Assembly,  // Application handlers
                typeof(UserRepository).Assembly,            // Infrastructure handlers
                typeof(EmailService).Assembly               // External service handlers
            )
            .RegisterCommandHandlers()
            .RegisterQueryHandlers()
            .RegisterEventHandlers()
            .RegisterValidators()
            
            // Configure behavior pipeline
            .AddValidation()
            .AddLogging()
            .AddMetrics()
            .AddCaching()
            .AddRetryPolicy()
            .AddCircuitBreaker()
            .AddDeadLetterQueue()
            
            // Configure message routing
            .ConfigureRouting(routing =>
            {
                routing
                    .RouteCommands(cmd => cmd.Metadata["Domain"])
                    .RouteQueries(query => query.Metadata["Domain"])
                    .RouteEvents(evt => evt.Metadata["EventType"]);
            });
    });
}
```

### **Message Dispatcher**

Central dispatcher that routes messages to appropriate handlers:

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMessageDispatcher _dispatcher;
    
    public UsersController(IMessageDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _dispatcher.DispatchAsync(command);
        return result.ToActionResult();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id, [FromQuery] bool includeProfile = true)
    {
        var query = new GetUserQuery
        {
            UserId = id,
            IncludeProfile = includeProfile
        };
        
        var result = await _dispatcher.DispatchAsync(query);
        return result.ToActionResult();
    }
    
    [HttpGet]
    public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersQuery query)
    {
        var result = await _dispatcher.DispatchAsync(query);
        return result.ToActionResult();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand command)
    {
        command = command with { UserId = id };
        var result = await _dispatcher.DispatchAsync(command);
        return result.ToActionResult();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var command = new DeleteUserCommand { UserId = id };
        var result = await _dispatcher.DispatchAsync(command);
        return result.ToActionResult();
    }
}
```

### **Advanced Routing Patterns**

Complex routing scenarios with conditional logic:

```csharp
services.AddZentientMessaging(builder =>
{
    builder.ConfigureRouting(routing =>
    {
        // Route based on command type and metadata
        routing
            .When<CreateUserCommand>()
            .RouteToHandler<CreateUserCommandHandler>()
            .WithValidation<CreateUserValidator>()
            .WithLogging(LogLevel.Information)
            .WithMetrics()
            .WithRetry(maxAttempts: 3);
            
        // Route high-priority commands to dedicated handlers
        routing
            .When(cmd => cmd.Metadata.GetValue<string>("Priority") == "High")
            .RouteToHandler<HighPriorityCommandHandler>()
            .WithTimeout(TimeSpan.FromSeconds(5))
            .WithCircuitBreaker(failureThreshold: 2);
            
        // Route based on tenant
        routing
            .When(cmd => cmd.Metadata.ContainsKey("TenantId"))
            .RouteToTenantSpecificHandler()
            .WithTenantIsolation()
            .WithTenantMetrics();
            
        // Route queries with caching
        routing
            .When<IQuery>()
            .WithCaching(duration: TimeSpan.FromMinutes(15))
            .WithCompressionThreshold(1024)
            .WithCache<RedisCache>();
            
        // Dead letter queue for failed messages
        routing
            .OnFailure()
            .After(attempts: 3)
            .RouteToDeadLetterQueue()
            .WithErrorAnalysis()
            .WithNotification();
    });
});
```

## 🔄 Event-Driven Architecture

### **Domain Events**

Rich domain events that capture business-meaningful occurrences:

```csharp
[EventDefinition("UserManagement.UserCreated", Version = "2.1.0")]
public record UserCreatedEvent : IDomainEvent<UserCreatedEvent, UserCode, UserError>
{
    public string Id => $"UserCreated_{UserId}_{Timestamp:yyyyMMddHHmmss}";
    public string Name => "User Created Event";
    public string Description => "Raised when a new user account is successfully created";
    
    public required string UserId { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    
    public IMetadata Metadata => new MetadataCollection
    {
        ["Domain"] = "UserManagement",
        ["EventType"] = "DomainEvent",
        ["AggregateType"] = "User",
        ["AggregateId"] = UserId,
        ["EventVersion"] = "2.1.0",
        ["CausationId"] = Id,
        ["CorrelationId"] = Id,
        ["BusinessProcess"] = "UserOnboarding",
        ["PublishStrategy"] = "Immediate",
        ["RetentionPeriod"] = "1 year"
    };
}

// Multiple event handlers for the same event
[EventHandlerRegistration(ServiceLifetime.Scoped)]
public class UserCreatedEmailHandler : IEventHandler<UserCreatedEvent, UserCode, UserError>
{
    private readonly IEmailService _emailService;
    
    public async Task<IEnvelope<UserCode, UserError>> HandleAsync(
        UserCreatedEvent @event,
        CancellationToken cancellationToken = default)
    {
        return await _emailService.SendWelcomeEmailAsync(@event.Email, @event.FirstName);
    }
}

[EventHandlerRegistration(ServiceLifetime.Scoped)]
public class UserCreatedAnalyticsHandler : IEventHandler<UserCreatedEvent, UserCode, UserError>
{
    private readonly IAnalyticsService _analytics;
    
    public async Task<IEnvelope<UserCode, UserError>> HandleAsync(
        UserCreatedEvent @event,
        CancellationToken cancellationToken = default)
    {
        return await _analytics.TrackUserRegistrationAsync(@event.UserId, @event.Email);
    }
}

[EventHandlerRegistration(ServiceLifetime.Scoped)]
public class UserCreatedNotificationHandler : IEventHandler<UserCreatedEvent, UserCode, UserError>
{
    private readonly INotificationService _notifications;
    
    public async Task<IEnvelope<UserCode, UserError>> HandleAsync(
        UserCreatedEvent @event,
        CancellationToken cancellationToken = default)
    {
        return await _notifications.NotifyAdministratorsAsync(
            $"New user registered: {@event.FirstName} {@event.LastName} ({@event.Email})"
        );
    }
}
```

### **Event Publishing Patterns**

Different event publishing strategies for various scenarios:

```csharp
public class CreateUserCommandHandler : 
    ICommandHandler<CreateUserCommand, UserCode, UserError>
{
    private readonly IEventPublisher _eventPublisher;
    
    public async Task<IEnvelope<UserCode, UserError>> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        // ... user creation logic ...
        
        var @event = new UserCreatedEvent
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        };
        
        // Immediate publishing (within same transaction)
        await _eventPublisher.PublishAsync(@event, PublishStrategy.Immediate);
        
        // Deferred publishing (after transaction commits)
        await _eventPublisher.PublishAsync(@event, PublishStrategy.Deferred);
        
        // Background publishing (fire and forget)
        _eventPublisher.PublishBackground(@event);
        
        // Scheduled publishing (publish at specific time)
        await _eventPublisher.PublishScheduledAsync(@event, DateTime.UtcNow.AddMinutes(5));
        
        return Envelope.Success(UserCode.Created, user);
    }
}
```

### **Saga Pattern Implementation**

Long-running business processes with compensation:

```csharp
[SagaDefinition("UserOnboarding", Version = "1.0.0")]
public class UserOnboardingSaga : 
    ISaga<UserOnboardingSaga, UserCode, UserError>,
    IEventHandler<UserCreatedEvent, UserCode, UserError>,
    IEventHandler<EmailSentEvent, UserCode, UserError>,
    IEventHandler<ProfileCreatedEvent, UserCode, UserError>
{
    public string Id => $"UserOnboarding_{Data.UserId}";
    public UserOnboardingData Data { get; set; } = new();
    
    // Handle UserCreatedEvent - start of saga
    public async Task<IEnvelope<UserCode, UserError>> HandleAsync(
        UserCreatedEvent @event,
        CancellationToken cancellationToken = default)
    {
        Data.UserId = @event.UserId;
        Data.Email = @event.Email;
        Data.UserCreated = true;
        Data.StartedAt = DateTime.UtcNow;
        
        // Send welcome email
        var emailCommand = new SendWelcomeEmailCommand
        {
            UserId = @event.UserId,
            Email = @event.Email,
            FirstName = @event.FirstName
        };
        
        return await PublishCommandAsync(emailCommand);
    }
    
    // Handle EmailSentEvent - continue saga
    public async Task<IEnvelope<UserCode, UserError>> HandleAsync(
        EmailSentEvent @event,
        CancellationToken cancellationToken = default)
    {
        if (@event.UserId != Data.UserId) return Envelope.Success(UserCode.Ignored);
        
        Data.WelcomeEmailSent = true;
        
        // Create user profile
        var profileCommand = new CreateUserProfileCommand
        {
            UserId = Data.UserId,
            Email = Data.Email
        };
        
        return await PublishCommandAsync(profileCommand);
    }
    
    // Handle ProfileCreatedEvent - complete saga
    public async Task<IEnvelope<UserCode, UserError>> HandleAsync(
        ProfileCreatedEvent @event,
        CancellationToken cancellationToken = default)
    {
        if (@event.UserId != Data.UserId) return Envelope.Success(UserCode.Ignored);
        
        Data.ProfileCreated = true;
        Data.CompletedAt = DateTime.UtcNow;
        
        // Publish completion event
        var completedEvent = new UserOnboardingCompletedEvent
        {
            UserId = Data.UserId,
            Email = Data.Email,
            Duration = Data.CompletedAt.Value - Data.StartedAt
        };
        
        await PublishEventAsync(completedEvent);
        
        // Mark saga as complete
        Complete();
        
        return Envelope.Success(UserCode.OnboardingCompleted);
    }
    
    // Compensation logic if saga fails
    public async Task<IEnvelope<UserCode, UserError>> CompensateAsync(
        CancellationToken cancellationToken = default)
    {
        var compensationCommands = new List<ICommand>();
        
        if (Data.ProfileCreated)
        {
            compensationCommands.Add(new DeleteUserProfileCommand { UserId = Data.UserId });
        }
        
        if (Data.WelcomeEmailSent)
        {
            compensationCommands.Add(new SendApologyEmailCommand 
            { 
                UserId = Data.UserId,
                Reason = "Account creation failed"
            });
        }
        
        if (Data.UserCreated)
        {
            compensationCommands.Add(new DeleteUserCommand { UserId = Data.UserId });
        }
        
        foreach (var command in compensationCommands)
        {
            await PublishCommandAsync(command);
        }
        
        return Envelope.Success(UserCode.Compensated);
    }
}

public class UserOnboardingData
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool UserCreated { get; set; }
    public bool WelcomeEmailSent { get; set; }
    public bool ProfileCreated { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
```

## 🔗 Message Pipelines & Behaviors

### **Behavior Pipeline**

Cross-cutting concerns applied through pipeline behaviors:

```csharp
// Validation behavior
public class ValidationBehavior<TRequest, TResponse> : 
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var failures = _validators
            .Select(validator => validator.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();
            
        if (failures.Any())
        {
            return CreateValidationErrorResponse<TResponse>(failures);
        }
        
        return await next();
    }
}

// Caching behavior for queries
public class CachingBehavior<TQuery, TResponse> : 
    IPipelineBehavior<TQuery, TResponse>
    where TQuery : class, IQuery
{
    private readonly ICache _cache;
    
    public async Task<TResponse> HandleAsync(
        TQuery query,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GenerateCacheKey(query);
        var cacheDuration = GetCacheDuration(query);
        
        var cached = await _cache.GetAsync<TResponse>(cacheKey);
        if (cached.IsSuccess)
        {
            return cached.Value;
        }
        
        var response = await next();
        
        if (IsSuccessResponse(response))
        {
            await _cache.SetAsync(cacheKey, response, cacheDuration);
        }
        
        return response;
    }
}

// Performance monitoring behavior
public class PerformanceBehavior<TRequest, TResponse> : 
    IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly IMetrics _metrics;
    
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        using var activity = Activity.StartActivity($"Handle {requestName}");
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            
            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;
            
            _metrics.RecordDuration($"message.duration", duration, new()
            {
                ["message_type"] = requestName,
                ["status"] = "success"
            });
            
            if (duration > 1000) // Log slow requests
            {
                _logger.LogWarning("Slow request detected: {RequestName} took {Duration}ms", 
                    requestName, duration);
            }
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _metrics.RecordDuration($"message.duration", stopwatch.ElapsedMilliseconds, new()
            {
                ["message_type"] = requestName,
                ["status"] = "error"
            });
            
            _logger.LogError(ex, "Error handling {RequestName}", requestName);
            throw;
        }
    }
}

// Registration of behaviors
services.AddZentientMessaging(builder =>
{
    builder
        .AddBehavior<ValidationBehavior<,>>()     // First - validate input
        .AddBehavior<CachingBehavior<,>>()        // Second - check cache
        .AddBehavior<PerformanceBehavior<,>>()    // Third - monitor performance
        .AddBehavior<LoggingBehavior<,>>()        // Fourth - log execution
        .AddBehavior<RetryBehavior<,>>()          // Fifth - retry on failures
        .AddBehavior<CircuitBreakerBehavior<,>>(); // Sixth - circuit breaker
});
```

## 🌐 Distributed Messaging

### **Message Bus Integration**

Integration with external message brokers:

```csharp
services.AddZentientMessaging(builder =>
{
    builder
        // Configure message bus
        .UseMessageBus(bus =>
        {
            bus
                .UseServiceBus(config =>
                {
                    config.ConnectionString = "Endpoint=sb://...";
                    config.DefaultTimeToLive = TimeSpan.FromDays(7);
                    config.MaxDeliveryCount = 5;
                })
                .UseRabbitMQ(config =>
                {
                    config.HostName = "localhost";
                    config.UserName = "guest";
                    config.Password = "guest";
                    config.VirtualHost = "/";
                })
                .UseKafka(config =>
                {
                    config.BootstrapServers = "localhost:9092";
                    config.GroupId = "zentient-consumers";
                });
        })
        
        // Configure message publishing
        .ConfigurePublishing(publishing =>
        {
            publishing
                .PublishCommands().ToTopic("commands")
                .PublishEvents().ToTopic("events")
                .PublishQueries().ToTopic("queries")
                .WithPartitioning(msg => msg.Metadata["TenantId"])
                .WithCompression()
                .WithEncryption()
                .WithDeduplication();
        })
        
        // Configure message consumption
        .ConfigureConsumption(consumption =>
        {
            consumption
                .ConsumeFrom("commands").WithConcurrency(10)
                .ConsumeFrom("events").WithConcurrency(20)
                .ConsumeFrom("queries").WithConcurrency(5)
                .WithDeadLetterQueue()
                .WithPoisonMessageHandling()
                .WithAutoScaling();
        });
});
```

### **Message Serialization & Versioning**

Handle message evolution and compatibility:

```csharp
// Message version compatibility
[MessageVersion("1.0.0")]
public record CreateUserCommandV1 : ICommand<CreateUserCommandV1, UserCode, UserError>
{
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}

[MessageVersion("2.0.0")]
public record CreateUserCommandV2 : ICommand<CreateUserCommandV2, UserCode, UserError>
{
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    
    // Migration from V1
    public static CreateUserCommandV2 FromV1(CreateUserCommandV1 v1)
    {
        var nameParts = v1.Name.Split(' ', 2);
        return new CreateUserCommandV2
        {
            Email = v1.Email,
            FirstName = nameParts.Length > 0 ? nameParts[0] : "",
            LastName = nameParts.Length > 1 ? nameParts[1] : ""
        };
    }
}

// Version-aware message handler
public class CreateUserCommandHandler : 
    ICommandHandler<CreateUserCommandV1, UserCode, UserError>,
    ICommandHandler<CreateUserCommandV2, UserCode, UserError>
{
    // Handle V1 messages by converting to V2
    public async Task<IEnvelope<UserCode, UserError>> HandleAsync(
        CreateUserCommandV1 command,
        CancellationToken cancellationToken = default)
    {
        var v2Command = CreateUserCommandV2.FromV1(command);
        return await HandleAsync(v2Command, cancellationToken);
    }
    
    // Handle V2 messages directly
    public async Task<IEnvelope<UserCode, UserError>> HandleAsync(
        CreateUserCommandV2 command,
        CancellationToken cancellationToken = default)
    {
        // Implementation using V2 structure
    }
}
```

---

**Zentient's CQRS and messaging architecture provides a robust foundation for building scalable, maintainable applications with clear separation of concerns, rich error handling, and comprehensive observability.**
