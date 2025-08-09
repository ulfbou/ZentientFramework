# 🚀 Getting Started with Zentient.Abstractions

Welcome to Zentient.Abstractions - the foundational library for building enterprise-grade .NET applications with the Zentient Framework's four-pillar architecture.

## 📦 Installation

### Package Manager Console
```powershell
Install-Package Zentient.Abstractions -Version 3.0.1
```

### .NET CLI
```bash
dotnet add package Zentient.Abstractions --version 3.0.1
```

### PackageReference
```xml
<PackageReference Include="Zentient.Abstractions" Version="3.0.1" />
```

## 🏗️ Framework Requirements

- **.NET 6.0** or higher
- **C# 10.0** language features
- **Nullable reference types** enabled (recommended)

## 🧩 Core Concepts

### 1. Definition-Centric Architecture

Everything in Zentient is self-describing through `ITypeDefinition`:

```csharp
using Zentient.Abstractions.Common;

// Every component describes itself
public class UserServiceDefinition : ITypeDefinition
{
    public string Name => "UserService";
    public string Description => "Manages user operations and authentication";
    public Type Type => typeof(IUserService);
}
```

### 2. Universal Envelope Pattern

All operations return standardized results:

```csharp
using Zentient.Abstractions.Envelopes;
using Zentient.Abstractions.Results;

public async Task<IResult<User>> GetUserAsync(int id)
{
    try
    {
        var user = await _repository.FindAsync(id);
        return user != null 
            ? Result.Success(user)
            : Result.Failure<User>("User not found");
    }
    catch (Exception ex)
    {
        return Result.Error<User>(ex);
    }
}
```

### 3. Fluent Dependency Injection

Build your application with fluent APIs:

```csharp
using Zentient.Abstractions.DependencyInjection;

var builder = new ContainerBuilder()
    .RegisterModule<UserModule>()
    .RegisterServices(services => services
        .AddTransient<IUserService, UserService>()
        .AddScoped<IUserRepository, UserRepository>())
    .EnableDiagnostics()
    .EnableValidation();

var container = builder.Build();
```

### 4. Built-in Observability

First-class diagnostics and validation:

```csharp
using Zentient.Abstractions.Diagnostics;
using Zentient.Abstractions.Validation;

public class UserValidator : IValidator<User, UserValidationError>
{
    public Task<IValidationResult<UserValidationError>> ValidateAsync(
        User user, 
        IValidationContext context)
    {
        var errors = new List<UserValidationError>();
        
        if (string.IsNullOrWhiteSpace(user.Email))
            errors.Add(new UserValidationError("Email is required"));
            
        return Task.FromResult(
            ValidationResult.Create(errors));
    }
}
```

## 🎯 Quick Start Example

Here's a complete example showing the four pillars in action:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zentient.Abstractions.DependencyInjection;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Results;

// 1. Define your service contract
public interface IWeatherService : IHasName, IHasDescription
{
    Task<IResult<Weather>> GetWeatherAsync(string city);
}

// 2. Implement with definition-centric approach
public class WeatherService : IWeatherService
{
    public string Name => "Weather Service";
    public string Description => "Provides weather information for cities";
    
    public async Task<IResult<Weather>> GetWeatherAsync(string city)
    {
        // Simulate weather API call
        if (string.IsNullOrWhiteSpace(city))
            return Result.Failure<Weather>("City name is required");
            
        var weather = new Weather 
        { 
            City = city, 
            Temperature = Random.Shared.Next(-10, 35),
            Condition = "Sunny"
        };
        
        return Result.Success(weather);
    }
}

// 3. Configure with fluent DI
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Use Zentient's fluent builder
                var zentientBuilder = new ContainerBuilder()
                    .RegisterServices(s => s
                        .AddScoped<IWeatherService, WeatherService>())
                    .EnableDiagnostics()
                    .EnableValidation();
                    
                // Integrate with Microsoft DI
                zentientBuilder.PopulateServices(services);
            })
            .Build();

        // 4. Use with built-in observability
        var weatherService = host.Services.GetRequiredService<IWeatherService>();
        var result = await weatherService.GetWeatherAsync("London");
        
        if (result.IsSuccess)
        {
            Console.WriteLine($"Weather in {result.Value.City}: {result.Value.Temperature}°C, {result.Value.Condition}");
        }
        else
        {
            Console.WriteLine($"Error: {result.ErrorMessage}");
        }
    }
}

public record Weather
{
    public string City { get; init; } = string.Empty;
    public int Temperature { get; init; }
    public string Condition { get; init; } = string.Empty;
}
```

## 📚 Next Steps

- **[API Reference](../api/)** - Explore the complete API
- **[Best Practices](best-practices.md)** - Learn recommended patterns
- **[Advanced Scenarios](advanced-scenarios.md)** - Complex use cases
- **[Migration Guide](MIGRATION_GUIDE_2.x_to_3.0.md)** - Upgrade from v2.x
- **[Samples Repository](https://github.com/ulfbou/Zentient.Samples)** - Complete examples

## 🆘 Need Help?

- 📖 [Documentation](https://ulfbou.github.io/Zentient.Abstractions/)
- 🐛 [Issues & Support](https://github.com/ulfbou/Zentient.Abstractions/issues)
- 💬 [Discussions](https://github.com/ulfbou/Zentient.Abstractions/discussions)
- 📧 [Contact](mailto:ulf.bourelius@gmail.com)
