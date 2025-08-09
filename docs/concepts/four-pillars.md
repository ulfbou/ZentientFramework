# 🏛️ The Four Pillars of Zentient.Abstractions

## Overview

Zentient.Abstractions is built upon four foundational pillars that work together to create a cohesive, powerful framework for building .NET applications. Each pillar addresses a fundamental aspect of modern software development, and their synergistic combination creates an exceptional developer experience.

## 🧩 Pillar 1: Definition-Centric Architecture

### **Core Concept**

Every component in the system is **self-describing** through rich type definitions and metadata. This creates an architecture that is discoverable, documented, and validatable at both design-time and runtime.

### **The Problem It Solves**

Traditional architectures often suffer from:
- **Implicit Dependencies**: Services depend on things that aren't clearly declared
- **Poor Discoverability**: Developers can't easily understand what services exist or how they work
- **Outdated Documentation**: Documentation becomes stale and unreliable
- **Integration Challenges**: Services don't provide enough information for tools and other services

### **How Zentient Solves It**

Through **Type Definitions**, every component declares:
- What it is and what it does
- What it depends on
- What capabilities it provides
- How it should be configured
- What guarantees it makes

```csharp
[ServiceDefinition("OrderProcessing", Version = "3.1.0")]
public record OrderServiceDefinition : IServiceDefinition
{
    public string Id => "OrderService.v3.1";
    public string Name => "Order Processing Service";
    public string Description => "Handles order lifecycle from creation to fulfillment";
    
    public IMetadata Metadata => new MetadataCollection
    {
        // Service ownership and governance
        ["Owner"] = "E-commerce Team",
        ["Maintainer"] = "orders-team@company.com",
        ["Repository"] = "https://github.com/company/order-service",
        
        // Service level agreements
        ["SLA.Availability"] = "99.95%",
        ["SLA.ResponseTime.P95"] = "150ms",
        ["SLA.Throughput"] = "10000 orders/minute",
        
        // Dependencies and integrations
        ["Dependencies"] = new[]
        {
            "PaymentService.v2+",
            "InventoryService.v1.5+",
            "CustomerService.v4+",
            "Database.PostgreSQL.v13+",
            "MessageBus.RabbitMQ.v3.8+"
        },
        
        // Capabilities and features
        ["Capabilities"] = new[]
        {
            "OrderCreation",
            "OrderModification",
            "OrderCancellation",
            "OrderTracking",
            "OrderReporting"
        },
        
        // Security and compliance
        ["SecurityLevel"] = "High",
        ["DataClassification"] = "Confidential",
        ["ComplianceRequirements"] = new[] { "PCI-DSS", "SOX" },
        
        // Operational characteristics
        ["Deployment.Strategy"] = "Blue-Green",
        ["Scaling.Min"] = 3,
        ["Scaling.Max"] = 20,
        ["Scaling.Target.CPU"] = "70%",
        ["HealthCheck.Endpoint"] = "/health",
        ["Metrics.Endpoint"] = "/metrics"
    };
}
```

### **Benefits**

#### **🔍 Automatic Discovery**
Tools can automatically discover and catalog all services, their dependencies, and capabilities:

```csharp
// Generate service catalog
var services = ServiceDiscovery.FindAllServices()
    .Where(s => s.Definition.Metadata.ContainsKey("Owner"))
    .GroupBy(s => s.Definition.Metadata["Owner"])
    .ToList();

// Generate dependency graph
var dependencies = ServiceDiscovery.BuildDependencyGraph();
```

#### **📚 Living Documentation**
Documentation is generated directly from the definitions and stays current:

```csharp
// Auto-generated API documentation
[HttpGet]
public IActionResult GetServiceCatalog()
{
    return Ok(ServiceRegistry.GetAllDefinitions().Select(def => new
    {
        def.Id,
        def.Name,
        def.Description,
        Version = def.Metadata["Version"],
        Owner = def.Metadata["Owner"],
        SLA = def.Metadata.Where(m => m.Key.StartsWith("SLA."))
    }));
}
```

#### **🛡️ Design-Time Validation**
Dependencies and contracts can be validated at build time:

```csharp
// Build-time dependency validation
[assembly: DependencyValidation]

// This will fail at build time if PaymentService.v3+ isn't available
[ServiceDependency("PaymentService", MinVersion = "3.0")]
public class OrderService : IOrderService { }
```

### **Real-World Example**

```csharp
// Service definition with complete metadata
[ServiceDefinition("UserAuthentication", Version = "2.3.1")]
public record AuthServiceDefinition : IServiceDefinition
{
    public string Id => "AuthService.v2.3.1";
    public string Name => "User Authentication Service";
    public string Description => "Handles user authentication, authorization, and session management";
    
    public IMetadata Metadata => new MetadataCollection
    {
        ["Owner"] = "Security Team",
        ["BusinessCriticality"] = "Critical",
        ["SLA.Availability"] = "99.99%",
        ["Dependencies"] = new[] { "UserService.v2+", "TokenService.v1+", "AuditService.v1+" },
        ["Capabilities"] = new[] { "Login", "Logout", "TokenValidation", "PasswordReset" },
        ["SecurityFeatures"] = new[] { "MFA", "RiskAssessment", "DeviceFingerprinting" }
    };
}

// Implementation references the definition
[ServiceRegistration(ServiceLifetime.Scoped)]
public class AuthService : IAuthService
{
    public IServiceDefinition Definition => new AuthServiceDefinition();
    
    // Service automatically inherits metadata for monitoring, health checks, etc.
}
```

---

## ✉️ Pillar 2: Universal Envelope Pattern

### **Core Concept**

All operations return **envelopes** that provide consistent structure for success and failure cases, rich error information, and contextual metadata. This creates predictable error handling and enables powerful composition patterns.

### **The Problem It Solves**

Traditional error handling approaches have significant issues:
- **Inconsistent Patterns**: Some methods throw exceptions, others return nulls, others return booleans
- **Lost Context**: Exception messages often lack the context needed for debugging
- **Poor Composability**: Different error handling patterns don't compose well
- **Difficult Monitoring**: Hard to track and correlate errors across operations

### **How Zentient Solves It**

The **Universal Envelope** provides a consistent structure for all operation results:

```csharp
public interface IEnvelope<out TCode, out TError>
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    TCode Code { get; }
    IReadOnlyCollection<TError> Errors { get; }
    DateTime Timestamp { get; }
    IMetadata Metadata { get; }
}

public interface IEnvelope<out TCode, out TError, out TValue> : IEnvelope<TCode, TError>
{
    TValue Value { get; }
    bool HasValue { get; }
}
```

### **Envelope Types and Usage**

#### **Success Envelopes**
```csharp
public async Task<IEnvelope<UserCode, UserError>> CreateUserAsync(CreateUserRequest request)
{
    var user = await _repository.CreateAsync(request);
    
    return Envelope.Success(UserCode.UserCreated, user, metadata: new MetadataCollection
    {
        ["CreatedAt"] = DateTime.UtcNow,
        ["CreatedBy"] = _currentUser.Id,
        ["Source"] = "WebAPI",
        ["RequestId"] = request.CorrelationId
    });
}
```

#### **Validation Error Envelopes**
```csharp
public async Task<IEnvelope<UserCode, UserError>> CreateUserAsync(CreateUserRequest request)
{
    var validationResult = await _validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Envelope.ValidationError<UserCode, UserError>(
            validationResult.Errors.Select(e => UserError.ValidationFailed(e.PropertyName, e.ErrorMessage))
        );
    }
    
    // Continue with creation...
}
```

#### **Business Error Envelopes**
```csharp
public async Task<IEnvelope<OrderCode, OrderError>> ProcessPaymentAsync(ProcessPaymentRequest request)
{
    try
    {
        var payment = await _paymentService.ProcessAsync(request);
        return Envelope.Success(OrderCode.PaymentProcessed, payment);
    }
    catch (InsufficientFundsException ex)
    {
        return Envelope.BusinessError<OrderCode, OrderError>(
            OrderError.InsufficientFunds(request.Amount, ex.AvailableAmount, new MetadataCollection
            {
                ["AccountId"] = request.AccountId,
                ["RequestedAmount"] = request.Amount,
                ["AvailableAmount"] = ex.AvailableAmount,
                ["Currency"] = request.Currency
            })
        );
    }
}
```

#### **Not Found Envelopes**
```csharp
public async Task<IEnvelope<UserCode, UserError>> GetUserAsync(string userId)
{
    var user = await _repository.GetByIdAsync(userId);
    
    return user != null
        ? Envelope.Success(UserCode.UserFound, user)
        : Envelope.NotFound<UserCode, UserError>(
            UserError.UserNotFound(userId, new MetadataCollection
            {
                ["SearchedAt"] = DateTime.UtcNow,
                ["SearchMethod"] = "ById",
                ["UserId"] = userId
            })
        );
}
```

### **Envelope Composition**

Envelopes can be composed and transformed, enabling powerful functional programming patterns:

```csharp
public async Task<IEnvelope<OrderCode, OrderError>> CreateOrderAsync(CreateOrderRequest request)
{
    return await ValidateCustomerAsync(request.CustomerId)
        .ContinueWith(customerResult =>
            customerResult.IsSuccess
                ? ValidateInventoryAsync(request.Items)
                : Task.FromResult(customerResult.MapError<OrderCode, OrderError>())
        )
        .ContinueWith(inventoryResult =>
            inventoryResult.Result.IsSuccess
                ? ProcessOrderAsync(request)
                : Task.FromResult(inventoryResult.Result)
        );
}

// Functional composition with monadic operations
public async Task<IEnvelope<OrderCode, OrderError>> CreateOrderFunctionalAsync(CreateOrderRequest request)
{
    return await ValidateCustomerAsync(request.CustomerId)
        .BindAsync(customer => ValidateInventoryAsync(request.Items))
        .BindAsync(inventory => CalculatePricingAsync(request))
        .BindAsync(pricing => ProcessPaymentAsync(request))
        .BindAsync(payment => CreateOrderRecordAsync(request))
        .MapAsync(order => EnrichOrderDataAsync(order));
}
```

### **Rich Error Information**

Envelopes provide structured error information that's perfect for logging, monitoring, and user interfaces:

```csharp
public record UserError : IError
{
    public string Code { get; init; }
    public string Message { get; init; }
    public IMetadata Metadata { get; init; } = new MetadataCollection();
    
    public static UserError ValidationFailed(string field, string message) => new()
    {
        Code = "USER_VALIDATION_FAILED",
        Message = $"Validation failed for {field}: {message}",
        Metadata = new MetadataCollection
        {
            ["Field"] = field,
            ["ValidationMessage"] = message,
            ["ErrorType"] = "Validation"
        }
    };
    
    public static UserError EmailAlreadyExists(string email) => new()
    {
        Code = "USER_EMAIL_EXISTS",
        Message = $"A user with email {email} already exists",
        Metadata = new MetadataCollection
        {
            ["Email"] = email,
            ["ErrorType"] = "BusinessRule",
            ["Suggestion"] = "Try logging in or use password reset"
        }
    };
    
    public static UserError DatabaseError(string operation, Exception ex) => new()
    {
        Code = "USER_DATABASE_ERROR",
        Message = $"Database operation failed: {operation}",
        Metadata = new MetadataCollection
        {
            ["Operation"] = operation,
            ["ExceptionType"] = ex.GetType().Name,
            ["ExceptionMessage"] = ex.Message,
            ["StackTrace"] = ex.StackTrace,
            ["ErrorType"] = "Infrastructure"
        }
    };
}
```

---

## 🏗️ Pillar 3: Fluent Dependency Injection

### **Core Concept**

Dependency injection becomes a **first-class architectural tool** through fluent APIs, automatic discovery, validation, and rich composition capabilities. Services are not just registered; they are composed into a coherent application architecture.

### **The Problem It Solves**

Traditional DI containers have limitations:
- **Registration Boilerplate**: Lots of repetitive registration code
- **Poor Discoverability**: Hard to understand what services are registered
- **Weak Validation**: Missing dependencies only discovered at runtime
- **Limited Composition**: Difficult to apply cross-cutting concerns consistently

### **How Zentient Solves It**

Through **Fluent DI**, service registration becomes declarative, discoverable, and composable:

#### **Attribute-Based Registration**
```csharp
// Simple registration
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    // Automatically registered as IUserService with Scoped lifetime
}

// Multiple interface registration
[ServiceRegistration(typeof(IUserService), typeof(IUserManager), ServiceLifetime.Scoped)]
public class UserService : IUserService, IUserManager
{
    // Registered for both interfaces
}

// Named registration
[ServiceRegistration(ServiceLifetime.Singleton, Name = "PrimaryCache")]
public class RedisCacheService : ICacheService
{
    // Named registration for multiple implementations
}

// Conditional registration
[ServiceRegistration(ServiceLifetime.Scoped, Condition = "Environment == 'Production'")]
public class ProductionUserService : IUserService
{
    // Only registered in production
}
```

#### **Fluent API Registration**
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
            .AddLogging()
            .AddMetrics()
            .AddHealthCheck();
            
        // Decorated service registration
        builder
            .RegisterScoped<IOrderService, OrderService>()
            .DecorateWith<OrderServiceLogger>()
            .DecorateWith<OrderServiceMetrics>()
            .DecorateWith<OrderServiceCache>()
            .DecorateWith<OrderServiceRetry>();
            
        // Factory registration
        builder
            .RegisterFactory<IEmailService>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                return config["EmailProvider"] switch
                {
                    "SendGrid" => new SendGridEmailService(config["SendGrid:ApiKey"]),
                    "SMTP" => new SmtpEmailService(config.GetConnectionString("SMTP")),
                    _ => new MockEmailService()
                };
            })
            .AsSingleton()
            .AddHealthCheck();
    });
}
```

#### **Automatic Discovery and Validation**
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddZentientServices(builder =>
    {
        // Automatic registration scanning
        builder
            .ScanAssembly(typeof(UserService).Assembly)
            .RegisterServicesWithAttribute<ServiceRegistrationAttribute>()
            .RegisterValidatorsWithAttribute<ValidatorAttribute>()
            .RegisterRepositoriesWithInterface<IRepository>()
            .ValidateDependencies()
            .ValidateConfiguration();
            
        // Dependency validation
        builder
            .ValidateServicesCanBeResolved()
            .ValidateNoCircularDependencies()
            .ValidateRequiredServices(
                typeof(ILogger<>),
                typeof(IConfiguration),
                typeof(IHttpContextAccessor)
            );
    });
}
```

### **Advanced Composition Patterns**

#### **Service Decoration**
```csharp
// Automatic decoration based on attributes
[ServiceRegistration(ServiceLifetime.Scoped)]
[DecorateWith(typeof(LoggingDecorator<>))]
[DecorateWith(typeof(MetricsDecorator<>))]
[DecorateWith(typeof(CachingDecorator<>))]
public class UserService : IUserService
{
    // Service is automatically wrapped with decorators
}

// Manual decoration chain
builder
    .RegisterScoped<IUserService, UserService>()
    .DecorateWith<UserServiceLogger>()      // Innermost decorator
    .DecorateWith<UserServiceCache>()       // Middle decorator  
    .DecorateWith<UserServiceMetrics>();    // Outermost decorator
```

#### **Conditional Registration**
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddZentientServices(builder =>
    {
        // Environment-based registration
        builder
            .When(env => env.IsDevelopment())
            .RegisterSingleton<IEmailService, MockEmailService>()
            .RegisterSingleton<IPaymentService, MockPaymentService>();
            
        builder
            .When(env => env.IsProduction())
            .RegisterSingleton<IEmailService, SendGridEmailService>()
            .RegisterSingleton<IPaymentService, StripePaymentService>();
            
        // Feature flag based registration
        builder
            .When(config => config.GetValue<bool>("Features:AdvancedCaching"))
            .RegisterSingleton<ICacheService, RedisCacheService>()
            .Otherwise()
            .RegisterSingleton<ICacheService, MemoryCacheService>();
    });
}
```

#### **Profile-Based Configuration**
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
            .AddLogging(LogLevel.Debug)
            .AddDetailedExceptions();
            
        // Production profile
        builder
            .Profile("Production")
            .RegisterSingleton<IEmailService, SendGridEmailService>()
            .RegisterSingleton<IPaymentService, StripePaymentService>()
            .AddLogging(LogLevel.Information)
            .AddSecurityHeaders()
            .AddPerformanceOptimizations();
    });
}
```

---

## 🩺 Pillar 4: Built-in Observability

### **Core Concept**

Observability is **not an afterthought** but a fundamental capability built into every component. Logging, metrics, tracing, and health checks work together to provide complete system visibility with zero configuration.

### **The Problem It Solves**

Traditional observability approaches have significant gaps:
- **Fragmented Tools**: Logging, metrics, and tracing are often separate systems
- **Manual Instrumentation**: Developers must manually add observability code
- **Inconsistent Data**: Different components log different information
- **Poor Correlation**: Difficult to correlate events across system boundaries

### **How Zentient Solves It**

Through **Built-in Observability**, every operation is automatically observable:

#### **Automatic Structured Logging**
```csharp
[ServiceRegistration(ServiceLifetime.Scoped)]
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    
    public async Task<IEnvelope<OrderCode, OrderError>> CreateOrderAsync(CreateOrderRequest request)
    {
        // Automatic structured logging scope
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = request.CorrelationId,
            ["CustomerId"] = request.CustomerId,
            ["OrderType"] = request.OrderType,
            ["ItemCount"] = request.Items.Count,
            ["TotalAmount"] = request.TotalAmount
        });
        
        _logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);
        
        try
        {
            var order = await ProcessOrderAsync(request);
            
            // Automatic success logging with context
            _logger.LogInformation("Order {OrderId} created successfully", order.Id);
            
            return Envelope.Success(OrderCode.OrderCreated, order);
        }
        catch (Exception ex)
        {
            // Automatic error logging with full context
            _logger.LogError(ex, "Failed to create order for customer {CustomerId}", request.CustomerId);
            
            return Envelope.Error<OrderCode, OrderError>(
                OrderError.CreationFailed(ex.Message)
            );
        }
    }
}
```

#### **Automatic Metrics Collection**
```csharp
[ServiceRegistration(ServiceLifetime.Scoped)]
public class OrderService : IOrderService
{
    private readonly IMeter _meter;
    
    // Metrics are automatically registered and managed
    private readonly Counter<long> _ordersCreated = _meter.CreateCounter<long>(
        "orders_created_total", 
        "count", 
        "Total number of orders created"
    );
    
    private readonly Histogram<double> _orderProcessingTime = _meter.CreateHistogram<double>(
        "order_processing_duration", 
        "ms", 
        "Time taken to process orders"
    );
    
    private readonly Gauge<int> _pendingOrders = _meter.CreateGauge<int>(
        "pending_orders_count", 
        "count", 
        "Number of orders pending processing"
    );
    
    public async Task<IEnvelope<OrderCode, OrderError>> CreateOrderAsync(CreateOrderRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var order = await ProcessOrderAsync(request);
            
            // Automatic success metrics
            _ordersCreated.Add(1, new TagList
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
            
            return Envelope.Success(OrderCode.OrderCreated, order);
        }
        catch (Exception ex)
        {
            // Automatic error metrics
            _ordersCreated.Add(1, new TagList
            {
                ["customer_type"] = request.CustomerType,
                ["order_type"] = request.OrderType,
                ["success"] = "false",
                ["error_type"] = ex.GetType().Name
            });
            
            return Envelope.Error<OrderCode, OrderError>(
                OrderError.CreationFailed(ex.Message)
            );
        }
    }
}
```

#### **Automatic Distributed Tracing**
```csharp
[ServiceRegistration(ServiceLifetime.Scoped)]
public class OrderService : IOrderService
{
    private readonly ITracer<OrderService> _tracer;
    
    public async Task<IEnvelope<OrderCode, OrderError>> CreateOrderAsync(CreateOrderRequest request)
    {
        // Automatic trace creation and management
        using var activity = _tracer.StartActivity("CreateOrder");
        
        // Automatic context propagation
        activity?.SetTag("order.customer_id", request.CustomerId);
        activity?.SetTag("order.type", request.OrderType);
        activity?.SetTag("order.item_count", request.Items.Count);
        activity?.SetTag("order.total_amount", request.TotalAmount);
        
        try
        {
            // Child operations automatically inherit trace context
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Validation failed");
                activity?.SetTag("validation.error_count", validationResult.Errors.Count);
                
                return Envelope.ValidationError<OrderCode, OrderError>(
                    validationResult.Errors.Select(e => OrderError.ValidationFailed(e.Message))
                );
            }
            
            var order = await _repository.CreateAsync(request);
            
            // Automatic success tracking
            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("order.id", order.Id);
            activity?.SetTag("order.status", order.Status);
            
            return Envelope.Success(OrderCode.OrderCreated, order);
        }
        catch (Exception ex)
        {
            // Automatic error tracking
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);
            
            throw;
        }
    }
}
```

#### **Automatic Health Checks**
```csharp
[DiagnosticCheck("OrderService.Database")]
[ServiceRegistration(ServiceLifetime.Singleton)]
public class OrderDatabaseHealthCheck : IDiagnosticCheck<DbContext, HealthCode, HealthError>
{
    public string Id => "OrderService.Database.HealthCheck.v1";
    public string Name => "Order Database Health Check";
    public TimeSpan Timeout => TimeSpan.FromSeconds(30);
    public Priority Priority => Priority.Critical;
    
    public async Task<IDiagnosticReport<HealthCode, HealthError>> CheckHealthAsync(
        DbContext context,
        IDiagnosticContext diagnosticContext,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Test database connectivity
            await context.Database.CanConnectAsync(cancellationToken);
            
            // Test query performance
            var orderCount = await context.Set<Order>().CountAsync(cancellationToken);
            
            stopwatch.Stop();
            
            var metadata = new MetadataCollection
            {
                ["ResponseTime"] = stopwatch.ElapsedMilliseconds,
                ["OrderCount"] = orderCount,
                ["DatabaseProvider"] = context.Database.ProviderName,
                ["ConnectionString"] = context.Database.GetConnectionString()
            };
            
            if (stopwatch.ElapsedMilliseconds > 1000)
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
            
            return DiagnosticReport.Unhealthy(
                HealthError.DatabaseUnavailable(ex.Message),
                "Database connectivity failed",
                new MetadataCollection
                {
                    ["Exception"] = ex.GetType().Name,
                    ["ElapsedTime"] = stopwatch.ElapsedMilliseconds,
                    ["ErrorMessage"] = ex.Message
                }
            );
        }
    }
}
```

## 🔗 How the Pillars Work Together

The four pillars are designed to work synergistically:

### **Definition-Centric + Envelopes**
```csharp
[ServiceDefinition("Payment", Version = "2.0")]
public interface IPaymentService : IIdentifiable
{
    // Definitions describe what envelopes to expect
    Task<IEnvelope<PaymentCode, PaymentError>> ProcessPaymentAsync(PaymentRequest request);
}
```

### **Envelopes + Fluent DI**
```csharp
services.AddZentientServices(builder =>
{
    builder
        .RegisterScoped<IPaymentService, PaymentService>()
        .AddEnvelopeMapping() // Automatic HTTP status code mapping
        .AddEnvelopeLogging() // Automatic envelope content logging
        .AddEnvelopeMetrics(); // Automatic envelope outcome metrics
});
```

### **Fluent DI + Observability**
```csharp
services.AddZentientServices(builder =>
{
    builder
        .RegisterScoped<IOrderService, OrderService>()
        .AddLogging() // Automatic logger injection
        .AddMetrics() // Automatic meter injection
        .AddTracing() // Automatic tracer injection
        .AddHealthCheck(); // Automatic health check registration
});
```

### **All Pillars Together**
```csharp
[ServiceDefinition("UserManagement", Version = "3.0")]           // Definition-Centric
[ServiceRegistration(ServiceLifetime.Scoped)]                   // Fluent DI
[DiagnosticCheck("UserService.Database")]                       // Observability
public class UserService : IUserService
{
    public async Task<IEnvelope<UserCode, UserError>> CreateUserAsync(CreateUserRequest request) // Envelopes
    {
        // All four pillars working together automatically
        using var activity = _tracer.StartActivity("CreateUser");     // Observability
        using var scope = _logger.BeginScope(request.CorrelationId); // Observability
        
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Envelope.ValidationError<UserCode, UserError>(     // Envelopes
                validationResult.Errors.Select(e => UserError.ValidationFailed(e.Message))
            );
        }
        
        // Service behavior driven by definition metadata
        var definition = ServiceRegistry.GetDefinition<UserServiceDefinition>();
        var cacheTtl = definition.Metadata.GetValue<TimeSpan>("Cache.TTL");
        
        // Continue with user creation...
    }
}
```

---

**The Four Pillars create a cohesive framework where every component is self-describing, every operation returns consistent results, services are composed through powerful DI, and everything is observable by default. Together, they enable building complex systems that are simple to understand, debug, and maintain.**
