# Zentient.Templates — Enterprise-Grade .NET Project Templates

[![Build](https://img.shields.io/github/actions/workflow/status/ulfbou/Zentient.Templates/docs.yml)](https://github.com/ulf**Minimal Web API Project:**
```bash
dotnet new zentient -n MyWebAPI \
  --UseDocker true \
  --UseRedisCache false \
  --UseEntityFramework false \
  --IncludeAdvancedPatterns false
```
ient.Templates/actions)
![License](https://img.shields.io/github/license/ulfbou/Zentient.Templates)
![.NET Versions](https://img.shields.io/badge/.NET-6.0%20%7C%207.0%20%7C%208.0%20%7C%209.0-blue)

---

## Table of Contents

* [Overview](#-overview)
* [Why Zentient Templates?](#-why-zentient-templates)
* [Available Templates](#-available-templates)
* [Quick Start](#-quick-start)
* [Template Features](#-template-features)
* [Advanced Usage](#-advanced-usage)
* [Contributing](#-contributing)

---

## 🚀 Overview

**Zentient.Templates** provides enterprise-grade .NET project templates that eliminate boilerplate and establish best practices from day one. These templates are designed with **developer experience (DX) friendliness** as the primary goal, providing everything developers need to be productive immediately.

Whether you're building libraries, applications, or complete projects, these templates provide comprehensive automation for building, testing, documentation, security, and deployment.

---

## ❓ Why Zentient Templates?

Common pain points when starting new .NET projects:

* � **Manual Setup Overhead** - Hours spent configuring build systems, CI/CD, quality gates
* 📚 **Inconsistent Standards** - Different projects using different conventions and tooling
* 🔍 **Missing Best Practices** - Security, performance, and maintainability considerations overlooked
* 🚫 **Incomplete Automation** - Manual processes that should be automated from the start

### ✨ Key Benefits

* 🏗️ **Zero-Setup Development**
  Everything works out of the box - no manual configuration required.

* 📦 **Enterprise-Ready Automation**
  Complete build, test, quality, security, and documentation pipelines included.

* 🛠️ **Developer-First Design**
  Optimized for productivity with clear conventions and comprehensive tooling.

* 🧪 **Production-Grade Quality**
  Security scanning, performance monitoring, and quality gates built-in.

---

## 📦 Available Templates

### 🏗️ Zentient Library Template (`zentient-lib`)
Enterprise-grade library template with comprehensive automation:

- **Complete Build System**: Multi-targeting, packaging, signing
- **Quality Automation**: Code analysis, StyleCop, security scanning  
- **Testing Infrastructure**: Unit tests, integration tests, coverage reporting
- **Documentation**: XML docs, DocFX integration, API documentation
- **Performance**: Benchmarking and profiling capabilities
- **Security**: Vulnerability scanning, compliance validation

```bash
dotnet new install ./templates/zentient-library-template
dotnet new zentient-lib -n MyLibrary
```

### 🚀 Zentient Project Template (`zentient`)
Complete application infrastructure template:

- **Multi-Project Structure**: Core, API, tests, and infrastructure
- **Modern Patterns**: CQRS, dependency injection, configuration management
- **Container Support**: Docker, docker-compose for development
- **Data Access**: Entity Framework integration with migrations
- **Caching**: Redis support for distributed scenarios

```bash
dotnet new install ./templates/zentient-project-template  
dotnet new zentient -n MyProject
```

---

## 💻 Quick Start

**1. Install Templates**

```bash
# Clone the repository
git clone https://github.com/ulfbou/Zentient.Templates.git
cd Zentient.Templates

# Install library template
dotnet new install ./templates/zentient-library-template

# Install project template  
dotnet new install ./templates/zentient-project-template
```

**2. Create a New Library**

```bash
# Basic library with all features
dotnet new zentient-lib -n Zentient.MyLibrary

# Customized library
dotnet new zentient-lib -n MyValidation \
  --LibraryType Validation \
  --EnablePerformance true \
  --Author "Your Name"
```

**3. Create a New Project**

```bash
# Complete project with all features
dotnet new zentient -n MyApplication

# Minimal project  
dotnet new zentient -n MySimpleApp \
  --UseDocker false \
  --UseRedisCache false
```

---

## �️ Template Features

### Comprehensive Automation
- **Build System**: MSBuild automation through Directory.*.props/targets files
- **Quality Gates**: Code analysis, StyleCop, security scanning
- **Testing**: xUnit, FluentAssertions, coverage reporting, benchmarks
- **Documentation**: XML docs, DocFX, API documentation generation
- **CI/CD**: GitHub Actions workflows for build, test, and deployment

### Developer Experience
- **Zero Configuration**: Everything works immediately after template instantiation
- **VS Code Integration**: Debug configurations, tasks, recommended extensions
- **IntelliSense**: Complete IDE support with proper project references
- **Hot Reload**: Development-optimized build configurations

### Enterprise Features
- **Security**: Vulnerability scanning, dependency auditing, security compliance
- **Performance**: Benchmarking, profiling, memory analysis
- **Observability**: Logging, metrics, health checks integration
- **Deployment**: Container support, package generation, signing

---

## 🔧 Advanced Usage

### Template Parameters

#### Library Template Parameters
| Parameter | Description | Default | Options |
|-----------|-------------|---------|---------|
| `ProjectName` | Library name | `Zentient.NewLibrary` | Any valid name |
| `Framework` | Target framework | `net8.0` | `net6.0`, `net7.0`, `net8.0`, `net9.0` |
| `LibraryType` | Library category | `Custom` | `Core`, `Validation`, `Configuration`, etc. |
| `EnableSigning` | Assembly signing | `true` | `true`, `false` |
| `EnableTesting` | Testing setup | `true` | `true`, `false` |
| `EnableDocumentation` | Documentation | `true` | `true`, `false` |
| `EnablePerformance` | Benchmarking | `false` | `true`, `false` |

#### Project Template Parameters  
| Parameter | Description | Default | Options |
|-----------|-------------|---------|---------|
| `ProjectName` | Project name | `MyZentientProject` | Any valid name |
| `UseDocker` | Docker support | `true` | `true`, `false` |
| `UseRedisCache` | Redis caching | `false` | `true`, `false` |
| `UseEntityFramework` | EF integration | `true` | `true`, `false` |
| `IncludeAdvancedPatterns` | CQRS patterns | `true` | `true`, `false` |

### Customization Examples

**Library with Performance Monitoring:**
```bash
dotnet new zentient-lib -n Zentient.MyCache \
  --LibraryType Caching \
  --EnablePerformance true \
  --EnableSecurity true \
  --Description "High-performance caching library"
```

**Minimal Web API Project:**
```bash
dotnet new zentient -n MyWebAPI \
  --UseDocker true \
  --UseRedisCache false \
  --UseEntityFramework false \
  --IncludeAdvancedPatterns false
```

---

## 🔧 Advanced Usage

### 🧰 Extend or Override Default Behavior

All Zentient modules support inversion-of-control overrides:

```csharp
public class MyCustomThing : I{ModuleServiceInterface}
{
    public Outcome Execute(...)
    {
        // your implementation
    }
}
```

Register with:

```csharp
builder.Services.AddScoped<I{ModuleServiceInterface}, MyCustomThing>();
```

### ⚙️ Functional Pipelines

Modules expose fluent, chainable operations via extension methods:

```csharp
var outcome = await _service.DoWork(input)
    .ValidateWith(...)
    .TransformWith(...)
    .ObserveWith(...);
```

---

## 🔌 Integration

Zentient modules are designed to be **transport-agnostic** and **ecosystem-neutral**, but optional extensions may include:

* `Zentient.Template.Http` — for ASP.NET Core integration
* `Zentient.Template.Grpc` — for gRPC-based bindings
* `Zentient.Template.Messaging` — for event-driven usage
* `Zentient.Template.Analyzers` — for Roslyn-assisted enforcement

---

## 📊 Observability

* 📈 **Structured Metadata Output**
  Every operation can emit standardized metadata for structured logging.

* 🔍 **Diagnostics-Friendly API**
  Hook into logs, traces, or error pipelines at every stage.

* 🧭 **OpenTelemetry Ready**
  Easily connect to spans, baggage, and tracing scopes.

---

## 🗺️ Vision & Roadmap

Zentient.Template is part of the long-term goal to modularize and standardize modern .NET systems around:

* 🧩 Extensible and decoupled service boundaries
* 🚦 Structured, domain-centric outcomes
* 📡 Telemetry-aware design
* 🧪 Developer-first tooling
* 🌐 Protocol and transport neutrality

**Planned:**

* ✅ Core abstraction stabilization
* 🔄 Support for additional transport adapters
* 🧠 IntelliSense-enhanced analyzers
* 🔒 Policy, validation, and observability extensions

---

## 🤝 Contributing

We welcome contributions from developers who care about modularity, clarity, and clean system boundaries.

* Fork the repository
* Submit an issue or join the discussion
* Open a PR with rationale and test coverage

> Zentient.Template exists to eliminate repetition, enforce clarity, and enable robust, scalable .NET systems.

---

> Created with ❤️ by [@ulfbou](https://github.com/ulfbou) and the Zentient contributors.
