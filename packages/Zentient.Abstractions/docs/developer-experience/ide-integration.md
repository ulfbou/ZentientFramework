# 🛠️ IDE Integration & Developer Experience

## Overview

Zentient.Abstractions is designed with **IntelliSense-first development** in mind. Every aspect of the framework prioritizes developer productivity through excellent IDE integration, rich tooling support, and intuitive APIs.

## 🎯 IntelliSense-Driven Development

### **Discoverable APIs**

The framework is designed so developers can discover functionality through IntelliSense without consulting documentation:

```csharp
// Type 'services.AddZentient' and IntelliSense shows all options
services.AddZentientServices(builder =>
{
    builder.Register // IntelliSense shows: RegisterScoped, RegisterSingleton, RegisterTransient
        .RegisterScoped<IUserService, UserService>()
        .Add // IntelliSense shows: AddValidation, AddCaching, AddRetryPolicy, AddLogging, etc.
        .AddValidation()
        .AddCaching(TimeSpan.FromMinutes(30))
        .AddRetryPolicy(maxAttempts: 3);
});
```

### **Rich Method Signatures**

Method signatures provide complete information without needing documentation:

```csharp
// Method signature tells the complete story
public static IServiceRegistrationBuilder<TInterface, TImplementation> AddCaching<TInterface, TImplementation>(
    this IServiceRegistrationBuilder<TInterface, TImplementation> builder,
    TimeSpan ttl,
    string region = null,
    CacheInvalidationStrategy invalidationStrategy = CacheInvalidationStrategy.TimeExpiry,
    bool enableDistributedCache = false)
    where TImplementation : class, TInterface
```

### **Fluent API Chaining**

APIs are designed for natural, readable chaining:

```csharp
services.AddZentientServices(builder =>
{
    builder
        .RegisterScoped<IOrderService, OrderService>()
        .AddValidation<OrderRequestValidator>()
        .AddCaching(TimeSpan.FromHours(1))
        .AddRetryPolicy(maxAttempts: 3, backoffStrategy: BackoffStrategy.Exponential)
        .AddCircuitBreaker(failureThreshold: 5, openDuration: TimeSpan.FromMinutes(1))
        .AddLogging(LogLevel.Information)
        .AddMetrics(includeDetailedMetrics: true)
        .AddHealthCheck(timeout: TimeSpan.FromSeconds(30));
});
```

## 🎨 Visual Studio Integration

### **Project Templates**

Zentient provides rich project templates that set up complete application structures:

```bash
# Install Zentient templates
dotnet new install Zentient.Templates

# Create new projects with complete setup
dotnet new zentient-api --name MyApi
dotnet new zentient-service --name MyService  
dotnet new zentient-microservice --name MyMicroservice
```

### **Code Snippets**

Rich code snippets accelerate common patterns:

- `zservice` - Creates a complete service with definition
- `zenvelope` - Creates envelope handling code
- `zvalidator` - Creates a validator implementation
- `zhealthcheck` - Creates a health check implementation
- `zcache` - Creates caching implementation

```csharp
// Type 'zservice' and press Tab twice
[ServiceDefinition("$name$", Version = "$version$")]
[ServiceRegistration(ServiceLifetime.$lifetime$)]
public class $name$Service : I$name$Service
{
    private readonly ILogger<$name$Service> _logger;
    
    public $name$Service(ILogger<$name$Service> logger)
    {
        _logger = logger;
    }
    
    public string Id => "$name$Service.v$version$";
    
    public async Task<IEnvelope<$name$Code, $name$Error>> Process($name$Request request)
    {
        try
        {
            // Implementation here
            return Envelope.Success($name$Code.Success, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process {RequestType}", typeof($name$Request).Name);
            return Envelope.Error<$name$Code, $name$Error>(
                $name$Error.ProcessingFailed(ex.Message)
            );
        }
    }
}
```

### **IntelliCode Support**

Zentient provides AI-assisted development through IntelliCode:

- **Smart Completions**: Context-aware suggestions for service registration
- **Pattern Recognition**: Suggests complete implementation patterns
- **Best Practice Recommendations**: Warns about anti-patterns

### **Live Templates**

Comprehensive live templates for common scenarios:

#### **Service Implementation Template**
```csharp
// File template: ZentientService.cs
using Zentient.Abstractions.Common;
using Zentient.Abstractions.DependencyInjection;
using Zentient.Abstractions.Envelopes;
using Microsoft.Extensions.Logging;

namespace $NAMESPACE$;

[ServiceDefinition("$SERVICE_NAME$", Version = "1.0")]
[ServiceRegistration(ServiceLifetime.Scoped)]
public class $SERVICE_NAME$Service : I$SERVICE_NAME$Service
{
    private readonly ILogger<$SERVICE_NAME$Service> _logger;
    
    public $SERVICE_NAME$Service(ILogger<$SERVICE_NAME$Service> logger)
    {
        _logger = logger;
    }
    
    public string Id => "$SERVICE_NAME$Service.v1.0";
    
    $METHODS$
}
```

#### **CQRS Handler Template**
```csharp
// File template: ZentientCommandHandler.cs
[CommandHandler(typeof($COMMAND_NAME$Command))]
[ServiceRegistration(ServiceLifetime.Scoped)]
public class $COMMAND_NAME$CommandHandler : ICommandHandler<$COMMAND_NAME$Command, $RESULT_TYPE$>
{
    private readonly IValidator<$COMMAND_NAME$Command> _validator;
    private readonly ILogger<$COMMAND_NAME$CommandHandler> _logger;
    
    public async Task<IEnvelope<CommandCode, CommandError>> Handle($COMMAND_NAME$Command command)
    {
        var validationResult = await _validator.Validate(command);
        if (!validationResult.IsValid)
        {
            return Envelope.ValidationError<CommandCode, CommandError>(
                validationResult.Errors.Select(e => CommandError.ValidationFailed(e.Message))
            );
        }
        
        try
        {
            // Implementation here
            return Envelope.Success(CommandCode.Success, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle command {CommandType}", typeof($COMMAND_NAME$Command).Name);
            return Envelope.Error<CommandCode, CommandError>(
                CommandError.ProcessingFailed(ex.Message)
            );
        }
    }
}
```

## 🔧 JetBrains Rider Integration

### **Live Templates**

Rider-specific live templates for rapid development:

```xml
<!-- ZentientService.xml -->
<templateSet group="Zentient">
  <template name="zservice" value="[ServiceDefinition(&quot;$SERVICE_NAME$&quot;, Version = &quot;$VERSION$&quot;)]&#10;[ServiceRegistration(ServiceLifetime.$LIFETIME$)]&#10;public class $SERVICE_NAME$Service : I$SERVICE_NAME$Service&#10;{&#10;    private readonly ILogger&lt;$SERVICE_NAME$Service&gt; _logger;&#10;    &#10;    public $SERVICE_NAME$Service(ILogger&lt;$SERVICE_NAME$Service&gt; logger)&#10;    {&#10;        _logger = logger;&#10;    }&#10;    &#10;    public string Id =&gt; &quot;$SERVICE_NAME$Service.v$VERSION$&quot;;&#10;    &#10;    $END$&#10;}" description="Create Zentient service" toReformat="true" toShortenFQNames="true">
    <variable name="SERVICE_NAME" expression="" defaultValue="" alwaysStopAt="true" />
    <variable name="VERSION" expression="" defaultValue="1.0" alwaysStopAt="true" />
    <variable name="LIFETIME" expression="enum(&quot;Scoped&quot;,&quot;Singleton&quot;,&quot;Transient&quot;)" defaultValue="Scoped" alwaysStopAt="true" />
    <context>
      <option name="CSHARP_TYPE_DECLARATION" value="true" />
    </context>
  </template>
</templateSet>
```

### **Structural Search and Replace**

Custom patterns for code modernization and refactoring:

```xml
<!-- Search Pattern: Old-style service registration -->
services.AddScoped<$SERVICE_TYPE$, $IMPLEMENTATION_TYPE$>();

<!-- Replace Pattern: Zentient-style registration -->
services.AddZentientServices(builder =>
{
    builder.RegisterScoped<$SERVICE_TYPE$, $IMPLEMENTATION_TYPE$>();
});
```

### **File Watchers**

Automatic code generation based on file changes:

```xml
<!-- ZentientCodeGen.xml -->
<TaskOptions>
  <option name="arguments" value="generate-service --input $FilePath$ --output $FileDir$" />
  <option name="checkSyntaxErrors" value="true" />
  <option name="description" />
  <option name="exitCodeBehavior" value="ERROR" />
  <option name="fileExtension" value="definition.cs" />
  <option name="immediateSync" value="false" />
  <option name="name" value="Zentient Service Generator" />
  <option name="output" value="" />
  <option name="outputFilters">
    <array />
  </option>
  <option name="outputFromStdout" value="false" />
  <option name="program" value="zentient-cli" />
  <option name="runOnExternalChanges" value="true" />
  <option name="scopeName" value="Project Files" />
  <option name="trackOnlyRoot" value="true" />
  <option name="workingDir" value="$ProjectFileDir$" />
  <option name="envs">
    <map />
  </option>
</TaskOptions>
```

## 🎮 VS Code Integration

### **Extension Features**

The Zentient VS Code extension provides:

- **Syntax Highlighting**: Rich highlighting for Zentient attributes and patterns
- **Code Completion**: Intelligent completion for service registration
- **Hover Information**: Rich tooltips showing service definitions and metadata
- **Go to Definition**: Navigate to service definitions and implementations
- **Find All References**: Find all usages of services and abstractions

### **Command Palette Integration**

Quick access to common operations:

- `Zentient: Create Service` - Generate a new service from template
- `Zentient: Create Validator` - Generate a validator implementation
- `Zentient: Create Health Check` - Generate a health check
- `Zentient: Analyze Dependencies` - Show dependency graph
- `Zentient: Generate Documentation` - Create API documentation

### **Debugging Support**

Enhanced debugging experience:

- **Envelope Visualizers**: Rich display of envelope contents during debugging
- **Service Tree View**: Visualize the service dependency tree
- **Health Check Dashboard**: Real-time health check status
- **Metrics Dashboard**: Live metrics and performance data

### **Settings Schema**

Rich JSON schema for configuration files:

```json
{
  "zentient": {
    "services": {
      "autoRegistration": {
        "enabled": true,
        "assemblies": ["MyApp.Services", "MyApp.Repositories"],
        "conventions": ["*Service", "*Repository", "*Handler"]
      },
      "validation": {
        "validateOnStartup": true,
        "validateDependencies": true,
        "validateConfiguration": true
      },
      "observability": {
        "logging": {
          "level": "Information",
          "includeScopes": true,
          "includeMetadata": true
        },
        "metrics": {
          "enabled": true,
          "detailedMetrics": true,
          "exportInterval": "00:01:00"
        },
        "tracing": {
          "enabled": true,
          "samplingRatio": 0.1,
          "exportEndpoint": "http://jaeger:14268"
        }
      }
    }
  }
}
```

## 🔍 Code Analysis & Diagnostics

### **Custom Analyzers**

Zentient provides rich code analysis through Roslyn analyzers:

#### **ZEN001: Missing Service Registration**
```csharp
// This triggers ZEN001
public class UserService : IUserService // ❌ Missing [ServiceRegistration]
{
}

// Fixed version
[ServiceRegistration(ServiceLifetime.Scoped)] // ✅ Analyzer satisfied
public class UserService : IUserService
{
}
```

#### **ZEN002: Inconsistent Envelope Usage**
```csharp
// This triggers ZEN002
public class UserService : IUserService
{
    public async Task<User> GetUser(string id) // ❌ Should return IEnvelope
    {
    }
}

// Fixed version
public class UserService : IUserService
{
    public async Task<IEnvelope<UserCode, UserError>> GetUser(string id) // ✅
    {
    }
}
```

#### **ZEN003: Missing Service Definition**
```csharp
// This triggers ZEN003
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService // ❌ Missing [ServiceDefinition]
{
}

// Fixed version
[ServiceDefinition("UserManagement", Version = "1.0")] // ✅
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
}
```

#### **ZEN004: Unvalidated Dependencies**
```csharp
// This triggers ZEN004
services.AddZentientServices(builder =>
{
    builder.RegisterScoped<IOrderService, OrderService>(); // ❌ Missing dependency validation
});

// Fixed version
services.AddZentientServices(builder =>
{
    builder
        .RegisterScoped<IOrderService, OrderService>()
        .ValidateDependencies(); // ✅
});
```

### **Code Fixes**

Automatic code fixes for common issues:

```csharp
// Before (triggers analyzer)
public class UserService : IUserService
{
    public async Task<User> CreateUser(CreateUserRequest request)
    {
        // Implementation
    }
}

// After applying code fix
[ServiceDefinition("UserManagement", Version = "1.0")]
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    public async Task<IEnvelope<UserCode, UserError>> CreateUser(CreateUserRequest request)
    {
        try
        {
            // Implementation
            return Envelope.Success(UserCode.UserCreated, user);
        }
        catch (Exception ex)
        {
            return Envelope.Error<UserCode, UserError>(
                UserError.CreationFailed(ex.Message)
            );
        }
    }
}
```

### **Refactoring Support**

Intelligent refactoring operations:

#### **Extract Service Interface**
```csharp
// Before refactoring
[ServiceDefinition("UserManagement", Version = "1.0")]
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService
{
    public async Task<IEnvelope<UserCode, UserError>> CreateUser(CreateUserRequest request)
    {
        // Implementation
    }
}

// After refactoring (automatically generates interface)
public interface IUserService : IIdentifiable
{
    Task<IEnvelope<UserCode, UserError>> CreateUser(CreateUserRequest request);
}

[ServiceDefinition("UserManagement", Version = "1.0")]
[ServiceRegistration(ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    public string Id => "UserService.v1.0";
    
    public async Task<IEnvelope<UserCode, UserError>> CreateUser(CreateUserRequest request)
    {
        // Implementation
    }
}
```

#### **Convert to Envelope Pattern**
```csharp
// Before refactoring
public async Task<User> GetUser(string id)
{
    var user = await _repository.GetById(id);
    if (user == null)
        throw new UserNotFoundException(id);
    return user;
}

// After refactoring
public async Task<IEnvelope<UserCode, UserError>> GetUser(string id)
{
    try
    {
        var user = await _repository.GetById(id);
        return user != null
            ? Envelope.Success(UserCode.UserFound, user)
            : Envelope.NotFound<UserCode, UserError>(UserError.UserNotFound(id));
    }
    catch (Exception ex)
    {
        return Envelope.Error<UserCode, UserError>(UserError.DatabaseError(ex.Message));
    }
}
```

## 🎯 Productivity Features

### **Automatic Code Generation**

Generate complete implementations from interfaces:

```csharp
// Define interface
public interface IOrderService : IIdentifiable
{
    Task<IEnvelope<OrderCode, OrderError>> CreateOrder(CreateOrderRequest request);
    Task<IEnvelope<OrderCode, OrderError>> GetOrder(string orderId);
    Task<IEnvelope<OrderCode, OrderError>> UpdateOrder(UpdateOrderRequest request);
    Task<IEnvelope<OrderCode, OrderError>> CancelOrder(string orderId);
}

// Right-click -> "Generate Zentient Implementation"
// Automatically creates:
[ServiceDefinition("OrderManagement", Version = "1.0")]
[ServiceRegistration(ServiceLifetime.Scoped)]
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IOrderRepository _repository;
    private readonly IValidator<CreateOrderRequest> _createValidator;
    private readonly IValidator<UpdateOrderRequest> _updateValidator;
    
    public OrderService(
        ILogger<OrderService> logger,
        IOrderRepository repository,
        IValidator<CreateOrderRequest> createValidator,
        IValidator<UpdateOrderRequest> updateValidator)
    {
        _logger = logger;
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    
    public string Id => "OrderService.v1.0";
    
    public async Task<IEnvelope<OrderCode, OrderError>> CreateOrder(CreateOrderRequest request)
    {
        var validationResult = await _createValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Envelope.ValidationError<OrderCode, OrderError>(
                validationResult.Errors.Select(e => OrderError.ValidationFailed(e.Message))
            );
        }
        
        try
        {
            var order = await _repository.Create(request);
            _logger.LogInformation("Order {OrderId} created successfully", order.Id);
            return Envelope.Success(OrderCode.OrderCreated, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order");
            return Envelope.Error<OrderCode, OrderError>(OrderError.CreationFailed(ex.Message));
        }
    }
    
    // ... other methods with similar patterns
}
```

### **Smart Scaffolding**

Generate complete project structures:

```bash
# Command line scaffolding
zentient scaffold microservice --name OrderService --domain Orders
```

Generates:
```
OrderService/
├── src/
│   ├── OrderService.Domain/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── Repositories/
│   ├── OrderService.Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Handlers/
│   │   └── Validators/
│   ├── OrderService.Infrastructure/
│   │   ├── Data/
│   │   ├── External/
│   │   └── Configuration/
│   └── OrderService.Api/
│       ├── Controllers/
│       ├── Middleware/
│       └── Configuration/
├── tests/
│   ├── OrderService.Domain.Tests/
│   ├── OrderService.Application.Tests/
│   ├── OrderService.Infrastructure.Tests/
│   └── OrderService.Api.Tests/
└── docs/
    ├── api-specification.yaml
    ├── architecture.md
    └── deployment.md
```

### **Configuration Intellisense**

Rich IntelliSense for configuration files:

```json
{
  "Zentient": {
    "Services": {
      "UserService": {
        "Cache": {
          "TTL": "00:30:00", // IntelliSense shows TimeSpan format
          "Region": "Users",  // IntelliSense shows available regions
          "Distributed": true // IntelliSense shows boolean options
        },
        "Retry": {
          "MaxAttempts": 3,              // IntelliSense shows valid range
          "BackoffStrategy": "Exponential", // IntelliSense shows enum values
          "BaseDelay": "00:00:01"        // IntelliSense shows TimeSpan format
        }
      }
    }
  }
}
```

---

**The Zentient IDE integration is designed to make developers productive from day one, with rich tooling support that guides best practices and accelerates development through intelligent automation.**
