# Zentient.Abstractions

**Zentient.Abstractions** provides the foundational interfaces and core types for building robust, extensible, and protocol-agnostic applications **across the entire Zentient Framework ecosystem**. It defines the essential contracts for representing operation outcomes, contextual information, metadata, and common patterns like policies and formatters, promoting a clean, modular, and testable architecture. **This library serves as a critical dependency for other Zentient components, including Zentient.Results, ensuring a consistent and unified approach to core concepts.**

## 🚀 Getting Started

Install Zentient.Abstractions into your .NET project using NuGet Package Manager:

```bash
dotnet add package Zentient.Abstractions
````

Zentient.Abstractions supports .NET 6.0, .NET 7.0, .NET 8.0, and .NET 9.0.

## ✨ Key Concepts & Features

This library introduces several fundamental abstractions:

  * **`IContext`**: Represents a runtime execution context, allowing for the propagation of contextual information (e.g., request type, metadata) throughout an operation.

    ```csharp
    public interface IContext
    {
        string Type { get; }
        IMetadata? Metadata { get; }
    }
    ```

  * **`IEnvelope` & `IEnvelope<TValue>`**: Defines a minimal, protocol-agnostic structure for the outcome of any operation. It indicates success or failure, provides a canonical code, optional messages, and metadata. `IEnvelope<TValue>` extends this for operations that yield a specific value.

    ```csharp
    public interface IEnvelope
    {
        bool IsSuccess { get; }
        IEndpointCode? Code { get; }
        IReadOnlyCollection<string>? Messages { get; }
        IMetadata? Metadata { get; }
    }

    public interface IEnvelope<out TValue> : IEnvelope
    {
        TValue? Value { get; }
    }
    ```

  * **`IEndpointCode`**: Provides a structured, symbolic way to interpret endpoint outcome semantics. It includes a `Name` (e.g., "NotFound", "Success"), an optional `Numeric` representation (e.g., HTTP 404), and a `Protocol` hint.

    ```csharp
    public interface IEndpointCode
    {
        string Name { get; }
        int? Numeric { get; }
        string? Protocol { get; }
    }
    ```

  * **`IMetadata`**: A flexible interface for attaching structured contextual metadata as key-value pairs to various entities (e.g., `IContext`, `IEnvelope`). It supports type-safe retrieval of tags.

    ```csharp
    public interface IMetadata
    {
        IReadOnlyCollection<string> Keys { get; }
        IReadOnlyCollection<object> Values { get; }
        bool TryGetTag<TValue>(string key, out TValue? value);
        IMetadata WithTag<TValue>(string key, TValue value);
    }
    ```

  * **`IPolicy<T>`**: Defines a contract for applying cross-cutting concerns or retry logic to operations that produce a result of type `T`.

    ```csharp
    public interface IPolicy<T>
    {
        Task<T> Execute(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default);
    }
    ```

  * **`IFormatter<TIn, TOut>`**: A generic interface for transforming one type of value into another, useful for data serialization, deserialization, or mapping.

    ```csharp
    public interface IFormatter<in TIn, TOut>
    {
        Task<TOut> Format(TIn input, CancellationToken cancellationToken = default);
    }
    ```

  * **`Unit`**: A lightweight, singleton value type used to represent the absence of a specific value in generic contexts, similar to `void` in C\# or `null` but as a concrete, serializable type. It's particularly useful in functional programming patterns where a generic type parameter is required but no meaningful data needs to be conveyed.

    ```csharp
    public readonly struct Unit : IEquatable<Unit>, IComparable, IComparable<Unit>
    {
        public static Unit Value { get; } // Singleton instance
        // ... operators and methods for equality, comparison, serialization
    }
    ```

## 💡 Why Use Zentient.Abstractions?

  * **Protocol Agnostic:** Design your core logic independently of specific transport protocols (HTTP, gRPC, Messaging).
  * **Structured Outcomes:** Standardize how operations report success, failure, and associated information, moving beyond exceptions for expected business flows. This is fundamental to how libraries like **Zentient.Results** operate.
  * **Extensibility:** Provides interfaces that can be easily implemented and extended to fit diverse application requirements.
  * **Testability:** Decoupled interfaces promote easier unit testing of business logic.
  * **Consistency:** Enforces a consistent approach to common application concerns across your codebase, **serving as the common language for all Zentient Framework components.**

## 🤝 Contributing

We welcome contributions\! If you're interested in contributing to Zentient.Abstractions, please visit our [GitHub Repository](https://github.com/ulfbou/Zentient.Endpoints) and refer to the [`CONTRIBUTING.md`](https://www.google.com/search?q=https://github.com/ulfbou/Zentient.Endpoints/blob/main/CONTRIBUTING.md) guide.

## 📄 License

Zentient.Abstractions is licensed under the MIT License. See the [`LICENSE`](https://www.google.com/search?q=https://github.com/ulfbou/Zentient.Endpoints/blob/main/LICENSE) file for more details.
