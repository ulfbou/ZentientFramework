# PROJECT_NAME Architecture

This document outlines the architectural decisions and patterns used in PROJECT_NAME.

## Overview

PROJECT_NAME follows modern .NET architectural patterns with emphasis on:

- **Clean Architecture**: Separation of concerns across layers
- **Dependency Injection**: Built-in .NET DI container
- **Configuration Management**: Strongly-typed configuration
- **Logging**: Structured logging with Serilog
#if (UseEntityFramework)
- **Data Access**: Entity Framework Core with repository pattern
#endif
#if (UseRedisCache)
- **Caching**: Redis for distributed caching
#endif
#if (UseDocker)
- **Containerization**: Docker support for deployment
#endif

## Project Structure

### Core Application (`src/PROJECT_NAME.Core`)

The main application project containing:

- **Infrastructure/Configuration**: Configuration extensions and settings
- **Infrastructure/DependencyInjection**: Service registration
#if (IncludeAdvancedPatterns)
- **Domain**: Business logic and domain models
- **Application**: Use cases and application services
- **Infrastructure**: External concerns (database, caching, etc.)
#endif

### Build System

The project uses a modular MSBuild configuration:

- `Directory.Build.props`: Common properties and package references
- `Directory.Pack.props`: NuGet packaging configuration
- `Directory.Quality.props`: Code analysis and quality rules
- `Directory.Security.props`: Security analysis and source linking
- `Directory.Documentation.props`: XML documentation and DocFX
- `Directory.Performance.props`: Performance optimization settings
- `Directory.Sign.props`: Assembly signing configuration
- `Directory.Test.props`: Test framework configuration

## Key Patterns

### Configuration

Configuration is managed through:

```csharp
// Strongly-typed configuration
services.Configure<MySettings>(configuration.GetSection("MySettings"));

// Configuration validation
services.AddOptionsWithValidation<MySettings>("MySettings");
```

### Dependency Injection

Services are registered in `ServiceCollectionExtensions`:

```csharp
public static IServiceCollection AddCoreServices(this IServiceCollection services)
{
    // Register your services here
    return services;
}
```

#if (UseEntityFramework)
### Data Access

Entity Framework Core is configured for:

- Code-first migrations
- Repository pattern implementation
- Unit of Work pattern
- Connection string management

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```
#endif

#if (UseRedisCache)
### Caching Strategy

Redis caching is implemented for:

- Distributed caching across instances
- Session state management
- Application-level caching

```csharp
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = connectionString;
});
```
#endif

## Quality Assurance

### Code Analysis

The project includes:

- **StyleCop Analyzers**: Consistent code formatting
- **Microsoft.CodeAnalysis.NetAnalyzers**: Security and performance rules
- **Custom Rulesets**: Project-specific quality rules

### Testing Strategy

- **Unit Tests**: Fast, isolated tests for business logic
- **Integration Tests**: End-to-end testing with real dependencies
- **Code Coverage**: Minimum 80% coverage requirement
- **Performance Tests**: Benchmarking critical paths

### Documentation

- **XML Documentation**: Required for all public APIs
- **DocFX**: Automated API documentation generation
- **Architecture Decision Records**: Document significant decisions

## Security Considerations

- **Source Link**: Enable debugging into NuGet packages
- **Deterministic Builds**: Reproducible build outputs
- **Assembly Signing**: Strong naming for production assemblies
- **Dependency Scanning**: Regular security updates

## Performance

- **AOT Ready**: Prepared for Ahead-of-Time compilation
- **Trimming Support**: Optimized for self-contained deployments
- **Memory Management**: Careful resource disposal patterns
- **Async/Await**: Non-blocking I/O operations
