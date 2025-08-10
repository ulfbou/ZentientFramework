# Zentient.Template — {One-line Description of the Module's Purpose}

[![NuGet](https://img.shields.io/nuget/v/Zentient.Template?label=Zentient.Template)](https://www.nuget.org/packages/Zentient.Template)
[![Build](https://img.shields.io/github/actions/workflow/status/ulfbou/Zentient.Template/build.yml)](https://github.com/ulfbou/Zentient.Template/actions)
![License](https://img.shields.io/github/license/ulfbou/Zentient.Template)
![.NET Versions](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-blue)

---

## Table of Contents

* [Overview](#-overview)
* [Why Zentient.Template?](#-why-zentientmodulename)
* [Architecture](#-architecture-overview)
* [Quick Start](#-quick-start)
* [Advanced Usage](#-advanced-usage)
* [Integration](#-integration)
* [Observability](#-observability)
* [Vision & Roadmap](#-vision--roadmap)
* [Contributing](#-contributing)

---

## 🚀 Overview

**Zentient.Template** is a modular, low-boilerplate building block for advanced .NET architectures. It aligns with the Zentient philosophy of clean separation, developer-first ergonomics, and protocol-agnostic design.

Whether used standalone or as part of the larger [Zentient Framework](https://github.com/ulfbou/zentient), this module offers high cohesion and pluggability across the result pipeline, validation, telemetry, or domain boundaries.

---

## ❓ Why Zentient.Template?

Common challenges in modern .NET systems that this module addresses:

* 🔁 Repetition of infrastructure logic across layers
* ⚠️ Tight coupling between concerns (validation, mapping, transport)
* 🔍 Lack of composability and observability around critical flows
* 🚫 Overuse of exceptions where structured data would be superior

### ✨ Key Features

* 🧩 **Composable Abstractions**
  Built on interface-first contracts, extensible via DI and partial opt-in.

* 📦 **Minimal Dependencies**
  Zero heavy framework bindings unless explicitly extended (e.g., `Http`, `Grpc`, etc).

* 🛠️ **Developer-First APIs**
  Predictable, discoverable, and functional-style extensions across the stack.

* 📐 **Clean Architecture Alignment**
  Each library is suitable for Domain, Application, or Presentation layers as appropriate.

* 🧪 **Testability by Design**
  Interfaces and factories are easy to mock, verify, or extend.

---

## 🏛️ Architecture Overview

This library is part of the **Zentient** modular ecosystem:

```
[ Domain Logic ] → [ Zentient.Results ] → [ Zentient.Template ] → [ Transport / Storage / Infra ]
```

A typical integration flow:

* Application returns `IResult<T>`
* `PackageTemplate` handles the cross-cutting concern (e.g., telemetry, validation, mapping)
* Final output is adapted to the transport layer or infrastructure gateway

![Architecture Diagram](./docs/assets/diagram.svg)

---

## 💻 Quick Start

**1. Install via NuGet**

```bash
dotnet add package Zentient.Template
```

**2. Register services in `Program.cs`**

```csharp
builder.Services.AddZentientPackageTemplate(); // DI-friendly setup
```

**3. Apply in your logic**

```csharp
var result = await _myService.DoWorkAsync(request);
return result
    .PipeThroughMyConcern()
    .ToWhatever(); // depends on module
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
