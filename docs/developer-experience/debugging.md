# 🐛 Debugging Guide for Zentient.Abstractions

## Overview

Zentient.Abstractions provides rich debugging capabilities that help developers quickly identify and resolve issues. This guide covers debugging techniques, common patterns, and troubleshooting strategies specific to the framework.

## 🔍 Debugging Framework Components

### **Service Resolution Debugging**

When services fail to resolve, Zentient provides detailed diagnostic information:

```csharp
// Enable detailed service resolution logging
services.AddZentientServices(builder =>
{
    builder
        .EnableDiagnostics()
        .AddServiceResolutionLogging(LogLevel.Debug)
        .ValidateDependencies(throwOnError: true);
});
```

**Common Service Resolution Issues:**

#### **Missing Registration**
```csharp
// Problem: Service not registered
public class OrderController : ControllerBase
{
    public OrderController(IOrderService orderService) // ❌ Throws at runtime
    {
    }
}

// Debug output:
// Unable to resolve service for type 'IOrderService' while attempting to activate 'OrderController'
// Available services: IUserService, IEmailService, IPaymentService
// Suggestion: Add [ServiceRegistration] attribute to OrderService or register manually

// Solution:
[ServiceRegistration(ServiceLifetime.Scoped)]
public class OrderService : IOrderService
{
}
```

#### **Circular Dependencies**
```csharp
// Problem: Circular dependency
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    public UserService(IOrderService orderService) { } // ❌ Circular dependency
}

[ServiceRegistration(ServiceLifetime.Scoped)]
public class OrderService : IOrderService
{
    public OrderService(IUserService userService) { } // ❌ Circular dependency
}

// Debug output:
// Circular dependency detected:
// UserService -> IOrderService -> OrderService -> IUserService -> UserService
// 
// Suggestions:
// 1. Extract shared logic into a separate service
// 2. Use events for loose coupling
// 3. Refactor to eliminate the circular dependency

// Solution: Extract shared logic
[ServiceRegistration(ServiceLifetime.Scoped)]
public class SharedDataService : ISharedDataService
{
    // Shared logic here
}

[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    public UserService(ISharedDataService sharedDataService) { } // ✅
}

[ServiceRegistration(ServiceLifetime.Scoped)]
public class OrderService : IOrderService
{
    public OrderService(ISharedDataService sharedDataService) { } // ✅
}
```

### **Envelope Debugging**

Rich debugging support for envelope operations:

```csharp
// Enable envelope debugging
public async Task<IEnvelope<UserCode, UserError>> CreateUserAsync(CreateUserRequest request)
{
    var envelope = await _userService.CreateUserAsync(request);
    
    // Debug envelope content in debugger
    // Envelope visualizer shows:
    // ✅ IsSuccess: false
    // 🔴 Errors: [
    //     {
    //       Code: "USER_EMAIL_EXISTS",
    //       Message: "A user with email john@example.com already exists",
    //       Metadata: {
    //         "Email": "john@example.com",
    //         "ExistingUserId": "user-123",
    //         "Timestamp": "2024-08-09T14:30:00Z"
    //       }
    //     }
    //   ]
    // 📊 Metadata: {
    //     "RequestId": "req-456",
    //     "Duration": "00:00:00.1234567",
    //     "Source": "UserService.CreateUserAsync"
    //   }
    
    return envelope;
}
```

#### **Envelope Debugging Extensions**
```csharp
public static class EnvelopeDebuggingExtensions
{
    public static IEnvelope<TCode, TError> Debug<TCode, TError>(
        this IEnvelope<TCode, TError> envelope,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string callerFile = null,
        [CallerLineNumber] int callerLine = 0)
    {
        if (envelope.IsFailure)
        {
            var debugInfo = new
            {
                Caller = $"{Path.GetFileName(callerFile)}:{callerName}:{callerLine}",
                Errors = envelope.Errors,
                Timestamp = envelope.Timestamp,
                Metadata = envelope.Metadata
            };
            
            // Set breakpoint here or log for debugging
            System.Diagnostics.Debug.WriteLine($"Envelope Error: {JsonSerializer.Serialize(debugInfo, new JsonSerializerOptions { WriteIndented = true })}");
        }
        
        return envelope;
    }
}

// Usage
var result = await _userService.CreateUserAsync(request)
    .Debug(); // Automatically logs detailed error information
```

### **Validation Debugging**

Comprehensive validation debugging support:

```csharp
[Validator(typeof(CreateUserRequest))]
public class CreateUserRequestValidator : IValidator<CreateUserRequest>
{
    public async Task<IValidationResult> ValidateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var errors = new List<IValidationError>();
        
        // Enable validation debugging
        using var validationScope = ValidationScope.Begin(request);
        
        // Email validation with debugging
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            var error = ValidationError.Required(nameof(request.Email));
            validationScope.AddError(error); // Automatically captured in debugger
            errors.Add(error);
        }
        else if (!IsValidEmail(request.Email))
        {
            var error = ValidationError.Format(nameof(request.Email), "Invalid email format");
            validationScope.AddError(error);
            errors.Add(error);
        }
        
        // Database validation with debugging
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            var error = ValidationError.Duplicate(nameof(request.Email), "Email already exists");
            error.Metadata["ExistingUserId"] = existingUser.Id;
            error.Metadata["ExistingUserCreated"] = existingUser.CreatedAt;
            validationScope.AddError(error);
            errors.Add(error);
        }
        
        // Validation scope provides rich debugging information:
        // - All validation steps executed
        // - Time taken for each validation
        // - Database queries performed
        // - External service calls made
        
        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            Metadata = validationScope.GetMetadata()
        };
    }
}
```

### **Cache Debugging**

Debug caching behavior and performance:

```csharp
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    private readonly ICache<UserDto> _cache;
    private readonly ICacheDebugger _cacheDebugger; // Injected debugging helper
    
    public async Task<IEnvelope<UserCode, UserError>> GetUserAsync(string userId)
    {
        var cacheKey = new UserCacheKey(userId);
        
        // Debug cache operations
        using var cacheScope = _cacheDebugger.BeginScope("GetUser", cacheKey);
        
        var cached = await _cache.GetAsync(cacheKey);
        if (cached.HasValue)
        {
            cacheScope.RecordHit(cached.Value);
            // Debugger shows:
            // ✅ Cache HIT for key: User:12345
            // 📊 Value: { Id: "12345", Name: "John Doe", ... }
            // ⏱️ Retrieval time: 2ms
            // 📅 Cached at: 2024-08-09T14:25:00Z
            // ⌛ TTL remaining: 00:25:15
            
            return Envelope.Success(UserCode.UserFound, cached.Value);
        }
        
        // Cache miss - load from repository
        cacheScope.RecordMiss();
        // Debugger shows:
        // ❌ Cache MISS for key: User:12345
        // 💡 Reason: Key not found
        // 🔄 Loading from source...
        
        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            return Envelope.NotFound<UserCode, UserError>(UserError.UserNotFound(userId));
        }
        
        // Cache the result with debugging
        await _cache.SetAsync(cacheKey, user, TimeSpan.FromHours(1));
        cacheScope.RecordSet(user);
        // Debugger shows:
        // ✅ Cached value for key: User:12345
        // 📊 Value: { Id: "12345", Name: "John Doe", ... }
        // ⌛ TTL: 01:00:00
        // 💾 Size: 1.2KB
        
        return Envelope.Success(UserCode.UserFound, user);
    }
}
```

## 🔧 Debugging Tools & Techniques

### **Zentient Debug Dashboard**

Enable the built-in debug dashboard for real-time insights:

```csharp
// Startup.cs / Program.cs
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseZentientDebugDashboard(); // Accessible at /zentient-debug
    }
}
```

The dashboard provides:
- **Service Registry**: All registered services with their configurations
- **Health Checks**: Real-time health status of all components
- **Metrics**: Live performance metrics and counters
- **Cache Status**: Cache hit rates, memory usage, and key distribution
- **Validation Results**: Recent validation failures with detailed context
- **Envelope Analytics**: Success/failure rates and error patterns

### **Conditional Compilation Debugging**

Use conditional compilation for debug-only features:

```csharp
public class UserService : IUserService
{
    public async Task<IEnvelope<UserCode, UserError>> CreateUserAsync(CreateUserRequest request)
    {
#if DEBUG
        // Debug-only validation
        if (request.Email?.Contains("@debug.com") == true)
        {
            return Envelope.Success(UserCode.UserCreated, new User
            {
                Id = "debug-user",
                Email = request.Email,
                Name = "Debug User"
            });
        }
        
        // Debug-only delays
        if (request.Email?.Contains("@slow.com") == true)
        {
            await Task.Delay(5000); // Simulate slow operation
        }
#endif
        
        // Normal implementation
        return await CreateUserInternalAsync(request);
    }
}
```

### **Structured Logging for Debugging**

Leverage structured logging for effective debugging:

```csharp
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    
    public async Task<IEnvelope<OrderCode, OrderError>> ProcessOrderAsync(ProcessOrderRequest request)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = request.CorrelationId,
            ["CustomerId"] = request.CustomerId,
            ["OrderType"] = request.OrderType,
            ["ItemCount"] = request.Items.Count,
            ["TotalAmount"] = request.TotalAmount
        });
        
        _logger.LogDebug("Starting order processing for customer {CustomerId}", request.CustomerId);
        
        // Validation step
        _logger.LogDebug("Validating order request");
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Order validation failed: {Errors}", 
                string.Join(", ", validationResult.Errors.Select(e => e.Message)));
            
            return Envelope.ValidationError<OrderCode, OrderError>(
                validationResult.Errors.Select(e => OrderError.ValidationFailed(e.Message))
            );
        }
        _logger.LogDebug("Order validation successful");
        
        // Inventory check
        _logger.LogDebug("Checking inventory for {ItemCount} items", request.Items.Count);
        var inventoryResult = await _inventoryService.CheckAvailabilityAsync(request.Items);
        if (inventoryResult.IsFailure)
        {
            _logger.LogWarning("Inventory check failed: {Errors}", 
                string.Join(", ", inventoryResult.Errors.Select(e => e.Message)));
            
            return inventoryResult.MapError<OrderCode, OrderError>();
        }
        _logger.LogDebug("Inventory check successful");
        
        // Payment processing
        _logger.LogDebug("Processing payment of {Amount}", request.TotalAmount);
        var paymentResult = await _paymentService.ProcessPaymentAsync(request.PaymentInfo);
        if (paymentResult.IsFailure)
        {
            _logger.LogError("Payment processing failed: {Errors}", 
                string.Join(", ", paymentResult.Errors.Select(e => e.Message)));
            
            return paymentResult.MapError<OrderCode, OrderError>();
        }
        _logger.LogInformation("Payment processed successfully: {TransactionId}", paymentResult.Value.TransactionId);
        
        // Order creation
        _logger.LogDebug("Creating order record");
        var order = await _repository.CreateOrderAsync(request, paymentResult.Value);
        _logger.LogInformation("Order {OrderId} created successfully", order.Id);
        
        return Envelope.Success(OrderCode.OrderProcessed, order);
    }
}
```

## 🚨 Common Issues & Solutions

### **Issue: Service Not Found**

**Symptoms:**
```
InvalidOperationException: Unable to resolve service for type 'IMyService'
```

**Debug Steps:**
1. Check if service is registered:
```csharp
// In debug dashboard or startup logging
var registeredServices = serviceProvider.GetService<IServiceCollection>();
var myServiceRegistrations = registeredServices
    .Where(s => s.ServiceType == typeof(IMyService))
    .ToList();

if (!myServiceRegistrations.Any())
{
    // Service not registered
}
```

2. Check assembly scanning:
```csharp
services.AddZentientServices(builder =>
{
    builder
        .ScanAssembly(typeof(MyService).Assembly) // Ensure correct assembly
        .EnableDiagnostics() // Show what was found
        .LogRegistrations(); // Log all registrations
});
```

**Solution:**
```csharp
[ServiceRegistration(ServiceLifetime.Scoped)] // Add missing attribute
public class MyService : IMyService
{
}
```

### **Issue: Envelope Always Returns Errors**

**Symptoms:**
```csharp
var result = await service.DoSomethingAsync();
// result.IsFailure is always true
```

**Debug Steps:**
1. Add envelope debugging:
```csharp
var result = await service.DoSomethingAsync()
    .Debug(); // Logs detailed error information
```

2. Check validation logic:
```csharp
// Add breakpoints in validator
public async Task<IValidationResult> ValidateAsync(MyRequest request)
{
    var errors = new List<IValidationError>();
    
    if (string.IsNullOrEmpty(request.Name)) // Check this condition
    {
        errors.Add(ValidationError.Required(nameof(request.Name)));
    }
    
    return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
}
```

**Solution:**
Fix validation logic or request data.

### **Issue: Cache Not Working**

**Symptoms:**
```csharp
// Cache always misses, even for repeated requests
```

**Debug Steps:**
1. Enable cache debugging:
```csharp
services.AddZentientServices(builder =>
{
    builder
        .AddCaching()
        .EnableCacheDebugging() // Shows cache operations
        .LogCacheOperations();
});
```

2. Check cache key implementation:
```csharp
public record MyCacheKey : ICacheKey<MyData>
{
    public string UserId { get; }
    
    // Ensure ToString() is implemented correctly
    public override string ToString() => $"MyData:{UserId}";
    
    // Ensure equality is implemented correctly for record types
}
```

**Solution:**
Fix cache key implementation or cache configuration.

### **Issue: Performance Problems**

**Symptoms:**
```csharp
// Operations are slower than expected
```

**Debug Steps:**
1. Enable performance profiling:
```csharp
services.AddZentientServices(builder =>
{
    builder
        .AddPerformanceProfiling()
        .AddDetailedMetrics()
        .LogSlowOperations(threshold: TimeSpan.FromMilliseconds(100));
});
```

2. Use performance scopes:
```csharp
public async Task<IEnvelope<MyCode, MyError>> MyOperationAsync()
{
    using var perfScope = _profiler.BeginScope("MyOperation");
    
    // Measure each step
    using (perfScope.BeginStep("Validation"))
    {
        await ValidateAsync();
    }
    
    using (perfScope.BeginStep("DatabaseQuery"))
    {
        await QueryDatabaseAsync();
    }
    
    using (perfScope.BeginStep("Processing"))
    {
        await ProcessDataAsync();
    }
    
    // Performance report automatically generated
    return result;
}
```

**Solution:**
Optimize bottlenecks identified by profiling.

## 🎯 Best Practices for Debugging

### **1. Use Structured Logging**
```csharp
// ✅ Good - Structured logging
_logger.LogInformation("Processing order {OrderId} for customer {CustomerId}", 
    order.Id, customer.Id);

// ❌ Bad - String concatenation
_logger.LogInformation($"Processing order {order.Id} for customer {customer.Id}");
```

### **2. Include Context in Envelopes**
```csharp
// ✅ Good - Rich context
return Envelope.Error<OrderCode, OrderError>(
    OrderError.ProcessingFailed("Payment declined", new MetadataCollection
    {
        ["PaymentMethod"] = request.PaymentMethod,
        ["Amount"] = request.Amount,
        ["Currency"] = request.Currency,
        ["TransactionId"] = transactionId,
        ["Timestamp"] = DateTime.UtcNow
    })
);

// ❌ Bad - Minimal context
return Envelope.Error<OrderCode, OrderError>(
    OrderError.ProcessingFailed("Payment failed")
);
```

### **3. Use Correlation IDs**
```csharp
public async Task<IEnvelope<OrderCode, OrderError>> ProcessOrderAsync(ProcessOrderRequest request)
{
    // Ensure correlation ID flows through all operations
    using var scope = _logger.BeginScope(new { CorrelationId = request.CorrelationId });
    
    var inventoryResult = await _inventoryService.CheckAvailabilityAsync(request.Items, request.CorrelationId);
    var paymentResult = await _paymentService.ProcessPaymentAsync(request.Payment, request.CorrelationId);
    
    // All operations can be correlated in logs
}
```

### **4. Enable Debug Visualizers**
```csharp
// Configure debug visualizers for complex types
[DebuggerDisplay("Envelope: {IsSuccess ? \"Success\" : \"Failed\"} - {Code}")]
public class Envelope<TCode, TError> : IEnvelope<TCode, TError>
{
    // Implementation
}

[DebuggerDisplay("ValidationResult: {IsValid ? \"Valid\" : \"Invalid\"} - {Errors.Count} errors")]
public class ValidationResult : IValidationResult
{
    // Implementation
}
```

---

**Effective debugging with Zentient.Abstractions leverages the framework's built-in observability features, structured patterns, and rich diagnostic capabilities to quickly identify and resolve issues.**
