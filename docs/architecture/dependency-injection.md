# 🏗️ Dependency Injection Architecture

## Overview

Zentient.Abstractions transforms dependency injection from a simple registration mechanism into a powerful architectural tool. The framework provides fluent APIs, automatic discovery, rich validation, and advanced composition patterns that enable building robust, maintainable applications.

## 🎯 Core Concepts

### **Service Definitions as Architecture**

Services are not just implementations - they are architectural components with rich metadata:

```csharp
[ServiceDefinition("UserManagement", Version = "3.1.0")]
public record UserServiceDefinition : IServiceDefinition
{
    public string Id => "UserService.v3.1";
    public string Name => "User Management Service";
    public string Description => "Handles user lifecycle, authentication, and profile management";
    
    public IMetadata Metadata => new MetadataCollection
    {
        // Architectural information
        ["Layer"] = "Application",
        ["Domain"] = "Identity",
        ["Subdomain"] = "UserManagement",
        
        // Dependencies
        ["Dependencies"] = new[]
        {
            "IUserRepository",
            "IEmailService", 
            "IAuthenticationService",
            "ICache<UserDto>"
        },
        
        // Capabilities
        ["Capabilities"] = new[]
        {
            "UserRegistration",
            "UserAuthentication", 
            "ProfileManagement",
            "PasswordReset"
        },
        
        // Quality attributes
        ["Performance.ResponseTime"] = "< 100ms",
        ["Reliability.Availability"] = "99.9%",
        ["Security.Level"] = "High",
        ["Scalability.MaxThroughput"] = "10000 req/min"
    };
}
```

### **Automatic Service Discovery**

Services are discovered and registered automatically through assembly scanning:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddZentientServices(builder =>
    {
        // Scan assemblies for services
        builder
            .ScanAssemblies(
                typeof(UserService).Assembly,      // Application layer
                typeof(UserRepository).Assembly,   // Infrastructure layer
                typeof(EmailService).Assembly      // External services layer
            )
            .RegisterServicesWithAttribute<ServiceRegistrationAttribute>()
            .RegisterValidatorsWithInterface<IValidator>()
            .RegisterRepositoriesWithInterface<IRepository>()
            .RegisterHandlersWithInterface<IHandler>();
    });
}
```

## 🔧 Registration Patterns

### **Attribute-Based Registration**

The simplest and most common registration pattern:

```csharp
// Basic registration
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    // Automatically registered as IUserService with Scoped lifetime
}

// Multiple interface registration
[ServiceRegistration(typeof(IUserService), typeof(IUserManager), ServiceLifetime.Scoped)]
public class UserService : IUserService, IUserManager
{
    // Registered for both IUserService and IUserManager
}

// Named registration for multiple implementations
[ServiceRegistration(ServiceLifetime.Singleton, Name = "DefaultEmailService")]
[ServiceRegistration(ServiceLifetime.Singleton, Name = "BackupEmailService")]
public class EmailService : IEmailService
{
    // Can be resolved by name
}

// Conditional registration
[ServiceRegistration(ServiceLifetime.Scoped, Condition = "Environment:Development")]
public class MockPaymentService : IPaymentService
{
    // Only registered in development environment
}

// Factory registration
[ServiceRegistration(ServiceLifetime.Singleton, UseFactory = true)]
public class DatabaseService : IDatabaseService
{
    public static IDatabaseService CreateInstance(IServiceProvider provider)
    {
        var config = provider.GetRequiredService<IConfiguration>();
        var connectionString = config.GetConnectionString("Default");
        return new DatabaseService(connectionString);
    }
}
```

### **Fluent API Registration**

For complex scenarios requiring programmatic registration:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddZentientServices(builder =>
    {
        // Service registration with cross-cutting concerns
        builder
            .RegisterScoped<IUserService, UserService>()
            .AddValidation<CreateUserRequestValidator>()
            .AddCaching(TimeSpan.FromMinutes(30))
            .AddRetryPolicy(maxAttempts: 3)
            .AddCircuitBreaker(failureThreshold: 5)
            .AddLogging(LogLevel.Information)
            .AddMetrics()
            .AddHealthCheck();
            
        // Repository registration with specific configuration
        builder
            .RegisterScoped<IUserRepository, SqlUserRepository>()
            .Configure<DatabaseOptions>(options =>
            {
                options.CommandTimeout = TimeSpan.FromSeconds(30);
                options.RetryCount = 3;
            })
            .AddConnectionPooling(maxConnections: 100)
            .AddQueryLogging()
            .AddPerformanceMonitoring();
            
        // External service registration with policies
        builder
            .RegisterHttpClient<IPaymentGateway, StripePaymentGateway>(client =>
            {
                client.BaseAddress = new Uri("https://api.stripe.com/");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddRetryPolicy(3)
            .AddCircuitBreaker(5, TimeSpan.FromMinutes(1))
            .AddTimeout(TimeSpan.FromSeconds(30))
            .AddLogging();
    });
}
```

### **Generic Service Registration**

Register generic services with type constraints:

```csharp
// Generic repository registration
[ServiceRegistration(ServiceLifetime.Scoped)]
public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class, IIdentifiable
{
    private readonly DbContext _context;
    
    public Repository(DbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnvelope<RepositoryCode, RepositoryError>> GetById(string id)
    {
        try
        {
            var entity = await _context.Set<TEntity>().Find(id);
            return entity != null
                ? Envelope.Success(RepositoryCode.Found, entity)
                : Envelope.NotFound<RepositoryCode, RepositoryError>(
                    RepositoryError.EntityNotFound(typeof(TEntity).Name, id)
                );
        }
        catch (Exception ex)
        {
            return Envelope.Error<RepositoryCode, RepositoryError>(
                RepositoryError.DatabaseError(ex.Message)
            );
        }
    }
}

// Register all entity repositories automatically
services.AddZentientServices(builder =>
{
    builder
        .RegisterGeneric(typeof(IRepository<>), typeof(Repository<>), ServiceLifetime.Scoped)
        .Where(type => type.IsAssignableTo<IEntity>())
        .AddCaching()
        .AddValidation()
        .AddLogging();
});
```

## 🎭 Service Decoration Patterns

### **Automatic Decoration**

Apply cross-cutting concerns through decoration:

```csharp
// Service with automatic decorators
[ServiceRegistration(ServiceLifetime.Scoped)]
[DecorateWith(typeof(LoggingDecorator<>))]
[DecorateWith(typeof(ValidationDecorator<>))]
[DecorateWith(typeof(CachingDecorator<>))]
[DecorateWith(typeof(MetricsDecorator<>))]
public class OrderService : IOrderService
{
    // Core business logic only
    public async Task<IEnvelope<OrderCode, OrderError>> CreateOrder(CreateOrderRequest request)
    {
        // Implementation focuses on business logic
        // Logging, validation, caching, and metrics are handled by decorators
    }
}

// Logging decorator implementation
public class LoggingDecorator<T> : IDecorator<T> where T : class
{
    private readonly T _decorated;
    private readonly ILogger<T> _logger;
    
    public LoggingDecorator(T decorated, ILogger<T> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }
    
    public async Task<TResult> Intercept<TResult>(
        Func<Task<TResult>> operation,
        string methodName,
        object[] parameters)
    {
        using var scope = _logger.BeginScope(new { Method = methodName, Parameters = parameters });
        
        _logger.LogInformation("Executing {Method}", methodName);
        
        try
        {
            var result = await operation();
            _logger.LogInformation("Executed {Method} successfully", methodName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing {Method}", methodName);
            throw;
        }
    }
}
```

### **Manual Decoration Chain**

For fine-grained control over decoration order:

```csharp
services.AddZentientServices(builder =>
{
    builder
        .RegisterScoped<IOrderService, OrderService>()
        
        // Decoration chain (innermost to outermost)
        .DecorateWith<OrderServiceValidator>()     // 1. Validate inputs
        .DecorateWith<OrderServiceLogger>()        // 2. Log operations  
        .DecorateWith<OrderServiceCache>()         // 3. Cache results
        .DecorateWith<OrderServiceMetrics>()       // 4. Collect metrics
        .DecorateWith<OrderServiceRetry>()         // 5. Retry on failures
        .DecorateWith<OrderServiceCircuitBreaker>(); // 6. Circuit breaker
});

// Each decorator wraps the previous one:
// CircuitBreaker -> Retry -> Metrics -> Cache -> Logger -> Validator -> OrderService
```

## 🔍 Service Validation & Discovery

### **Dependency Validation**

Validate service dependencies at startup:

```csharp
services.AddZentientServices(builder =>
{
    builder
        .ScanAssembly(typeof(UserService).Assembly)
        .RegisterServicesWithAttribute<ServiceRegistrationAttribute>()
        
        // Comprehensive validation
        .ValidateServicesCanBeResolved()
        .ValidateNoCircularDependencies()
        .ValidateRequiredInterfaces()
        .ValidateServiceLifetimes()
        .ValidateConfiguration()
        
        // Throw on validation errors
        .ThrowOnValidationFailure();
});

// Validation report
public class ServiceValidationReport
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; }
    public List<ValidationWarning> Warnings { get; set; }
    
    public class ValidationError
    {
        public string ServiceType { get; set; }
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public string[] Suggestions { get; set; }
    }
}
```

### **Runtime Service Discovery**

Discover and analyze services at runtime:

```csharp
public class ServiceDiscoveryController : ControllerBase
{
    private readonly IServiceDiscovery _discovery;
    
    [HttpGet("services")]
    public IActionResult GetServices()
    {
        var services = _discovery.GetAllServices()
            .Select(service => new
            {
                service.Definition.Id,
                service.Definition.Name,
                service.Definition.Description,
                Type = service.Implementation.GetType().Name,
                Lifetime = service.Lifetime.ToString(),
                Dependencies = service.Dependencies.Select(d => d.Name),
                Decorators = service.Decorators.Select(d => d.GetType().Name),
                Metadata = service.Definition.Metadata
            });
            
        return Ok(services);
    }
    
    [HttpGet("services/{serviceId}/dependencies")]
    public IActionResult GetDependencyGraph(string serviceId)
    {
        var graph = _discovery.BuildDependencyGraph(serviceId);
        return Ok(graph);
    }
    
    [HttpGet("services/health")]
    public async Task<IActionResult> GetServiceHealth()
    {
        var healthChecks = await _discovery.RunHealthChecks();
        return Ok(healthChecks);
    }
}
```

## 🌐 Advanced Composition Patterns

### **Conditional Registration**

Register services based on runtime conditions:

```csharp
services.AddZentientServices(builder =>
{
    // Environment-based registration
    builder
        .When(env => env.IsDevelopment())
        .RegisterSingleton<IEmailService, MockEmailService>()
        .RegisterSingleton<IPaymentService, MockPaymentService>()
        .RegisterSingleton<ISmsService, MockSmsService>();
        
    builder
        .When(env => env.IsProduction())
        .RegisterSingleton<IEmailService, SendGridEmailService>()
        .RegisterSingleton<IPaymentService, StripePaymentService>()
        .RegisterSingleton<ISmsService, TwilioSmsService>();
        
    // Feature flag registration
    builder
        .When(config => config.GetValue<bool>("Features:AdvancedCaching"))
        .RegisterSingleton<ICacheService, RedisCacheService>()
        .Otherwise()
        .RegisterSingleton<ICacheService, MemoryCacheService>();
        
    // Configuration-based registration
    builder
        .When(config => config["Database:Provider"] == "PostgreSQL")
        .RegisterScoped<IUserRepository, PostgreSqlUserRepository>()
        .When(config => config["Database:Provider"] == "SqlServer")
        .RegisterScoped<IUserRepository, SqlServerUserRepository>()
        .When(config => config["Database:Provider"] == "InMemory")
        .RegisterScoped<IUserRepository, InMemoryUserRepository>();
});
```

### **Profile-Based Configuration**

Organize registration into profiles:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddZentientServices(builder =>
    {
        // Development profile
        builder
            .Profile("Development")
            .RegisterSingleton<IEmailService, MockEmailService>()
            .RegisterSingleton<IPaymentService, MockPaymentService>()
            .AddDetailedLogging()
            .AddDevelopmentTools()
            .EnableHotReload();
            
        // Testing profile
        builder
            .Profile("Testing")
            .RegisterSingleton<IEmailService, TestEmailService>()
            .RegisterSingleton<IPaymentService, TestPaymentService>()
            .AddTestingHelpers()
            .AddMockServices()
            .EnableTestingDashboard();
            
        // Production profile
        builder
            .Profile("Production")
            .RegisterSingleton<IEmailService, SendGridEmailService>()
            .RegisterSingleton<IPaymentService, StripePaymentService>()
            .AddProductionOptimizations()
            .AddSecurityHardening()
            .AddMonitoring()
            .AddAlerting();
            
        // Common services across all profiles
        builder
            .AllProfiles()
            .RegisterScoped<IUserService, UserService>()
            .RegisterScoped<IOrderService, OrderService>()
            .AddValidation()
            .AddCaching()
            .AddHealthChecks();
    });
}
```

### **Multi-Tenant Service Registration**

Support multi-tenant applications:

```csharp
services.AddZentientServices(builder =>
{
    // Shared services (single instance across all tenants)
    builder
        .RegisterSingleton<IConfigurationService, ConfigurationService>()
        .RegisterSingleton<IAuthenticationService, AuthenticationService>();
        
    // Tenant-scoped services (separate instance per tenant)
    builder
        .RegisterTenantScoped<IUserService, UserService>()
        .RegisterTenantScoped<IOrderService, OrderService>()
        .RegisterTenantScoped<IPaymentService, PaymentService>();
        
    // Tenant-specific implementations
    builder
        .RegisterTenantSpecific<IDatabaseContext>((tenant, provider) =>
        {
            var connectionString = provider
                .GetRequiredService<IConfiguration>()
                .GetConnectionString($"Tenant_{tenant.Id}");
                
            return new DatabaseContext(connectionString);
        });
});

// Usage in controllers
[ApiController]
public class UserController : ControllerBase
{
    private readonly ITenantService<IUserService> _userService;
    
    public UserController(ITenantService<IUserService> userService)
    {
        _userService = userService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        // Automatically uses the correct tenant's user service
        var result = await _userService.Current.GetUser(id);
        return result.ToActionResult();
    }
}
```

### **Factory Pattern Integration**

Integrate factory patterns with DI:

```csharp
// Factory interface
public interface IServiceFactory<T>
{
    T Create(string name);
    T Create<TImplementation>() where TImplementation : class, T;
    T Create(Type implementationType);
}

// Factory registration
services.AddZentientServices(builder =>
{
    // Register factory
    builder.RegisterSingleton<IServiceFactory<IPaymentProcessor>, PaymentProcessorFactory>();
    
    // Register implementations that factory can create
    builder
        .RegisterTransient<StripePaymentProcessor>()
        .RegisterTransient<PayPalPaymentProcessor>()
        .RegisterTransient<ApplePayPaymentProcessor>();
});

// Factory implementation
[ServiceRegistration(ServiceLifetime.Singleton)]
public class PaymentProcessorFactory : IServiceFactory<IPaymentProcessor>
{
    private readonly IServiceProvider _serviceProvider;
    
    public PaymentProcessorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IPaymentProcessor Create(string name)
    {
        return name.ToLower() switch
        {
            "stripe" => _serviceProvider.GetRequiredService<StripePaymentProcessor>(),
            "paypal" => _serviceProvider.GetRequiredService<PayPalPaymentProcessor>(),
            "applepay" => _serviceProvider.GetRequiredService<ApplePayPaymentProcessor>(),
            _ => throw new ArgumentException($"Unknown payment processor: {name}")
        };
    }
}

// Usage
public class OrderService : IOrderService
{
    private readonly IServiceFactory<IPaymentProcessor> _paymentFactory;
    
    public async Task<IEnvelope<OrderCode, OrderError>> ProcessPayment(
        ProcessPaymentRequest request)
    {
        var processor = _paymentFactory.Create(request.PaymentMethod);
        return await processor.Process(request);
    }
}
```

## 📊 Performance Optimization

### **Service Resolution Optimization**

Optimize service resolution for high-performance scenarios:

```csharp
services.AddZentientServices(builder =>
{
    builder
        // Pre-compile service resolution
        .PreCompileServices()
        
        // Use singleton pattern for expensive services
        .RegisterSingleton<IExpensiveService, ExpensiveService>()
        
        // Pool expensive objects
        .RegisterPooled<IConnectionManager, ConnectionManager>(poolSize: 10)
        
        // Use factory pattern for lightweight services
        .RegisterFactory<ILightweightService>(() => new LightweightService())
        
        // Optimize generic service resolution
        .OptimizeGenericServices()
        
        // Enable service resolution caching
        .EnableResolutionCaching();
});
```

### **Lazy Service Resolution**

Defer service creation until needed:

```csharp
public class OrderService : IOrderService
{
    private readonly Lazy<IPaymentService> _paymentService;
    private readonly Lazy<IInventoryService> _inventoryService;
    private readonly Lazy<IEmailService> _emailService;
    
    public OrderService(
        Lazy<IPaymentService> paymentService,
        Lazy<IInventoryService> inventoryService,
        Lazy<IEmailService> emailService)
    {
        _paymentService = paymentService;
        _inventoryService = inventoryService;
        _emailService = emailService;
    }
    
    public async Task<IEnvelope<OrderCode, OrderError>> CreateOrder(CreateOrderRequest request)
    {
        // Services are only created when accessed
        var inventoryResult = await _inventoryService.Value.CheckAvailability(request.Items);
        
        if (inventoryResult.IsSuccess)
        {
            var paymentResult = await _paymentService.Value.ProcessPayment(request.Payment);
            
            if (paymentResult.IsSuccess)
            {
                // Email service only created if order is successful
                await _emailService.Value.SendOrderConfirmation(request.CustomerId);
            }
        }
        
        return result;
    }
}
```

---

**Zentient's dependency injection architecture transforms service registration from boilerplate code into a powerful architectural tool that promotes good design, enables rich composition patterns, and provides comprehensive runtime insights.**
