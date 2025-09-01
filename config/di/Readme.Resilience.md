# Resilience Support Library for .NET

## Introduction

The Resilience Support Library is a reusable.NET library designed to enhance application resilience and user experience by providing features for handling exceptions, retries, fallbacks, and feature toggling. This library integrates seamlessly with existing.NET infrastructure, providing developers with a robust and flexible toolkit to build resilient applications.

## Project Structure

1. Class Library Project: Developed in C# using .NET 8+.

2. Namespace: Zentient.Exceptions 

## Dependencies

The following NuGet packages are required:

• Polly (for retry and fallback policies)
• Serilog (for logging)
• Microsoft.Extensions. DependencyInjection (for IServiceProvider integration)

## Design Overview

1. Utility Classes:

• LoggingUtility: Wraps a Serilog Logger instance to provide logging methods.

• RetryUtility: Utilizes Polly's retry and fallback policies.

• FeatureFlagsUtility: Manages feature flags and controls feature visibil↓

2. Integration:

• Integrates with IService Provider and ILogger.
• Supports attribute-based service registration for retry and fallback policies.

Includes a custom XML configuration section for feature flags and logging behavior.

## Implementation Details

### LoggingUtility Class

• Wraps a Serilog Logger instance.

Provides methods for logging at different levels (LogError, LogWarning, LogInfo).

Configures logging behavior based on exception severity.

RetryUtility Class

Wraps Polly's retry and fallback policies.

• Provides methods for defining and executing retry policies (Retry, RetryWithFallback).

FeatureFlagsUtility Class

Manages feature flags and controls feature visibility.

Implements IsFeatureEnabled method to check feature status based on flags.

Attribute-Based Service Registration

• Custom attributes (RetryAttribute, FallbackAttribute) for declarative configuration.

• Extension method for IServiceCollection to register services with retry and fallback policies.

Custom XML Configuration Section

Defines a section for users to specify feature flags and configure logging behavior.

Installation

1. Ensure you have.NET 8+ installed.

2. Add the required NuGet packages to your project:

```
dotnet add package Polly
dotnet add package Serilog
dotnet add package Microsoft.Extensions.DependencyInjection
```

3. Include the library in your project by adding a reference to the compiled DLL or using it as a NuGet package.

Usage

LoggingUtility Example

```csharp
var logger = LoggingUtility.CreateLogger(); logger.LogError("An error occurred");
```

Retry Utility Example

```csharp
RetryUtility.Retry(() => { // Your operation here });
```

Feature FlagsUtility Example

```csharp
var logger = LoggingUtility.CreateLogger(); logger.LogError("An error occurred");
```

RetryUtility Example

```csharp
RetryUtility.Retry(() => { // Your operation here });
```

Feature FlagsUtility Example

```csharp
if (FeatureFlagsUtility. IsFeatureEnabled("NewFeatur { // Execute feature-specific code
}
```

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests.

## License

This project is licensed under the MIT License. 