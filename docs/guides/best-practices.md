# 🎯 Best Practices for Zentient.Abstractions

This guide outlines recommended patterns, practices, and conventions for building robust applications with Zentient.Abstractions.

## 🏗️ Architecture Principles

### 1. Definition-First Design

Always start with clear, self-describing definitions:

```csharp
// ✅ Good: Clear, descriptive definition
public class OrderProcessingServiceDefinition : ITypeDefinition
{
    public string Name => "OrderProcessingService";
    public string Description => "Processes customer orders through the complete fulfillment pipeline";
    public string Category => "Business Logic";
    public Type Type => typeof(IOrderProcessingService);
    public string Version => "1.2.0";
}

// ❌ Avoid: Vague or incomplete definitions
public class ServiceDef : ITypeDefinition
{
    public string Name => "Service";
    public string Description => "Does stuff";
    public Type Type => typeof(object);
}
```

### 2. Envelope Everything

Use consistent result patterns throughout your application:

```csharp
// ✅ Good: Consistent envelope usage
public async Task<IResult<Order>> CreateOrderAsync(CreateOrderRequest request)
{
    try
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Invalid<Order>(validationResult.Errors);

        var order = await _orderService.CreateAsync(request);
        return Result.Success(order);
    }
    catch (BusinessException ex)
    {
        return Result.Failure<Order>(ex.Message);
    }
    catch (Exception ex)
    {
        return Result.Error<Order>(ex);
    }
}

// ❌ Avoid: Mixed return patterns
public async Task<Order?> CreateOrderBad(CreateOrderRequest request)
{
    if (!IsValid(request))
        throw new ArgumentException("Invalid request");
        
    return await _service.CreateAsync(request); // Could return null
}
```

### 3. Validation Early and Often

Implement comprehensive validation at boundaries:

```csharp
public class CreateOrderRequestValidator : IValidator<CreateOrderRequest, OrderValidationError>
{
    public async Task<IValidationResult<OrderValidationError>> ValidateAsync(
        CreateOrderRequest request, 
        IValidationContext context)
    {
        var errors = new List<OrderValidationError>();

        // Required field validation
        if (string.IsNullOrWhiteSpace(request.CustomerId))
            errors.Add(OrderValidationError.CustomerIdRequired());

        // Business rule validation
        if (request.Items?.Count == 0)
            errors.Add(OrderValidationError.OrderMustHaveItems());

        // Async validation
        if (!string.IsNullOrWhiteSpace(request.CustomerId))
        {
            var customerExists = await _customerService.ExistsAsync(request.CustomerId);
            if (!customerExists)
                errors.Add(OrderValidationError.CustomerNotFound(request.CustomerId));
        }

        return ValidationResult.Create(errors);
    }
}
```

## 🔧 Dependency Injection Patterns

### Service Registration Best Practices

```csharp
public class OrderModule : IServiceModule
{
    public void ConfigureServices(IServiceRegistrationBuilder builder)
    {
        // Register with explicit lifetimes
        builder
            .AddTransient<IOrderValidator, OrderValidator>()
            .AddScoped<IOrderService, OrderService>()
            .AddSingleton<IOrderConfiguration, OrderConfiguration>()
            
            // Register with factory patterns for complex initialization
            .AddScoped<IOrderProcessor>(provider => 
                new OrderProcessor(
                    provider.GetRequiredService<IPaymentService>(),
                    provider.GetRequiredService<IInventoryService>(),
                    provider.GetRequiredService<ILogger<OrderProcessor>>()))
                    
            // Register with conditions
            .AddScoped<INotificationService, EmailNotificationService>()
                .When(ctx => ctx.Environment.IsProduction())
                
            .AddScoped<INotificationService, ConsoleNotificationService>()
                .When(ctx => ctx.Environment.IsDevelopment());
    }
}
```

### Avoid Service Locator Anti-Pattern

```csharp
// ✅ Good: Constructor injection
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IValidator<Order> _validator;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository repository,
        IValidator<Order> validator,
        ILogger<OrderService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}

// ❌ Avoid: Service locator pattern
public class OrderServiceBad : IOrderService
{
    private readonly IServiceProvider _serviceProvider;

    public OrderServiceBad(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ProcessAsync(Order order)
    {
        // Anti-pattern: asking for dependencies at runtime
        var repository = _serviceProvider.GetRequiredService<IOrderRepository>();
        var validator = _serviceProvider.GetRequiredService<IValidator<Order>>();
    }
}
```

## 🔍 Error Handling Strategies

### Structured Error Information

```csharp
public record OrderError : IErrorInfo<OrderErrorCode>
{
    public OrderErrorCode Code { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? Details { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string? CorrelationId { get; init; }

    public static OrderError CustomerNotFound(string customerId) => new()
    {
        Code = OrderErrorCode.CustomerNotFound,
        Message = "Customer not found",
        Details = $"Customer ID: {customerId}"
    };

    public static OrderError InsufficientInventory(string productId, int requested, int available) => new()
    {
        Code = OrderErrorCode.InsufficientInventory,
        Message = "Insufficient inventory",
        Details = $"Product: {productId}, Requested: {requested}, Available: {available}"
    };
}

public enum OrderErrorCode
{
    Unknown = 0,
    CustomerNotFound = 1001,
    InsufficientInventory = 1002,
    PaymentFailed = 1003,
    InvalidOrderData = 1004
}
```

### Error Propagation Patterns

```csharp
public async Task<IResult<Order>> ProcessOrderAsync(CreateOrderRequest request)
{
    // Validate input
    var validationResult = await _validator.ValidateAsync(request);
    if (!validationResult.IsValid)
        return Result.Invalid<Order>(validationResult.Errors.Select(e => e.Message));

    // Check customer
    var customerResult = await _customerService.GetCustomerAsync(request.CustomerId);
    if (!customerResult.IsSuccess)
        return Result.PropagateFailure<Order>(customerResult);

    // Reserve inventory
    var inventoryResult = await _inventoryService.ReserveItemsAsync(request.Items);
    if (!inventoryResult.IsSuccess)
        return Result.PropagateFailure<Order>(inventoryResult);

    // Process payment
    var paymentResult = await _paymentService.ProcessPaymentAsync(request.Payment);
    if (!paymentResult.IsSuccess)
    {
        // Compensating action
        await _inventoryService.ReleaseReservationAsync(inventoryResult.Value);
        return Result.PropagateFailure<Order>(paymentResult);
    }

    // Create order
    var order = new Order(customerResult.Value, inventoryResult.Value, paymentResult.Value);
    return Result.Success(order);
}
```

## 🎭 Context and Scope Management

### Execution Context Patterns

```csharp
public class OrderProcessingScope : IExecutionScope
{
    public string CorrelationId { get; }
    public string UserId { get; }
    public DateTime StartTime { get; }
    
    public OrderProcessingScope(string correlationId, string userId)
    {
        CorrelationId = correlationId;
        UserId = userId;
        StartTime = DateTime.UtcNow;
    }
}

public class OrderService : IOrderService
{
    private readonly IExecutionScopeAccessor _scopeAccessor;

    public async Task<IResult<Order>> CreateOrderAsync(CreateOrderRequest request)
    {
        using var scope = _scopeAccessor.BeginScope(
            new OrderProcessingScope(request.CorrelationId, request.UserId));

        // All operations within this scope have access to the context
        return await ProcessOrderInternalAsync(request);
    }
}
```

## 🧪 Testing Strategies

### Unit Testing with Zentient Patterns

```csharp
[TestClass]
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IValidator<Order>> _mockValidator;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockValidator = new Mock<IValidator<Order>>();
        _mockLogger = new Mock<ILogger<OrderService>>();
        
        _orderService = new OrderService(
            _mockRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    [TestMethod]
    public async Task CreateOrderAsync_WithValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var request = new CreateOrderRequest { /* valid data */ };
        var expectedOrder = new Order { /* expected data */ };
        
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<Order>(), It.IsAny<IValidationContext>()))
            .ReturnsAsync(ValidationResult.Valid<OrderValidationError>());
            
        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _orderService.CreateOrderAsync(request);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(expectedOrder, result.Value);
        
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
        _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<Order>(), It.IsAny<IValidationContext>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateOrderAsync_WithInvalidRequest_ReturnsFailureResult()
    {
        // Arrange
        var request = new CreateOrderRequest { /* invalid data */ };
        var validationErrors = new[] { OrderValidationError.CustomerIdRequired() };
        
        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<Order>(), It.IsAny<IValidationContext>()))
            .ReturnsAsync(ValidationResult.Invalid(validationErrors));

        // Act
        var result = await _orderService.CreateOrderAsync(request);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.IsInvalid);
        CollectionAssert.AreEqual(validationErrors.Select(e => e.Message).ToArray(), 
                                  result.ValidationErrors.ToArray());
        
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Never);
    }
}
```

## 🏃‍♂️ Performance Considerations

### Async Patterns

```csharp
// ✅ Good: Proper async/await usage
public async Task<IResult<IEnumerable<Order>>> GetOrdersAsync(string customerId)
{
    var orders = await _repository.GetOrdersByCustomerAsync(customerId);
    return Result.Success(orders);
}

// ✅ Good: Parallel operations when appropriate
public async Task<IResult<OrderSummary>> GetOrderSummaryAsync(string customerId)
{
    var ordersTask = _repository.GetOrdersByCustomerAsync(customerId);
    var customerTask = _customerService.GetCustomerAsync(customerId);
    
    await Task.WhenAll(ordersTask, customerTask);
    
    var orders = await ordersTask;
    var customer = await customerTask;
    
    return Result.Success(new OrderSummary(customer.Value, orders));
}

// ❌ Avoid: Blocking async operations
public IResult<Order> GetOrderBad(int orderId)
{
    var order = _repository.GetOrderAsync(orderId).Result; // Blocks!
    return Result.Success(order);
}
```

### Memory Management

```csharp
// ✅ Good: Dispose resources properly
public async Task<IResult<ProcessingReport>> ProcessLargeOrderBatchAsync(
    IEnumerable<CreateOrderRequest> requests)
{
    using var scope = _serviceProvider.CreateScope();
    var processor = scope.ServiceProvider.GetRequiredService<IBatchOrderProcessor>();
    
    return await processor.ProcessBatchAsync(requests);
}

// ✅ Good: Use streaming for large datasets
public async IAsyncEnumerable<IResult<Order>> ProcessOrdersStreamAsync(
    IAsyncEnumerable<CreateOrderRequest> requests,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    await foreach (var request in requests.WithCancellation(cancellationToken))
    {
        var result = await ProcessOrderAsync(request);
        yield return result;
    }
}
```

## 🔒 Security Best Practices

### Input Validation

```csharp
public class SecurityAwareValidator : IValidator<ApiRequest, SecurityValidationError>
{
    public async Task<IValidationResult<SecurityValidationError>> ValidateAsync(
        ApiRequest request, 
        IValidationContext context)
    {
        var errors = new List<SecurityValidationError>();

        // Sanitize inputs
        if (ContainsSqlInjectionPatterns(request.Query))
            errors.Add(SecurityValidationError.PotentialSqlInjection());

        // Validate authorization context
        if (!context.User.HasPermission(request.RequiredPermission))
            errors.Add(SecurityValidationError.InsufficientPermissions());

        return ValidationResult.Create(errors);
    }
}
```

## 📊 Observability Integration

### Structured Logging

```csharp
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;

    public async Task<IResult<Order>> CreateOrderAsync(CreateOrderRequest request)
    {
        using var activity = OrderServiceActivity.CreateOrder(request.CorrelationId);
        
        _logger.LogInformation(
            "Creating order for customer {CustomerId} with {ItemCount} items",
            request.CustomerId, 
            request.Items.Count);

        try
        {
            var result = await ProcessOrderInternalAsync(request);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Order {OrderId} created successfully for customer {CustomerId}",
                    result.Value.Id,
                    request.CustomerId);
            }
            else
            {
                _logger.LogWarning(
                    "Order creation failed for customer {CustomerId}: {ErrorMessage}",
                    request.CustomerId,
                    result.ErrorMessage);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error creating order for customer {CustomerId}",
                request.CustomerId);
            throw;
        }
    }
}
```

## 🎯 Summary

Following these best practices will help you build robust, maintainable applications with Zentient.Abstractions:

1. **Design with definitions first** - Make your components self-describing
2. **Use consistent result patterns** - Envelope all operations 
3. **Validate early and comprehensively** - Fail fast with clear errors
4. **Inject dependencies explicitly** - Avoid service locator patterns
5. **Handle errors gracefully** - Provide structured, actionable error information
6. **Test comprehensively** - Unit test all patterns and edge cases
7. **Monitor and observe** - Implement structured logging and metrics
8. **Secure by design** - Validate inputs and check permissions

These patterns will help you leverage the full power of Zentient's four-pillar architecture while maintaining code quality and developer productivity.
