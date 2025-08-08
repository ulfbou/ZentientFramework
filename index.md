# Zentient.Abstractions API Documentation

Welcome to the Zentient.Abstractions API documentation. This comprehensive library provides the foundational abstractions for the Zentient Framework 3.0.

## 🏗️ Four-Pillar Architecture

Zentient Framework 3.0 is built upon four interconnected pillars:

### 1. 🧩 Definition-Centric Core
Everything in the framework is self-describing through the `ITypeDefinition` interface and composed `IHas...` contracts.

### 2. ✉️ Universal Envelope  
All operations return standardized `IEnvelope<TCode, TError>` results for consistent success/failure communication.

### 3. 🏗️ Fluent DI & Application Builder
The powerful `IContainerBuilder` serves as the composition root with fluent APIs for dependency injection.

### 4. 🩺 Built-in Observability
First-class diagnostic and validation systems through `IDiagnosticCheck` and `IValidator` interfaces.

## Quick Links

- [📚 API Documentation](api/index.md)
- [🔗 Browse by Namespace](api/)

## 🚀 Getting Started

```csharp
// Install the package
dotnet add package Zentient.Abstractions

// Basic usage
using Zentient.Abstractions;

// Use the unified interface
var zentient = serviceProvider.GetRequiredService<IZentient>();
```

## 🔗 Additional Resources

- [GitHub Repository](https://github.com/ulfbou/Zentient.Abstractions)
- [NuGet Package](https://www.nuget.org/packages/Zentient.Abstractions)
- [Migration Guide](https://github.com/ulfbou/Zentient.Abstractions/blob/main/MIGRATION_GUIDE_2.x_to_3.0.md)

---

**Supported Frameworks:** .NET 6.0, 7.0, 8.0, 9.0
